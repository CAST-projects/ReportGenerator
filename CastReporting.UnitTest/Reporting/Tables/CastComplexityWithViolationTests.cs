using System.Collections.Generic;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class CastComplexityWithViolationTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestOneSnapshot()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new CastComplexityWithViolation();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "CAST Complexity", "Artifacts", "With Violations" });
            expectedData.AddRange(new List<string> { "Extreme", "71", "29"});
            expectedData.AddRange(new List<string> { "High", "172", "74"});
            expectedData.AddRange(new List<string> { "Average", "1,167", "270" });
            expectedData.AddRange(new List<string> { "Low", "8,881", "221"});
            TestUtility.AssertTableContent(table, expectedData, 3, 4);
            Assert.IsTrue(table.HasColumnHeaders);
            Assert.IsFalse(table.HasRowHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastComplexityWithViolation();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "CAST Complexity", "Artifacts", "With Violations" });
            expectedData.AddRange(new List<string> { "Extreme", "71", "29" });
            expectedData.AddRange(new List<string> { "High", "172", "74" });
            expectedData.AddRange(new List<string> { "Average", "1,167", "270" });
            expectedData.AddRange(new List<string> { "Low", "8,881", "221" });
            TestUtility.AssertTableContent(table, expectedData, 3, 4);
            Assert.IsTrue(table.HasColumnHeaders);
            Assert.IsFalse(table.HasRowHeaders);
        }
    }
}
