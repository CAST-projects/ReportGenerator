using System.Collections.Generic;
using CastReporting.Reporting.Block.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class CastDistributionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestCastComplexityNoPar()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2" });
            expectedData.AddRange(new List<string> { " ", "0" });
            expectedData.AddRange(new List<string> { "Low", "8881" });
            expectedData.AddRange(new List<string> { "Average", "1167" });
            expectedData.AddRange(new List<string> { "High", "172" });
            expectedData.AddRange(new List<string> { "Very High", "71" });
            expectedData.AddRange(new List<string> { " ", "0" });
            TestUtility.AssertTableContent(table, expectedData, 2, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestCastComplexityOneSnapshot()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67001"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2" });
            expectedData.AddRange(new List<string> { " ", "0" });
            expectedData.AddRange(new List<string> { "Low", "8881" });
            expectedData.AddRange(new List<string> { "Average", "1167" });
            expectedData.AddRange(new List<string> { "High", "172" });
            expectedData.AddRange(new List<string> { "Very High", "71" });
            expectedData.AddRange(new List<string> { " ", "0" });
            TestUtility.AssertTableContent(table, expectedData, 2, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestCastComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67001"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "8881", "8824" });
            expectedData.AddRange(new List<string> { "Average", "1167", "1140" });
            expectedData.AddRange(new List<string> { "High", "172", "170" });
            expectedData.AddRange(new List<string> { "Very High", "71", "68" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestCyclomaticComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65501"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "8305", "8235" });
            expectedData.AddRange(new List<string> { "Average", "838", "824" });
            expectedData.AddRange(new List<string> { "High", "161", "159" });
            expectedData.AddRange(new List<string> { "Very High", "71", "68" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestFourGLComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65601"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "0", "0" });
            expectedData.AddRange(new List<string> { "Average", "0", "0" });
            expectedData.AddRange(new List<string> { "High", "0", "0" });
            expectedData.AddRange(new List<string> { "Very High", "0", "0" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestClassComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66015"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "434", "433" });
            expectedData.AddRange(new List<string> { "Average", "76", "78" });
            expectedData.AddRange(new List<string> { "High", "59", "54" });
            expectedData.AddRange(new List<string> { "Very High", "17", "17" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestOOComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65701"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "0", "0" });
            expectedData.AddRange(new List<string> { "Average", "569", "565" });
            expectedData.AddRange(new List<string> { "High", "17", "17" });
            expectedData.AddRange(new List<string> { "Very High", "0", "0" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestSQLComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65801"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "522", "509" });
            expectedData.AddRange(new List<string> { "Average", "1", "1" });
            expectedData.AddRange(new List<string> { "High", "12", "12" });
            expectedData.AddRange(new List<string> { "Very High", "0", "0" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestCouplingComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65350"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "9413", "9345" });
            expectedData.AddRange(new List<string> { "Average", "666", "653" });
            expectedData.AddRange(new List<string> { "High", "170", "163" });
            expectedData.AddRange(new List<string> { "Very High", "42", "41" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestClassFanOutComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66020"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "411", "412" });
            expectedData.AddRange(new List<string> { "Average", "132", "130" });
            expectedData.AddRange(new List<string> { "High", "40", "37" });
            expectedData.AddRange(new List<string> { "Very High", "3", "3" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestClassFanInComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66021"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "527", "525" });
            expectedData.AddRange(new List<string> { "Average", "18", "17" });
            expectedData.AddRange(new List<string> { "High", "19", "19" });
            expectedData.AddRange(new List<string> { "Very High", "22", "21" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestSizeDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65105"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "8619", "8558" });
            expectedData.AddRange(new List<string> { "Average", "1315", "1298" });
            expectedData.AddRange(new List<string> { "High", "321", "310" });
            expectedData.AddRange(new List<string> { "Very High", "36", "36" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestReusebyCallDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66010"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "9413", "9345" });
            expectedData.AddRange(new List<string> { "Average", "666", "653" });
            expectedData.AddRange(new List<string> { "High", "170", "163" });
            expectedData.AddRange(new List<string> { "Very High", "42", "41" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestViolationsToCriticalDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67020"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "227", "226" });
            expectedData.AddRange(new List<string> { "Average", "308", "299" });
            expectedData.AddRange(new List<string> { "High", "107", "106" });
            expectedData.AddRange(new List<string> { "Very High", "59", "55" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestDefectsToCriticalDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67030"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            expectedData.AddRange(new List<string> { "Low", "221", "221" });
            expectedData.AddRange(new List<string> { "Average", "270", "262" });
            expectedData.AddRange(new List<string> { "High", "74", "73" });
            expectedData.AddRange(new List<string> { "Very High", "29", "26" });
            expectedData.AddRange(new List<string> { " ", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 3, 7);

        }
    }
}
