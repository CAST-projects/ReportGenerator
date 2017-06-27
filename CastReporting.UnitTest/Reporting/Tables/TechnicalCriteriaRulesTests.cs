using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TechnicalCriteriaRulesTests
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
        [DeploymentItem(@".\Data\cc60011.json", "Data")]
        [DeploymentItem(@".\Data\nc60011.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern1634.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern4592.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\RuleListBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.CurrentSnapshot = TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\TechCrit61009Violations.json");
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @".\Data\cc60011.json", null);
            reportData = TestUtility.AddNonCriticalRuleViolations(reportData, 60011, @".\Data\nc60011.json", null);


            var component = new TechnicalCriteriaRules();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BZID","60011" },
                {"TCID","61009" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Description", "# Violations" });
            expectedData.AddRange(new List<string> { "Avoid hiding static Methods", "Hiding Static Methods is not allowed. This Quality Rule retrieves all static methods that are redefined in subclasses i.e. \"implicitly hidden\". A Static Method MyMethod of Class MySuperClass is \"implicitly hidden\" in Subclass MySubClass if MySubClass contains a similar declaration of MyMethod (i.e. same signature).", "124" });
            expectedData.AddRange(new List<string> { "Avoid accessing data by using the position and length", "This rule searches for Cobol programs accessing part of data by using a position and a length.", "123" });
            expectedData.AddRange(new List<string> { "Action Mappings should have few forwards", "All Action Mappings with more than 5 forward will be listed.", "58" });
            expectedData.AddRange(new List<string> { "Avoid unreferenced Tables", "List of schema tables that are not called", "58" });
            expectedData.AddRange(new List<string> { "Avoid Artifacts with High Cyclomatic Complexity", "Retrieves all Artifacts having a Cyclomatic Complexity greater than the specified threshold . Cyclomatic Complexity is a measure of the complexity of the control structure of an Artifact. It is the number of linearly independent paths and therefore, the minimum number of independent paths when executing the software. \nThe threshold is a parameter and can be changed at will.", "35" });
            TestUtility.AssertTableContent(table, expectedData, 3, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\RuleListBCTC.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\TechCrit61009Violations.json", "Data")]
        [DeploymentItem(@".\Data\cc60011.json", "Data")]
        [DeploymentItem(@".\Data\nc60011.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern1634.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern4592.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\RuleListBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.CurrentSnapshot = TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\TechCrit61009Violations.json");
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @".\Data\cc60011.json", null);
            reportData = TestUtility.AddNonCriticalRuleViolations(reportData, 60011, @".\Data\nc60011.json", null);

            var component = new TechnicalCriteriaRules();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BZID","60011" },
                {"TCID","61009" },
                {"CNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Description", "# Violations" });
            expectedData.AddRange(new List<string> { "Avoid hiding static Methods", "Hiding Static Methods is not allowed. This Quality Rule retrieves all static methods that are redefined in subclasses i.e. \"implicitly hidden\". A Static Method MyMethod of Class MySuperClass is \"implicitly hidden\" in Subclass MySubClass if MySubClass contains a similar declaration of MyMethod (i.e. same signature).", "124" });
            expectedData.AddRange(new List<string> { "Avoid accessing data by using the position and length", "This rule searches for Cobol programs accessing part of data by using a position and a length.", "123" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }
    }
}

