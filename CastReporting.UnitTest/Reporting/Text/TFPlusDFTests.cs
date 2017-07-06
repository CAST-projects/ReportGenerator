using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class TFPlusDFTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        public void TestCurrentContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.CurrencySymbol = "€";
            var component = new TFPlusDF();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("5,667", str);
        }

        [TestMethod]
        public void TestNoResults()
        {
            var component = new TFPlusDF();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(null, config);
            Assert.AreEqual("n/a", str);
        }

    }
}
