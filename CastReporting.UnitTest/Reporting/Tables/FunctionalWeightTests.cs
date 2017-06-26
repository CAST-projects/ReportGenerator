using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class FunctionalWeightTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
               null, null, null, null, null, null);

            var component = new FunctionalWeight();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Total" });
            expectedData.AddRange(new List<string> { "Automated Function Points", "5,667" });
            expectedData.AddRange(new List<string> { "Decision Points (total CC)", "11,964" });
            expectedData.AddRange(new List<string> { "Backfired Function Points", "418" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
