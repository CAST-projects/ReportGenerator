using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesListStatisticsRatioTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestCriticalTCMetrics()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","66070" },
                {"CRITICAL","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "Action Mappings should have few forwards (7132)","77","8","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestNonCriticalTCMetrics()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","66070" },
                {"CRITICAL","false" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "Avoid Methods with a very low comment/code ratio (7846)","128","8","2",
                "Action Mappings should have few forwards (7132)","77","8","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestRulesSortedMetrics()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","7846|7132|7424" },
                {"COMPLIANCE", "true" },
                {"SORTED","COMPLIANCE" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities","Compliance Score (%)",
                "Avoid using SQL queries inside a loop (7424)","86","2","3","32.6 %",
                "Avoid Methods with a very low comment/code ratio (7846)","128","8","2","63.4 %",
                "Action Mappings should have few forwards (7132)","77","8","2","63.4 %"
            };

            TestUtility.AssertTableContent(table, expectedData, 5, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestStgTagsSortedMetrics()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","CISQ" },
                {"COMPLIANCE", "true" },
                {"SORTED","TOTAL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities","Compliance Score (%)",
                "Avoid artifacts having recursive calls (7388)","12","8","2","63.4 %",
                "Avoid accessing data by using the position and length (7558)","6","8","2","63.4 %"
            };

            TestUtility.AssertTableContent(table, expectedData, 5, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        public void TestCriticalBCMetrics()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"CRITICAL","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "Action Mappings should have few forwards (7132)","77","8","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestSpecificHeaders()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","66070" },
                {"CRITICAL","false" },
                {"LBL","violations" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Violations","Added Violations","Removed Violations",
                "Avoid Methods with a very low comment/code ratio (7846)","128","8","2",
                "Action Mappings should have few forwards (7132)","77","8","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestNoApplicableRules()
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
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListStatisticsRatio();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","12345" },
                {"CRITICAL","false" },
                {"LBL","vulnerabilities" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CAST Rules","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "No applicable rules for given application","","",""
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 2);

        }

    }
}

