using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class PortfolioNumberOfApplicationsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24Snap12Results.json", "Data")]
        public void TestContent()
        {
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3Snap4Results.json", @".\Data\AAD2App24Snap12Results.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[1].Snapshots.FirstOrDefault();
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;

            var component = new PortfolioNumberOfApplications();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("2", str);
        }


        [TestMethod]
        public void TestNoResult()
        {
            ReportData reportData = TestUtility.PrepaEmptyPortfolioReportData();

            var component = new PortfolioCriticalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

       
    }
}
