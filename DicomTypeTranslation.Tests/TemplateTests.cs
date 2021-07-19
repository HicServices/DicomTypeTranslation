using Dicom;
using DicomTypeTranslation.Elevation;
using DicomTypeTranslation.Elevation.Serialization;
using DicomTypeTranslation.TableCreation;
using FAnsi.Discovery;
using FAnsi.Discovery.TypeTranslation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TypeGuesser;

namespace DicomTypeTranslation.Tests
{
    class TemplateTests:DatabaseTests
    {
        [Test]
        public void Template_ExampleYaml()
        {
            ImageTableTemplateCollection collection = new ImageTableTemplateCollection();
            ImageTableTemplate table = new ImageTableTemplate();
            
            var colTemplate = new ImageColumnTemplate();
            colTemplate.ColumnName = "mycol";
            colTemplate.AllowNulls = true;
            colTemplate.Type = new DatabaseTypeRequest(typeof(string),100);

            table.Columns = new[] {colTemplate};
            collection.Tables.Add(table);

            TestContext.Write(collection.Serialize());
        }

        [TestCase("CT", FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("CT", FAnsi.DatabaseType.MySql)]
        [TestCase("CT", FAnsi.DatabaseType.Oracle)]
        [TestCase("MR", FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("MR", FAnsi.DatabaseType.MySql)]
        [TestCase("MR", FAnsi.DatabaseType.Oracle)]
        [TestCase("PT", FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("PT", FAnsi.DatabaseType.MySql)]
        [TestCase("PT", FAnsi.DatabaseType.Oracle)]
        [TestCase("NM", FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("NM", FAnsi.DatabaseType.MySql)]
        [TestCase("NM", FAnsi.DatabaseType.Oracle)]
        [TestCase("OTHER",FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("OTHER",FAnsi.DatabaseType.MySql)]
        [TestCase("OTHER",FAnsi.DatabaseType.Oracle)]
        [TestCase("DX",FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("DX",FAnsi.DatabaseType.MySql)]
        [TestCase("DX",FAnsi.DatabaseType.Oracle)]
        [TestCase("SR",FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("SR",FAnsi.DatabaseType.MySql)]
        [TestCase("SR",FAnsi.DatabaseType.Oracle)]
        [TestCase("XA", FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("XA", FAnsi.DatabaseType.MySql)]
        [TestCase("XA", FAnsi.DatabaseType.Oracle)]
        [TestCase("US", FAnsi.DatabaseType.MicrosoftSQLServer)]
        [TestCase("US", FAnsi.DatabaseType.MySql)]
        [TestCase("US", FAnsi.DatabaseType.Oracle)]
        public void TestTemplate(string template, FAnsi.DatabaseType dbType)
        {
            string templateFile = Path.Combine(TestContext.CurrentContext.TestDirectory,"Templates",template + ".it");

            ImageTableTemplateCollection collection = ImageTableTemplateCollection.LoadFrom(File.ReadAllText(templateFile));

            foreach (var tableTemplate in collection.Tables) 
                Validate(tableTemplate,templateFile);
            
            var db = GetTestDatabase(dbType);
            
            var creator = new ImagingTableCreation(db.Server.GetQuerySyntaxHelper());

            foreach(var table in collection.Tables)
            {
                if(string.Equals(table.TableName, "ImageTable",StringComparison.CurrentCultureIgnoreCase))
                {
                    EnforceExpectedImageColumns(template,table);
                }

                var tbl = db.ExpectTable(table.TableName);
                creator.CreateTable(tbl,table);

                Assert.IsTrue(tbl.Exists());
            }
        }

        private void EnforceExpectedImageColumns(string template, ImageTableTemplate table)
        {
            foreach(var req in new[] { "PatientID","DicomFileSize","StudyInstanceUID"})
            {
                if (!table.Columns.Any(c => c.ColumnName.Equals(req)))
                {
                    Assert.Fail($"Template {Path.GetFileName(template)} is missing expected field {req} in section {table.TableName}");
                }
            }
            
        }

        private void Validate(ImageTableTemplate tableTemplate, string templateFile)
        {
            List<Exception> errors = new List<Exception>();

            foreach (var col in tableTemplate.Columns)
            {
                try
                {
                    Assert.LessOrEqual(col.ColumnName.Length,64, $"Column name '{col.ColumnName}' is too long");

                    Regex rSeq = new Regex(@"_([A-Za-z]+)$");
                    var seqMatch = rSeq.Match(col.ColumnName);

                    if (seqMatch.Success)
                    {
                        var leafTag = seqMatch.Groups[1].Value;

                        var tag = DicomDictionary.Default.FirstOrDefault(t => t.Keyword == leafTag);

                        if (tag == null)
                            throw new NotSupportedException($"Leaf tag {leafTag} of sequence column {col.ColumnName} was not a valid dicom tag name");

                        var type = DicomTypeTranslater.GetNaturalTypeForVr(tag.ValueRepresentations, tag.ValueMultiplicity);

                        Assert.AreEqual(type.CSharpType , col.Type.CSharpType,$"Listed Type for column {col.ColumnName} did not match expected Type");
                        
                        // The declared widths must be sufficient to hold the basic leaf node
                        if(type.Width == int.MaxValue)
                            Assert.GreaterOrEqual(col.Type.Width ??0,100,$"Listed Width for column {col.ColumnName} did not match expected minimum Width");
                        else
                            Assert.GreaterOrEqual(col.Type.Width ??0,type.Width ??0 ,$"Listed Width for column {col.ColumnName} did not match expected minimum Width");
                        
                        Assert.AreEqual(type.Size , col.Type.Size,$"Listed Size for column {col.ColumnName} ({DescribeSize(col.Type.Size)}) did not match expected Size ({DescribeSize(type.Size)})");
                    
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }

            if(errors.Any())
                throw new AggregateException($"Errors in file '{templateFile}'",errors.ToArray());
        }

        private string DescribeSize(DecimalSize typeSize)
        {
            return "NumbersBeforeDecimalPlace:" + typeSize.NumbersBeforeDecimalPlace + " NumbersAfterDecimalPlace:" + typeSize.NumbersAfterDecimalPlace;
        }

        [TestCase("SmiTagElevation")]
        public void TestElevationTemplate(string template)
        {
            string templateFile = Path.Combine(TestContext.CurrentContext.TestDirectory,"Templates",template + ".xml");
            
            TagElevationRequestCollection elevation = new TagElevationRequestCollection(File.ReadAllText(templateFile));
            
            //at least one request
            Assert.GreaterOrEqual(elevation.Requests.Count,1);

        }

        /// <summary>
        /// Tests the systems ability to serialize and deserialize <see cref="ImageTableTemplateCollection"/> which has only
        /// columns with DicomTags in them
        /// </summary>
        [Test]
        public void Test_Serializing_DicomTagsOnly()
        {
            ImageTableTemplateCollection collection = new ImageTableTemplateCollection();

            ImageTableTemplate table = new ImageTableTemplate();
            table.TableName = "Fish";
            
            
            table.Columns = new[]
            {
                new ImageColumnTemplate(DicomTag.SOPInstanceUID){IsPrimaryKey = true },
                new ImageColumnTemplate(DicomTag.CurrentPatientLocation)
            };

            collection.Tables.Add(table);

            var yaml = collection.Serialize();

            var collection2 = ImageTableTemplateCollection.LoadFrom(yaml);

            Assert.AreEqual(collection.Tables[0].TableName, collection2.Tables[0].TableName);

            Assert.AreEqual(collection.Tables[0].Columns[0].ColumnName,   collection2.Tables[0].Columns[0].ColumnName );
            Assert.AreEqual(collection.Tables[0].Columns[0].Type, collection2.Tables[0].Columns[0].Type);
            Assert.AreEqual(collection.Tables[0].Columns[0].IsPrimaryKey, collection2.Tables[0].Columns[0].IsPrimaryKey);

            Assert.AreEqual(collection.Tables[0].Columns[1].ColumnName,   collection2.Tables[0].Columns[1].ColumnName);
            Assert.AreEqual(collection.Tables[0].Columns[1].Type, collection2.Tables[0].Columns[1].Type);
            Assert.AreEqual(collection.Tables[0].Columns[1].IsPrimaryKey, collection2.Tables[0].Columns[1].IsPrimaryKey);

            //doesn't actually have to exist
            var db = new DiscoveredServer("localhost","nobody",FAnsi.DatabaseType.MySql,"captain","morgans").ExpectDatabase("neverland");

            ImagingTableCreation creator = new ImagingTableCreation(db.Server.GetQuerySyntaxHelper());
                       

            var sql1 = creator.GetCreateTableSql(db,collection.Tables[0].TableName, collection.Tables[0],null);
            var sql2 = creator.GetCreateTableSql(db, collection2.Tables[0].TableName, collection.Tables[0], null);

            Assert.AreEqual(sql1,sql2);
        }
        
        /// <summary>
        /// Tests the systems ability to serialize and deserialize <see cref="ImageTableTemplateCollection"/> when there
        /// is a mixture of DicomTag and normal columns
        /// </summary>
        [Test]
        public void Test_Serializing_DicomTagsAndArbitraryColumns()
        {
            ImageTableTemplateCollection collection = new ImageTableTemplateCollection();

            ImageTableTemplate table = new ImageTableTemplate();
            table.TableName = "Fish";

            //table has 3 columns, one is a DicomTag (SOPInstanceUID) while the other 2 are arbitrary
            table.Columns = new[]
            {
                new ImageColumnTemplate(DicomTag.SOPInstanceUID){IsPrimaryKey = true },
                new ImageColumnTemplate(DicomTag.SeriesInstanceUID),
                new ImageColumnTemplate(DicomTag.StudyInstanceUID),
                new ImageColumnTemplate(DicomTag.PatientID),
                new ImageColumnTemplate(){
                    ColumnName = "LocationOfFiles",
                    Type = new DatabaseTypeRequest(typeof(string),500,null),
                    AllowNulls = true},
                new ImageColumnTemplate(){
                    ColumnName = "DataQualityEngineScore",
                    Type = new DatabaseTypeRequest(typeof(decimal),null,new DecimalSize(10,5)),
                    AllowNulls = false} 
            };
    

            collection.Tables.Add(table);

            var yaml = collection.Serialize();

            Console.WriteLine("Yaml is:");
            Console.Write(yaml);

            var collection2 = ImageTableTemplateCollection.LoadFrom(yaml);

            Assert.AreEqual(collection.Tables[0].TableName, collection2.Tables[0].TableName);

            Assert.AreEqual(collection.Tables[0].Columns[0].ColumnName,     collection2.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual(collection.Tables[0].Columns[0].Type,           collection2.Tables[0].Columns[0].Type);
            Assert.AreEqual(collection.Tables[0].Columns[0].IsPrimaryKey,   collection2.Tables[0].Columns[0].IsPrimaryKey);

            Assert.AreEqual(collection.Tables[0].Columns[4].ColumnName,     collection2.Tables[0].Columns[4].ColumnName);
            Assert.AreEqual(collection.Tables[0].Columns[4].Type,           collection2.Tables[0].Columns[4].Type);
            Assert.AreEqual(collection.Tables[0].Columns[4].IsPrimaryKey,   collection2.Tables[0].Columns[4].IsPrimaryKey);
            
            Assert.AreEqual(collection.Tables[0].Columns[5].ColumnName,     collection2.Tables[0].Columns[5].ColumnName);
            Assert.AreEqual(collection.Tables[0].Columns[5].Type,           collection2.Tables[0].Columns[5].Type);
            Assert.AreEqual(collection.Tables[0].Columns[5].IsPrimaryKey,   collection2.Tables[0].Columns[5].IsPrimaryKey);

            //doesn't actually have to exist
            var db = new DiscoveredServer("localhost", "nobody", FAnsi.DatabaseType.MySql, "captain", "morgans").ExpectDatabase("neverland");

            ImagingTableCreation creator = new ImagingTableCreation(db.Server.GetQuerySyntaxHelper());

            var sql1 = creator.GetCreateTableSql(db, collection.Tables[0].TableName, collection.Tables[0], null);
            var sql2 = creator.GetCreateTableSql(db, collection2.Tables[0].TableName, collection.Tables[0], null);

            Assert.AreEqual(sql1, sql2);
        }

    }
}

