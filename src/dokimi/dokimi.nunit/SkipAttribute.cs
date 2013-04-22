using System;
using NUnit.Framework;

namespace dokimi.nunit
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SkipAttribute : IgnoreAttribute, dokimi.core.SkipAttribute
    {
        public SkipAttribute(string reason) : base(reason)
        {
        }
    }
}