using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesListTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\RuleListBCTC.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\TechCrit61009Violations.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\RuleListBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.CurrentSnapshot = TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\TechCrit61009Violations.json");

            var component = new RulesList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Criticality", "Weight", "Grade", "Technical Criterion", "Rule Name", "# Viol.", "Successful Checks" });
            expectedData.AddRange(new List<string> { "µ", "9", "3.56", "Complexity - Algorithmic and Control Structure Complexity", "Action Mappings should have few forwards", "58", "3,117" });
            expectedData.AddRange(new List<string> { "", "45", "3.56", "Complexity - Algorithmic and Control Structure Complexity", "Avoid accessing data by using the position and length", "123", "1,234" });
            expectedData.AddRange(new List<string> { "", "72", "3.56", "Complexity - Algorithmic and Control Structure Complexity", "Avoid artifacts having recursive calls", "26", "4,767" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\RuleListBCTC.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\TechCrit61009Violations.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\RuleListBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.CurrentSnapshot = TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\TechCrit61009Violations.json");

            var component = new RulesList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" },
                {"COUNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Criticality", "Weight", "Grade", "Technical Criterion", "Rule Name", "# Viol.", "Successful Checks" });
            expectedData.AddRange(new List<string> { "µ", "9", "3.56", "Complexity - Algorithmic and Control Structure Complexity", "Action Mappings should have few forwards", "58", "3,117" });
            expectedData.AddRange(new List<string> { "", "45", "3.56", "Complexity - Algorithmic and Control Structure Complexity", "Avoid accessing data by using the position and length", "123", "1,234" });
            TestUtility.AssertTableContent(table, expectedData, 7, 3);
        }
    }
}

