using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class QualityRuleViolationsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        // TODO :
        // - test with no bcid / bcid = 60016
        // - test with no count / count = 6
        // - test with shortname / fullname
        // - test with TQI (60017) and "Avoid using SQL queries inside a loop" (7424) => no pri
        // - test with Security (60016) and "Avoid Methods with a very low comment/code ratio" (7846) => pri
        // - test with current / previous snapshot
        // - test with only one snapshot (previous does not exists= => no status column

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestOneSnapshotShortNames()
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

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60017" },
                {"ID","7424" },
                {"COUNT","6" },
                {"NAME","SHORT" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Objects in violation for rule Avoid using SQL queries inside a loop" });
            expectedData.AddRange(new List<string> { "adg_central_grades_std" });
            expectedData.AddRange(new List<string> { "adg_central_startup_init" });
            expectedData.AddRange(new List<string> { "adg_init_techno_children" });
            expectedData.AddRange(new List<string> { "adg_m_central_grades_std" });
            expectedData.AddRange(new List<string> { "adg_m_central_startup_init" });
            expectedData.AddRange(new List<string> { "adgc_delta_debt_added" });
            TestUtility.AssertTableContent(table, expectedData, 1, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestTwoSnapshotFullNames()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60017" },
                {"ID","7424" },
                {"COUNT","6" },
                {"NAME","FULL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Objects in violation for rule Avoid using SQL queries inside a loop", "Status" });
            expectedData.AddRange(new List<string> { "aedtst_exclusions_central.adg_central_grades_std", "added" });
            expectedData.AddRange(new List<string> { "aedtst_exclusions_central.adg_central_startup_init", "added" });
            expectedData.AddRange(new List<string> { "aedtst_exclusions_central.adg_init_techno_children", "added" });
            expectedData.AddRange(new List<string> { "aedtst_exclusions_central.adg_m_central_grades_std", "updated" });
            expectedData.AddRange(new List<string> { "aedtst_exclusions_central.adg_m_central_startup_init", "updated" });
            expectedData.AddRange(new List<string> { "aedtst_exclusions_central.adgc_delta_debt_added", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 2, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestNoBcidNoNameNoCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7846" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Objects in violation for rule Avoid Methods with a very low comment/code ratio", "PRI", "Status" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.dumpStack", "122,280", "added" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.dumpStack", "65,880", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.logInBase", "54,120", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.logInBase", "43,880", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.getFormattedMsg", "35,750", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.getFormattedMsg", "33,825", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.runProcedure", "24,780", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.consolidation.AadConsolidation.manageNodeID", "7,950", "updated" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.executeDeleteQuery", "5,940", "unchanged" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.truncateTable", "4,360", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 3, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestPreviousSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7846" },
                {"SNAPSHOT","PREVIOUS" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Objects in violation for rule Avoid Methods with a very low comment/code ratio", "PRI" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.dumpStack", "122,280" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.dumpStack", "65,880" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.logInBase", "54,120" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.logInBase", "43,880" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.getFormattedMsg", "35,750" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.getFormattedMsg", "33,825" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.runProcedure", "24,780" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.consolidation.AadConsolidation.manageNodeID", "7,950" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.executeDeleteQuery", "5,940" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aed.common.AedCommandLine.truncateTable", "4,360" });
            TestUtility.AssertTableContent(table, expectedData, 2, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestPreviousButNoPreviousSnapshot()
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

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7846" },
                {"SNAPSHOT", "PREVIOUS" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Objects in violation for rule Avoid Methods with a very low comment/code ratio", "PRI" });
            expectedData.AddRange(new List<string> { "-" });
            TestUtility.AssertTableContent(table, expectedData, 2, 11);
        }

    }
}

