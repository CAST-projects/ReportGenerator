using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class HFByModuleTests
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

            var component = new HFbyModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "CoCRestAPI - 8.3.ra", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.77", "3.20", "1.83", "3.09", "2.89", "2.76" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.91", "3.41", "1.87", "3.24", "2.98", "2.89" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.79", "3.18", "2.66", "3.21", "2.89", "1.87" });
            TestUtility.AssertTableContent(table, expectedData, 7, 5);
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

            var component = new HFbyModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "CoCRestAPI - 8.3.ra", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.77", "3.20", "1.83", "3.09", "2.89", "2.76" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.91", "3.41", "1.87", "3.24", "2.98", "2.89" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.79", "3.18", "2.66", "3.21", "2.89", "1.87" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "CoCRestAPI - 8.2.4", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.76", "3.17", "1.82", "3.05", "2.87", "2.78" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.92", "3.40", "1.87", "3.24", "3.06", "2.89" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.65", "2.91", "2.69", "2.74", "2.95", "1.93" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Variation", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "+0.64 %", "+0.69 %", "+0.25 %", "+1.46 %", "+0.50 %", "-0.60 %" });
            expectedData.AddRange(new List<string> { "AED-Admin", "-0.50 %", "+0.26 %", "0 %", "+0.24 %", "-2.42 %", "+0.01 %" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "+5.29 %", "+9.15 %", "-1.08 %", "+17.1 %", "-1.73 %", "-2.88 %" });
            TestUtility.AssertTableContent(table, expectedData, 7, 15);
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

            var component = new HFbyModule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"HEADER","SHORT" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robu.", "Efcy", "Secu.", "Trans.", "Chang." });
            expectedData.AddRange(new List<string> { "CoCRestAPI - 8.3.ra", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "2.77", "3.20", "1.83", "3.09", "2.89", "2.76" });
            expectedData.AddRange(new List<string> { "AED-Admin", "2.91", "3.41", "1.87", "3.24", "2.98", "2.89" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.79", "3.18", "2.66", "3.21", "2.89", "1.87" });
            TestUtility.AssertTableContent(table, expectedData, 7, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
