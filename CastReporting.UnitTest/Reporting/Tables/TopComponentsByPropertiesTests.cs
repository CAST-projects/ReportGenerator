using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TopComponentsByPropertiesTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\ComponentsWithProperties.json", "Data")]
        public void TestContentCycloFanOut()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.TopComponentsByProperties();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PROP1","cyclomaticComplexity" },
                {"PROP2","fanOut" },
                {"ORDER1","desc" },
                {"ORDER2","asc" },
                {"COUNT","5" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Cyclomatic Complexity", "Fan-Out"});
            expectedData.AddRange(new List<string> { "com.castsoftware.util.string.StringHelper.encodeString", "65", "8" });
            expectedData.AddRange(new List<string> { "com.castsoftware.util.string.StringHelper.isEncodedString", "59", "4" });
            expectedData.AddRange(new List<string> { "com.castsoftware.graph.GraphDoc.Definition.setProperty", "57", "9" });
            expectedData.AddRange(new List<string> { "com.castsoftware.graph.GraphDoc.Database.submit", "52", "31" });
            expectedData.AddRange(new List<string> { "com.castsoftware.viewer.data.History.initCodeLastHistory", "52", "30" });
            TestUtility.AssertTableContent(table, expectedData, 3, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\ComponentsWithProperties.json", "Data")]
        public void TestContentCycloLowDoc()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.TopComponentsByProperties();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PROP1","cyclomaticComplexity" },
                {"PROP2","ratioCommentLinesCodeLines" },
                {"ORDER1","desc" },
                {"ORDER2","asc" },
                {"COUNT","5" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Cyclomatic Complexity", "Documentation Ratio" });
            expectedData.AddRange(new List<string> { "com.castsoftware.util.string.StringHelper.encodeString", "65", "0.00" });
            expectedData.AddRange(new List<string> { "com.castsoftware.util.string.StringHelper.isEncodedString", "59", "0.00" });
            expectedData.AddRange(new List<string> { "com.castsoftware.graph.GraphDoc.Definition.setProperty", "57", "0.00" });
            expectedData.AddRange(new List<string> { "com.castsoftware.graph.GraphDoc.Database.submit", "52", "0.03" });
            expectedData.AddRange(new List<string> { "com.castsoftware.viewer.data.History.initCodeLastHistory", "52", "0.16" });
            TestUtility.AssertTableContent(table, expectedData, 3, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\ComponentsWithProperties.json", "Data")]
        public void TestNoPropertiesAvailableContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.TopComponentsByProperties();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PROP1","cyclomaticPlexity" },
                {"PROP2","ratioCommentLinesCodeLines" },
                {"ORDER1","desc" },
                {"ORDER2","asc" },
                {"COUNT","5" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "", "Documentation Ratio" });
            expectedData.AddRange(new List<string> { "Properties are not available. Only following properties are available : ", "", "" });
            expectedData.AddRange(new List<string> { "codeLines", "", "" });
            expectedData.AddRange(new List<string> { "commentedCodeLines", "", "" });
            expectedData.AddRange(new List<string> { "commentLines", "", "" });
            expectedData.AddRange(new List<string> { "coupling", "", "" });
            expectedData.AddRange(new List<string> { "fanIn", "", "" });
            expectedData.AddRange(new List<string> { "fanOut", "", "" });
            expectedData.AddRange(new List<string> { "cyclomaticComplexity", "", "" });
            expectedData.AddRange(new List<string> { "ratioCommentLinesCodeLines", "", "" });
            expectedData.AddRange(new List<string> { "halsteadProgramLength", "", "" });
            expectedData.AddRange(new List<string> { "halsteadProgramVocabulary", "", "" });
            expectedData.AddRange(new List<string> { "halsteadVolume", "", "" });
            expectedData.AddRange(new List<string> { "distinctOperators", "", "" });
            expectedData.AddRange(new List<string> { "distinctOperands", "", "" });
            expectedData.AddRange(new List<string> { "integrationComplexity", "", "" });
            expectedData.AddRange(new List<string> { "essentialComplexity",  "", "" });
            TestUtility.AssertTableContent(table, expectedData, 3, 17);
        }
    }
}

