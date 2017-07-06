using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class FailedChecksByRuleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }
        
        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        [DeploymentItem(@".\Data\RuleViolation1634.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new FailedChecksByRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"RULID", "1634" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("209", str);
        }
        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        [DeploymentItem(@".\Data\RuleViolation1634.json", "Data")]
        public void TestCurrentContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new FailedChecksByRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"RULID", "1634" },
                {"SNAPSHOT", "CURRENT" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("209", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        [DeploymentItem(@".\Data\cocraFuncWeightPrevious.json", "Data")]
        [DeploymentItem(@".\Data\RuleViolation1634.json", "Data")]
        [DeploymentItem(@".\Data\RuleViolation1634Previous.json", "Data")]
        public void TestPreviousContent()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/5", "Snap5_CAIP-8.3ra2_RG-1.6a", "8.3.ra2", currentDate,
                null, @".\Data\cocraFuncWeightPrevious.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new FailedChecksByRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"RULID", "1634" },
                {"SNAPSHOT", "PREVIOUS" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("26", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\cocraFuncWeight.json", "Data")]
        public void TestNoResult()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\cocraFuncWeight.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            if (snap != null) snap.SizingMeasuresResults = null;

            var component = new FailedChecksByRule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

       
    }
}
