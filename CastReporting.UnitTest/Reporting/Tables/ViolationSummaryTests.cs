using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ViolationSummaryTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\QRviolRatioModules.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\QRviolRatioModules.json", "AED/applications/3/snapshots/5", "Snap4_CAIP-8.3ra2_RG-1.6.a", "8.3.ra2", currentDate,
               null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ViolationSummary();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Grade", "Total Checks", "Added", "Removed", "Critical" });
            expectedData.AddRange(new List<string> { "Avoid declaring public Fields", "4.00", "113", "0","18", "X" });
            expectedData.AddRange(new List<string> { "Close SQL connection ASAP", "4.00", "1,110", "2", "7", "X" });
            expectedData.AddRange(new List<string> { "Avoid declaring Public Instance Variables", "3.08", "435", "0", "0", "X" });
            expectedData.AddRange(new List<string> { "Avoid hiding static Methods", "1.00", "70", "23", "7", "X" });
            expectedData.AddRange(new List<string> { "Avoid using Fields (non static final) from other Classes", "1.00", "642", "123", "456", "X" });
            TestUtility.AssertTableContent(table, expectedData, 6, 6);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\QRviolRatioModules.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestModuleContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\QRviolRatioModules.json", "AED/applications/3/snapshots/5", "Snap4_CAIP-8.3ra2_RG-1.6.a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ViolationSummary();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"MODULES","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Module Name", "Rule Name", "Grade", "Total Checks", "Added", "Removed", "Critical" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "Avoid declaring Public Instance Variables", "4.00", "93", "0", "34", "X" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "Avoid hiding static Methods", "1.00", "38", "23", "7", "X" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "Avoid using Fields (non static final) from other Classes", "1.00", "346", "0", "0", "X" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\QRviolRatioModules.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\QRviolRatioModules.json", "AED/applications/3/snapshots/5", "Snap4_CAIP-8.3ra2_RG-1.6.a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ViolationSummary();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Grade", "Total Checks", "Added", "Removed", "Critical" });
            expectedData.AddRange(new List<string> { "Avoid declaring public Fields", "4.00", "113", "0", "18", "X" });
            expectedData.AddRange(new List<string> { "Close SQL connection ASAP", "4.00", "1,110", "2", "7", "X" });
            TestUtility.AssertTableContent(table, expectedData, 6, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\QRviolRatioModules.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestNoGradeNoCritical()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\QRviolRatioModules.json", "AED/applications/3/snapshots/5", "Snap4_CAIP-8.3ra2_RG-1.6.a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ViolationSummary();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"GRADE", "0" },
                {"CRITICAL", "0" },
                {"NONCRITICAL", "1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Total Checks", "Added", "Removed", "Critical" });
            expectedData.AddRange(new List<string> { "Avoid unreferenced Tables", "931", "159", "78", "" });
            expectedData.AddRange(new List<string> { "Namespace naming convention - case control", "45", "0", "0", "" });
            expectedData.AddRange(new List<string> { "Interface naming convention - case and character set control", "9", "0", "0", "" });
            expectedData.AddRange(new List<string> { "Enumerations naming convention - case and character set control", "23", "0", "0", "" });
            expectedData.AddRange(new List<string> { "Enumeration Items naming convention - case and character set control", "136", "0", "0", "" });
            TestUtility.AssertTableContent(table, expectedData, 5, 6);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\QRviolRatioModules.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestFailedChecks()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\QRviolRatioModules.json", "AED/applications/3/snapshots/5", "Snap4_CAIP-8.3ra2_RG-1.6.a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ViolationSummary();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ADDEDREMOVED", "0" },
                {"TOTAL","0" },
                {"FAILED","1" },
                {"SUCCESSFUL","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Grade", "# Violations", "Successful Checks", "Critical" });
            expectedData.AddRange(new List<string> { "Avoid declaring public Fields", "4.00", "0", "113", "X" });
            expectedData.AddRange(new List<string> { "Close SQL connection ASAP", "4.00", "0", "1,110", "X" });
            expectedData.AddRange(new List<string> { "Avoid declaring Public Instance Variables", "3.08", "2", "433", "X" });
            expectedData.AddRange(new List<string> { "Avoid hiding static Methods", "1.00", "3", "67", "X" });
            expectedData.AddRange(new List<string> { "Avoid using Fields (non static final) from other Classes", "1.00", "76", "566", "X" });
            TestUtility.AssertTableContent(table, expectedData, 5, 6);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\QRviolRatioModules.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public void TestModuleComplianceContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\QRviolRatioModules.json", "AED/applications/3/snapshots/5", "Snap4_CAIP-8.3ra2_RG-1.6.a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ViolationSummary();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"MODULES","1" },
                {"COMPLIANCE", "1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Module Name", "Rule Name", "Grade", "Total Checks", "Compliance", "Added", "Removed", "Critical" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "Avoid declaring Public Instance Variables", "4.00", "93", "100 %", "0", "34", "X" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "Avoid hiding static Methods", "1.00", "38", "94.7 %", "23", "7", "X" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "Avoid using Fields (non static final) from other Classes", "1.00", "346", "87.0 %", "0", "0", "X" });
            TestUtility.AssertTableContent(table, expectedData, 8, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
