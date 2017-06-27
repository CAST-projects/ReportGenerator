using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RuleNameAndDescriptionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern1634.json", "Data")]
        [DeploymentItem(@".\Data\RuleViolation1634.json", "Data")]
        public void TestOneRule()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RuleNameAndDescription();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"RULID","1634" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Avoid unreferenced Tables", null});
            expectedData.AddRange(new List<string> { "Rationale", "All Tables should be referenced.\nUnreferenced tables may be the symptom of Dead Code. And dead Code must be avoided as it makes source code less readable and increases the cost of the software maintenance.\n\n\nWarning:\nUnreference code can also be the symptoms of missing code (code not included in the source code analysis) and / or can be the symptom of use of polymorphism." });
            expectedData.AddRange(new List<string> { "Description", "List of schema tables that are not called" });
            expectedData.AddRange(new List<string> { "Remediation", "Check if the table  is truly unnecessary.\nRemoveit if so" });
            expectedData.AddRange(new List<string> { "# Violations", "209" });
            TestUtility.AssertTableContent(table, expectedData, 2, 5);
        }

    }
}

