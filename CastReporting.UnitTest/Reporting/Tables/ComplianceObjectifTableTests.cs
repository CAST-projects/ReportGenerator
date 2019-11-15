using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ComplianceToObjectifTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017.json", "Data")]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017.json", null);
            reportData = TestUtility.AddSizingResults(reportData, @".\Data\critViolStats.json", null);

            var component = new ComplianceObjectifTable();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Objectives", "Achievement", "Achievement ratio"});
            expectedData.AddRange(new List<string> { "Entire Application (whole code)", "8", "3", "38 %" });
            expectedData.AddRange(new List<string> { "Last Delivery (new and modified)", "8", "6", "75 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017.json", "Data")]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        [DeploymentItem(@".\Data\cc60017Previous.json", "Data")]
        [DeploymentItem(@".\Data\critViolStatsPrevious.json", "Data")]
        public void TestTwoSnapshots()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017.json", @".\Data\cc60017Previous.json");
            reportData = TestUtility.AddSizingResults(reportData, @".\Data\critViolStats.json", @".\Data\critViolStatsPrevious.json");

            var component = new ComplianceObjectifTable();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Objectives", "Achievement", "Achievement ratio" });
            expectedData.AddRange(new List<string> { "Entire Application (whole code)", "8", "3", "38 %" });
            expectedData.AddRange(new List<string> { "Last Delivery (new and modified)", "8", "6", "75 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017.json", "Data")]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        [DeploymentItem(@".\Data\cc60017Previous.json", "Data")]
        [DeploymentItem(@".\Data\critViolStatsPrevious.json", "Data")]
        public void TestShortHeader()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017.json", @".\Data\cc60017Previous.json");
            reportData = TestUtility.AddSizingResults(reportData, @".\Data\critViolStats.json", @".\Data\critViolStatsPrevious.json");

            var component = new ComplianceObjectifTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"HEADER", "SHORT"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Obj.", "Achiev.", "%" });
            expectedData.AddRange(new List<string> { "Entire Application (whole code)", "8", "3", "38 %" });
            expectedData.AddRange(new List<string> { "Last Delivery (new and modified)", "8", "6", "75 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }
    }
}

