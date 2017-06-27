using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TCImprovementOpportunityTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\RuleListBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QIBusinessCriteriaConf.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\TechCrit61009Violations.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\RuleListBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            reportData = TestUtility.AddQIBusinessCriteriaConfiguration(reportData, @".\Data\QIBusinessCriteriaConf.json");
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.CurrentSnapshot = TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\TechCrit61009Violations.json");

            var component = new CastReporting.Reporting.Block.Table.TCImprovementOpportunity();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical criterion name", "# Violations", "Total Checks", "Grade" });
            expectedData.AddRange(new List<string> { "Complexity - Algorithmic and Control Structure Complexity", "443", "17,056", "3.56" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }


    }
}

