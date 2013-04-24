using System.Drawing;
using Novacode;
using System.Linq;
using dokimi.core;

namespace dokimi.Printers
{
    namespace ConsoleApplication1
    {
        public class WordSpecSuitePrinter : ISpecSuitePrinter
        {
            private readonly string _fileName;

            public WordSpecSuitePrinter(string fileName)
            {
                _fileName = fileName;
            }

            public void Print(SpecSuite suite)
            {
                if (suite.Categories.Any())
                {
                    using (DocX document = DocX.Create(string.Format("{0}.docx", _fileName)))
                    {
                        foreach (var category in suite.Categories)
                            printCategory(category, document);

                        document.Save();
                    }
                }
            }

            private void printCategory(SpecCategory category, DocX document)
            {
                document.InsertParagraph()
                        .Append(category.ToString())
                        .Font(new FontFamily("Cambria"))
                        .FontSize(26)
                        .Color(Color.FromArgb(255, 23, 54, 93))
                        .Spacing(2.0f)
                        .AppendLine();

                foreach (var spec in category.Specs)
                    printSpec(document, spec);

            }

            private static void printSpec(DocX document, SpecInfo spec)
            {
                printName(document, spec);
                printGiven(document, spec);
                printWhen(document, spec);
                printThen(document, spec);

                document.InsertParagraph();
            }

            private static void printGiven(DocX document, SpecInfo spec)
            {
                document.InsertParagraph()
                    .Append("Given")
                    .Font(new FontFamily("Cambria"))
                        .FontSize(13)
                        .Bold()
                        .Color(Color.FromArgb(255, 79, 129, 189))
                        ;


                foreach (var given in spec.Givens)
                {
                    var description = spec.HasExecuted
                                          ? string.Format("{0} [{1}]", given.Description,
                                                          given.Passed ? "Passed" : "Failed")
                                          : given.Description;

                    var p = document.InsertParagraph()
                                    .Append(description)
                                    .Color(spec.HasExecuted ? (given.Passed ? Color.Green : Color.Red) : Color.Black)
                                    .FontSize(12);

                    p.IndentationBefore = 0.5f;
                }


                if (!spec.Givens.All(x => x.Passed) && spec.Exception != null)
                {
                    document.InsertParagraph()
                            .Append(spec.Exception.ToString())
                            .Color(Color.Black)
                            .FontSize(12)
                            .IndentationBefore = 0.5f;
                }
            }

            private static void printWhen(DocX document, SpecInfo spec)
            {
                document.InsertParagraph()
                        .Append("When")
                        .Font(new FontFamily("Cambria"))
                        .FontSize(13)
                        .Bold()
                        .Color(Color.FromArgb(255, 79, 129, 189));


                var p = document.InsertParagraph()
                    .Append(spec.HasExecuted ? string.Format("{0} [{1}]", spec.When.Description, spec.When.Passed ? "Passed" : "Failed") : spec.When.Description)
                        .Color(spec.HasExecuted ? (spec.When.Passed ? Color.Green : Color.Red) : Color.Black)
                        .FontSize(12);

                if (shouldDisplayWhenException(spec))
                {
                    p.AppendLine(spec.Exception.ToString());
                    p.AppendLine(spec.Exception.StackTrace);
                }

                p.IndentationBefore = 0.5f;

            }

            private static bool shouldDisplayWhenException(SpecInfo spec)
            {
                return !spec.When.Passed && spec.Exception != null
                    && spec.Givens.All(x =>x.Passed);
            }

            private static void printThen(DocX document, SpecInfo spec)
            {
                document.InsertParagraph()
                    .Append("Then")
                    .Font(new FontFamily("Cambria"))
                        .FontSize(13)
                        .Bold()
                        .Color(Color.FromArgb(255, 79, 129, 189));

                foreach (var then in spec.Thens)
                {
                    var description = spec.HasExecuted
                                          ? string.Format("{0} [{1}]", then.Description, then.Passed ? "Passed" : "Failed")

                                          : then.Description;
                    var p = document.InsertParagraph()
                            .Append(description)
                            .FontSize(12)
                            .Color(spec.HasExecuted ? (then.Passed ? Color.Green : Color.Red) : Color.Black);

                    if (!then.Passed && then.Exception != null)
                        p.AppendLine(then.Exception.ToString());

                    p.IndentationBefore = 0.5f;
                }
            }

            private static void printName(DocX document, SpecInfo spec)
            {
                document.InsertParagraph()
                        .Append(spec.Name)
                        .Font(new FontFamily("Cambria"))
                        .FontSize(13)
                        .Bold()
                        .Color(Color.FromArgb(255, 54, 95, 145))
                        ;
            }
        }
    }
}