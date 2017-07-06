using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class PreviousSnapshotVersionNumberTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1496959200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);

            var component = new PreviousSnapshotVersionNumber();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("V-1.4.1", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNoResult()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);

            var component = new PreviousSnapshotVersionNumber();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

       
    }
}
