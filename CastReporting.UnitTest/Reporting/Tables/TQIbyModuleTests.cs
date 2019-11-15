using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TQIbyModuleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new TQIbyModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Current TQI", "Previous TQI", "Variation" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.77", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.91", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.79", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTCmodules.json", "Data")]
        public void TestTwoSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            CastDate previousDate = new CastDate { Time = 1492380000000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               @".\Data\ModulesCoCRA.json", @".\Data\PreviousBCTCmodules.json", "AED/applications/3/snapshots/3", "Snap3_CAIP-8.2.4_RG-1.4.1", "8.2.4", previousDate);

            var component = new TQIbyModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Current TQI", "Previous TQI", "Variation" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.77", "2.76", "+0.64 %" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.91", "2.92", "-0.50 %" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.79", "2.65", "+5.29 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        public void TestShortHeader()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new TQIbyModule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"HEADER","SHORT" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Cur. TQI", "Prev. TQI", "Var" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.77", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.91", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.79", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
