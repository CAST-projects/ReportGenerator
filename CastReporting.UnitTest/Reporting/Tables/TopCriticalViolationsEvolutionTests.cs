using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TopCriticalViolationsEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\cc60011.json", "Data")]
        [DeploymentItem(@".\Data\cc60011previous.json", "Data")]
        [DeploymentItem(@".\Data\nc60011.json", "Data")]
        [DeploymentItem(@".\Data\nc60011previous.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @".\Data\cc60011.json", @".\Data\cc60011previous.json");
            reportData = TestUtility.AddNonCriticalRuleViolations(reportData,60011, @".\Data\nc60011.json", @".\Data\nc60011previous.json");

            var component = new CastReporting.Reporting.Block.Table.TopCriticalViolationsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Current", "Previous", "Evolution", "% Evolution"});
            expectedData.AddRange(new List<string> { "Avoid hiding static Methods", "63", "3", "+60", "+2,000 %"});
            expectedData.AddRange(new List<string> { "Avoid using untyped DataSet", "4", "6", "-2", "-33.3 %" });
            expectedData.AddRange(new List<string> { "Suspicious similar method names or signatures in an inheritance tree", "13", "0", "+13", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\cc60011.json", "Data")]
        [DeploymentItem(@".\Data\cc60011previous.json", "Data")]
        [DeploymentItem(@".\Data\nc60011.json", "Data")]
        [DeploymentItem(@".\Data\nc60011previous.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @".\Data\cc60011.json", @".\Data\cc60011previous.json");
            reportData = TestUtility.AddNonCriticalRuleViolations(reportData, 60011, @".\Data\nc60011.json", @".\Data\nc60011previous.json");

            var component = new CastReporting.Reporting.Block.Table.TopCriticalViolationsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Current", "Previous", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "Avoid hiding static Methods", "63", "3", "+60", "+2,000 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
        }

    }
}

