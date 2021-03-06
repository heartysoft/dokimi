using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dokimi.core;

namespace dokimi
{
    public class SpecExtractor
    {
        private readonly Dictionary<SpecificationCategory, MessageFormatter> _formatters = new Dictionary<SpecificationCategory, MessageFormatter>();

        public SpecExtractor RegisterFormatter(SpecificationCategory categoryName, MessageFormatter formatter)
        {
            _formatters[categoryName] = formatter;
            return this;
        }

        public SpecSuite DescribeSuite(params Assembly[] assemblies)
        {
            return processSuite(getdescriptionInfo, assemblies);
        }

        public SpecSuite RunSuite(params Assembly[] assemblies)
        {
            return processSuite(getRunInfo, assemblies);
        }

        private SpecSuite processSuite(Func<SpecificationMethodInfo, SpecInfo> processor, params Assembly[] assemblies)
        {
            var suite = new SpecSuite();
            var specs = assemblies
                .SelectMany(getSpecs)
                .Select(x => new { Category = x.Specification.SpecificationCategory, SpecInfo = processor(x) })
                .GroupBy(x => x.Category)
                .Select(group =>
                {
                    var category = new SpecCategory(group.Key);

                    foreach (var g in group)
                        category.Add(g.SpecInfo);

                    return category;
                }).ToArray();

            suite.AddRange(specs);

            return suite;
        }

        private IEnumerable<SpecificationMethodInfo> getSpecs(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => typeof(SpecificationTest).IsAssignableFrom(x));

            var methods =
                types.SelectMany(type => type.GetMethods().Where(x => typeof(Specification).IsAssignableFrom(x.ReturnType)));

            foreach (var methodInfo in methods)
            {
                var type = methodInfo.DeclaringType;
                var instance = Activator.CreateInstance(type);
                yield return new SpecificationMethodInfo(methodInfo.Invoke(instance, new object[0]) as Specification, methodInfo);
            }
        }

        private SpecInfo getRunInfo(SpecificationMethodInfo spec)
        {
            var formatter = getFormatter(spec.Specification);
            var specInfo = new SpecInfo(formatter);
            specInfo.Name = getSpecName(spec.MethodInfo.DeclaringType, spec.MethodInfo);

            spec.UpdateSkipInformation(specInfo);

            if (specInfo.Skipped.IsSkipped)
            {
                spec.Specification.EnrichDescription(specInfo, formatter);
                return specInfo;
            }

            specInfo.ReportSpecExecutionHasTriggered();

            try
            {
                spec.Specification.Run(specInfo, getFormatter(spec.Specification));
            }
            catch (Exception e)
            {
                specInfo.Exception = e;
            }

            return specInfo;
        }

        private SpecInfo getdescriptionInfo(SpecificationMethodInfo spec)
        {
            var formatter = getFormatter(spec.Specification);
            var specInfo = new SpecInfo(formatter);
            specInfo.Name = getSpecName(spec.MethodInfo.DeclaringType, spec.MethodInfo);
            spec.Specification.EnrichDescription(specInfo, getFormatter(spec.Specification));

            spec.UpdateSkipInformation(specInfo);

            return specInfo;
        }

        private static string getSpecName(Type type, MethodInfo methodInfo)
        {
            var raw = type.Name + " " + methodInfo.Name;
            var underscoreReplaced = raw.Replace('_', ' ');

            var charArray = underscoreReplaced.ToCharArray();
            charArray[0] = char.ToUpperInvariant(charArray[0]);
            var firstLetterCapitalised = new string(charArray);

            return firstLetterCapitalised;
        }

        private MessageFormatter getFormatter(Specification spec)
        {
            if (_formatters.ContainsKey(spec.SpecificationCategory))
                return _formatters[spec.SpecificationCategory];

            return MessageFormatterRegistry.GetFormatter(spec.SpecificationCategory);
        }

        private class SpecificationMethodInfo
        {
            public SpecificationMethodInfo(Specification specification, MethodInfo methodInfo)
            {
                Specification = specification;
                MethodInfo = methodInfo;
            }

            public Specification Specification { get; private set; }
            public MethodInfo MethodInfo { get; private set; }

            public void UpdateSkipInformation(SpecInfo specInfo)
            {
                var skipAttribute =
                    MethodInfo.DeclaringType.GetCustomAttributes().FirstOrDefault(x => x is SkipAttributeContract) ??
                    MethodInfo.GetCustomAttributes().FirstOrDefault(x => x is SkipAttributeContract);

                if (skipAttribute == null)
                    return;

                var typed = (SkipAttributeContract)skipAttribute;
                string reason = typed.Reason;

                specInfo.ReportSkipped(reason);
            }
        }
    }
}