using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ViolationStatisticsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\critViolStats.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new ViolationStatistics();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Value" });
            expectedData.AddRange(new List<string> { "Critical Violations", "75"});
            expectedData.AddRange(new List<string> { "  per File", "0.22"});
            expectedData.AddRange(new List<string> { "  per kLoC", "3.53" });
            expectedData.AddRange(new List<string> { "Complex Objects", "243"});
            expectedData.AddRange(new List<string> { "  With Violations", "103" });
            TestUtility.AssertTableContent(table, expectedData, 2, 6);
            Assert.IsFalse(table.HasColumnHeaders);
            Assert.IsTrue(table.HasRowHeaders);
        }

        // test case numCritPerFile == -1
        [TestMethod]
        [DeploymentItem(@".\Data\cocraCritViolStats.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestNegativePerFile()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\cocraCritViolStats.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new ViolationStatistics();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Value" });
            expectedData.AddRange(new List<string> { "Critical Violations", "1,411" });
            expectedData.AddRange(new List<string> { "  per File", "n/a" });
            expectedData.AddRange(new List<string> { "  per kLoC", "0.91" });
            expectedData.AddRange(new List<string> { "Complex Objects", "243" });
            expectedData.AddRange(new List<string> { "  With Violations", "103" });
            TestUtility.AssertTableContent(table, expectedData, 2, 6);
            Assert.IsFalse(table.HasColumnHeaders);
            Assert.IsTrue(table.HasRowHeaders);
        }
    }
}
