using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class LocPerModuleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new LocByModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Module Name", "LoC"});
            expectedData.AddRange(new List<string> { "AAD-Admin", "9,684" });
            expectedData.AddRange(new List<string> { "AED-Admin", "28,297" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "24,446" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestKLocs()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);

            var component = new LocByModule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"FORMAT","KLOC" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Module Name", "kLoC" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "10" });
            expectedData.AddRange(new List<string> { "AED-Admin", "28" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "24" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
