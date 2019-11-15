using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesListLargestVariationTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\current.json", "Data")]
        [DeploymentItem(@".\Data\previous.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestIncreasePercent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\previous.json", "AED/applications/3/snapshots/5", "PreVersion 1.4.0 sprint 1 shot 2", "V-1.4.0_Sprint 1_2", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\current.json");
            TestUtility.AddSameTechCritRulesViolations(reportData.PreviousSnapshot, @".\Data\previous.json");

            var component = new CastReporting.Reporting.Block.Table.RulesListLargestVariation();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60016" },
                {"VARIATION","increase" },
                {"DATA","percent" },
                {"COUNT","3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Weight", "Variation", "Rule Name" });
            expectedData.AddRange(new List<string> { "80", "+100 %", "Provide a private default Constructor for utility Classes" });
            expectedData.AddRange(new List<string> { "96", "+0.27 %", "Avoid direct definition of JavaScript Functions in a Web page" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\current.json", "Data")]
        [DeploymentItem(@".\Data\previous.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestIncreaseNumber()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\previous.json", "AED/applications/3/snapshots/5", "PreVersion 1.4.0 sprint 1 shot 2", "V-1.4.0_Sprint 1_2", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\current.json");
            TestUtility.AddSameTechCritRulesViolations(reportData.PreviousSnapshot, @".\Data\previous.json");

            var component = new CastReporting.Reporting.Block.Table.RulesListLargestVariation();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60016" },
                {"VARIATION","increase" },
                {"DATA","number" },
                {"COUNT","3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Weight", "Variation", "Rule Name" });
            expectedData.AddRange(new List<string> { "96", "163", "Avoid direct definition of JavaScript Functions in a Web page" });
            expectedData.AddRange(new List<string> { "160", "54", "Avoid catching an exception of type Exception, RuntimeException, or Throwable" });
            expectedData.AddRange(new List<string> { "160", "38", "The exception Exception should never been thrown. Always Subclass Exception and throw the subclassed Classes." });
            TestUtility.AssertTableContent(table, expectedData, 3, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\current.json", "Data")]
        [DeploymentItem(@".\Data\previous.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestDecreasePercent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\previous.json", "AED/applications/3/snapshots/5", "PreVersion 1.4.0 sprint 1 shot 2", "V-1.4.0_Sprint 1_2", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\current.json");
            TestUtility.AddSameTechCritRulesViolations(reportData.PreviousSnapshot, @".\Data\previous.json");

            var component = new CastReporting.Reporting.Block.Table.RulesListLargestVariation();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60016" },
                {"VARIATION","decrease" },
                {"DATA","percent" },
                {"COUNT","3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Weight", "Variation", "Rule Name" });
            expectedData.AddRange(new List<string> { "22", "+0.94 %", "Track Classes referencing Database objects" });
            expectedData.AddRange(new List<string> { "96", "+0.68 %", "Use of style sheets" });
            expectedData.AddRange(new List<string> { "160", "+0.19 %", "Avoid catching an exception of type Exception, RuntimeException, or Throwable" });
            TestUtility.AssertTableContent(table, expectedData, 3, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\current.json", "Data")]
        [DeploymentItem(@".\Data\previous.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestDecreaseNumber()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\previous.json", "AED/applications/3/snapshots/5", "PreVersion 1.4.0 sprint 1 shot 2", "V-1.4.0_Sprint 1_2", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\current.json");
            TestUtility.AddSameTechCritRulesViolations(reportData.PreviousSnapshot, @".\Data\previous.json");

            var component = new CastReporting.Reporting.Block.Table.RulesListLargestVariation();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60016" },
                {"VARIATION","decrease" },
                {"DATA","number" },
                {"COUNT","3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Weight", "Variation", "Rule Name" });
            expectedData.AddRange(new List<string> { "120", "25", "Avoid using 'System.err' and 'System.out' outside a try catch block" });
            expectedData.AddRange(new List<string> { "22", "4", "Track Classes referencing Database objects" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\current.json", "Data")]
        [DeploymentItem(@".\Data\previous.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestDecreaseNumberNoPrevious()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\current.json");

            var component = new CastReporting.Reporting.Block.Table.RulesListLargestVariation();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60016" },
                {"VARIATION","decrease" },
                {"DATA","number" },
                {"COUNT","3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Weight", "Variation", "Rule Name" });
            expectedData.AddRange(new List<string> { "No enabled item", "", "" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\current.json", "Data")]
        [DeploymentItem(@".\Data\previous.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestNoConfig()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\previous.json", "AED/applications/3/snapshots/5", "PreVersion 1.4.0 sprint 1 shot 2", "V-1.4.0_Sprint 1_2", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            TestUtility.AddSameTechCritRulesViolations(reportData.CurrentSnapshot, @".\Data\current.json");
            TestUtility.AddSameTechCritRulesViolations(reportData.PreviousSnapshot, @".\Data\previous.json");

            var component = new CastReporting.Reporting.Block.Table.RulesListLargestVariation();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Weight", "Variation", "Rule Name" });
            expectedData.AddRange(new List<string> { "120", "25", "Avoid using 'System.err' and 'System.out' outside a try catch block" });
            expectedData.AddRange(new List<string> { "22", "4", "Track Classes referencing Database objects" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }

    }
}

