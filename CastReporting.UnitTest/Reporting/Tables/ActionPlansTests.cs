using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ActionPlansTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ActionPlanSummaryApp3.json", "Data")]
        public void TestContentOneSnap()
        {
            /*
             * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
             * @".\Data\ActionPlanSummaryApp3.json" => AED/applications/3/snapshots/6/action-plan/summary
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationActionPlan(reportData, @".\Data\ActionPlanSummaryApp3.json", null);

            var component = new ActionPlans();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule", "Still Violations (#)", "New Violations (#)" });
            expectedData.AddRange(new List<string> { "Avoid declaring public Fields", "14", "3" });
            expectedData.AddRange(new List<string> { "Avoid instantiations inside loops", "42", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ActionPlanSummaryApp3.json", "Data")]
        [DeploymentItem(@".\Data\ActionPlanSummaryApp3Previous.json", "Data")]
        public void TestContentTwoSnaps()
        {
            /*
             * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
             * @".\Data\ActionPlanSummaryApp3.json" => AED/applications/3/snapshots/6/action-plan/summary
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationActionPlan(reportData, @".\Data\ActionPlanSummaryApp3.json", @".\Data\ActionPlanSummaryApp3Previous.json");

            var component = new ActionPlans();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule", "Still Violations (#)", "New Violations (#)" });
            expectedData.AddRange(new List<string> { "Avoid declaring public Fields", "14", "3" });
            expectedData.AddRange(new List<string> { "Avoid instantiations inside loops", "42", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);

        }

    }
}
