using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TechnoLoCModuleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoCModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "", ".NET", "JEE", "SQL Analyzer" });
            expectedData.AddRange(new List<string> { "AAD-Admin", "n/a", "7,970", "1,714" });
            expectedData.AddRange(new List<string> { "AED-Admin", "n/a", "5,341", "22,956" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "24,446", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
