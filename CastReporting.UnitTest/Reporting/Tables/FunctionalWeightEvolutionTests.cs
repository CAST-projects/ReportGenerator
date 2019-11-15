using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class FunctionalWeightEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        [DeploymentItem(@".\Data\cocraFuncWeightPrevious.json", "Data")]
        public void TestTwoSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
               null, @".\Data\cocraFuncWeightPrevious.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", previousDate);

            var component = new FunctionalWeightEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "Automated Function Points", "5,667", "1,991", "+3,676", "+185 %" });
            expectedData.AddRange(new List<string> { "Decision Points (total CC)", "11,964", "6,814", "+5,150", "+75.6 %" });
            expectedData.AddRange(new List<string> { "Backfired Function Points", "418", "420", "-2", "-0.41 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
               null,null, null, null, null, null);

            var component = new FunctionalWeightEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "Automated Function Points", "5,667", Constants.No_Value, Constants.No_Value, Constants.No_Value });
            expectedData.AddRange(new List<string> { "Decision Points (total CC)", "11,964", Constants.No_Value, Constants.No_Value, Constants.No_Value });
            expectedData.AddRange(new List<string> { "Backfired Function Points", "418", Constants.No_Value, Constants.No_Value, Constants.No_Value });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
