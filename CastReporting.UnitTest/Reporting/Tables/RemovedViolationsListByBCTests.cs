using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RemovedViolationsListByBCTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        [DeploymentItem(@".\Data\RemovedViolations.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestLimitDefaultBC()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RemovedViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","4" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            
            expectedData.AddRange(new List<string> { "Violation Status", "Exclusion Status", "Action Status", "Rule Name", "Weight", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "corrected", "n/a", "n/a", "All types of a serializable Class must be serializable (ASCRM-RLB-2)", "100", "com.castsoftware.util.server.Session.server", "unchanged" });
            expectedData.AddRange(new List<string> { "corrected", "n/a", "solved", "All types of a serializable Class must be serializable (ASCRM-RLB-2)", "100", "com.castsoftware.util.server.user.User.dataManager", "unchanged" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "added", "Avoid Artifacts with high Commented-out Code Lines/Code Lines ratio", "64", "<Default Package>.ButtonOk.actionPerformed", "unchanged" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with high Commented-out Code Lines/Code Lines ratio", "64", "<Default Package>.ConfigOptions.ConfigOptions", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 7, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\BaseQI60012.json", "Data")]
        [DeploymentItem(@".\Data\RemovedViolations-60012.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestBC()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RemovedViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60012" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();

            expectedData.AddRange(new List<string> { "Violation Status", "Exclusion Status", "Action Status", "Rule Name", "Weight", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Cyclomatic Complexity", "21", "com.castsoftware.util.string.StringHelper.encodeString", "deleted" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Cyclomatic Complexity", "21", "com.castsoftware.viewer.data.renderer.TreeNodeRenderer.emitHTML", "deleted" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Depth of Code", "15", "com.castsoftware.viewer.macro.FieldValueWrapMacro.eval", "deleted" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Essential Complexity", "42", "com.castsoftware.util.string.StringHelper.encodeString", "deleted" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Essential Complexity", "42", "com.castsoftware.viewer.macro.FieldValueWrapMacro.eval", "deleted" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Essential Complexity", "42", "Pchit.WebClientRunner.DownloadPage", "deleted" });
            expectedData.AddRange(new List<string> { "corrected", "n/a", "n/a", "Avoid Artifacts with High Fan-In", "21", "com.castsoftware.util.data.IDataManager.getDataList", "unchanged" });
            expectedData.AddRange(new List<string> { "disappeared", "n/a", "n/a", "Avoid Artifacts with High Fan-In", "21", "com.castsoftware.util.row.HashmapRowReader.getRow", "deleted" });
            expectedData.AddRange(new List<string> { "corrected", "n/a", "n/a", "Avoid Artifacts with High Fan-In", "21", "com.castsoftware.util.row.HashmapRowReader.next", "unchanged" });
            expectedData.AddRange(new List<string> { "corrected", "n/a", "n/a", "Avoid Artifacts with High Fan-In", "21", "com.castsoftware.util.row.HashmapRowReader.read", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 7, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\BaseQI60012.json", "Data")]
        [DeploymentItem(@".\Data\RemovedViolations-60012.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestBCBadVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.ServerVersion = "1.7.0.111";
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RemovedViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60012" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();

            expectedData.AddRange(new List<string> { "Violation Status", "Exclusion Status", "Action Status", "Rule Name", "Weight", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "No data found", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            TestUtility.AssertTableContent(table, expectedData, 7, 2);
        }
    }
}

