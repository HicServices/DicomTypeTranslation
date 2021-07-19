using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DicomTypeTranslation.TableCreation;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DicomTypeTranslation.Tests.Templates
{
    class TemplateDocumentationTests
    {
        [Test]
        public void Test_TemplateDocumentation_Generate()
        {
            var files = Directory.EnumerateFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, "Templates"), "*.it");

            StringBuilder sb = new StringBuilder();

            foreach (string file in files)
            {
                var collection = ImageTableTemplateCollection.LoadFrom(File.ReadAllText(file));


                sb.AppendLine("## " + Path.GetFileNameWithoutExtension(file));
                sb.AppendLine();

                foreach (ImageTableTemplate table in collection.Tables)
                {
                    sb.AppendLine("### " + table.TableName);
                    sb.AppendLine();
                    sb.AppendLine("| Field | Description |");
                    sb.AppendLine("| ------------- | ------------- |");

                    foreach (ImageColumnTemplate col in table.Columns)
                    {
                        sb.AppendLine("| " + col.ColumnName + " |  |");
                    }

                    sb.AppendLine();
                }
            }

            TestContext.WriteLine("Suggested Documentation:");
            TestContext.Write(sb.ToString());
        }
    }
}
