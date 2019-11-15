using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ViolationStatisticsEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        [DeploymentItem(@".\Data\critViolStatsPrevious.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\critViolStats.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, @".\Data\critViolStatsPrevious.json", "AED/applications/3/snapshots/4", "Snap5_CAIP-8.3ra_RG-1.5.0", "8.3.ra", previousDate);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new ViolationStatisticsEvolution();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "% Evolution" });
            expectedData.AddRange(new List<string> { "Critical Violations", "75", "97", "-22.7 %"});
            expectedData.AddRange(new List<string> { "  per File", "0.22", "0.26", "-15.4 %"});
            expectedData.AddRange(new List<string> { "  per kLoC", "3.53", "4.34", "-18.7 %" });
            expectedData.AddRange(new List<string> { "Complex Objects", "243", "238", "+2.10 %"});
            expectedData.AddRange(new List<string> { "  With Violations", "103", "99", "+4.04 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 6);
            Assert.IsFalse(table.HasColumnHeaders);
            Assert.IsTrue(table.HasRowHeaders);
        }

        // test case numCritPerFile == -1
        [TestMethod]
        [DeploymentItem(@".\Data\cocraCritViolStats.json", "Data")]
        [DeploymentItem(@".\Data\cocraCritViolStatsPrevious.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestNegativePerFile()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\cocraCritViolStats.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, @".\Data\cocraCritViolStatsPrevious.json", "AED/applications/3/snapshots/4", "Snap5_CAIP-8.3ra_RG-1.5.0", "8.3.ra", previousDate);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new ViolationStatisticsEvolution();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "% Evolution" });
            expectedData.AddRange(new List<string> { "Critical Violations", "1,411", "680", "+108 %" });
            expectedData.AddRange(new List<string> { "  per File", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "  per kLoC", "0.91", "10.89", "-91.6 %" });
            expectedData.AddRange(new List<string> { "Complex Objects", "243", "238", "+2.10 %" });
            expectedData.AddRange(new List<string> { "  With Violations", "103", "99", "+4.04 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 6);
            Assert.IsFalse(table.HasColumnHeaders);
            Assert.IsTrue(table.HasRowHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\critViolStats.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new ViolationStatisticsEvolution();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "% Evolution" });
            expectedData.AddRange(new List<string> { "Critical Violations", "75", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "  per File", "0.22", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "  per kLoC", "3.53", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Complex Objects", "243", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "  With Violations", "103", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 4, 6);
            Assert.IsFalse(table.HasColumnHeaders);
            Assert.IsTrue(table.HasRowHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\critViolStats.json", "Data")]
        [DeploymentItem(@".\Data\critViolStatsPrevious.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestContentInChinese()
        {
            TestUtility.SetCulture("zh-CN");
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\critViolStats.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, @".\Data\critViolStatsPrevious.json", "AED/applications/3/snapshots/4", "Snap5_CAIP-8.3ra_RG-1.5.0", "8.3.ra", previousDate);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new ViolationStatisticsEvolution();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "名称", "当前", "上一个", "%演变" });
            expectedData.AddRange(new List<string> { "关键违规数", "75", "97", "-22.7%" });
            expectedData.AddRange(new List<string> { "  每文件", "0.22", "0.26", "-15.4%" });
            expectedData.AddRange(new List<string> { "  每千行代码", "3.53", "4.34", "-18.7%" });
            expectedData.AddRange(new List<string> { "复杂对象", "243", "238", "+2.10%" });
            expectedData.AddRange(new List<string> { "  含违规", "103", "99", "+4.04%" });
            TestUtility.AssertTableContent(table, expectedData, 4, 6);
            Assert.IsFalse(table.HasColumnHeaders);
            Assert.IsTrue(table.HasRowHeaders);
        }

    }
}
