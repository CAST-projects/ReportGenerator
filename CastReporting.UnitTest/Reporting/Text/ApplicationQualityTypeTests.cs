using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class ApplicationQualityTypeTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestVeryLowContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationQualityGood = 3.7;
            reportData.Parameter.ApplicationQualityLow = 2.3;
            reportData.Parameter.ApplicationQualityMedium = 3.1;
            reportData.Parameter.ApplicationQualityVeryLow = 1.5;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == 60017);
            if (res != null) res.DetailResult.Grade = 1.00;

            var component = new ApplicationQualityType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("very low", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestLowContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationQualityGood = 3.7;
            reportData.Parameter.ApplicationQualityLow = 2.3;
            reportData.Parameter.ApplicationQualityMedium = 3.1;
            reportData.Parameter.ApplicationQualityVeryLow = 1.5;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == 60017);
            if (res != null) res.DetailResult.Grade = 2.00;

            var component = new ApplicationQualityType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("low", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestMediumContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationQualityGood = 3.7;
            reportData.Parameter.ApplicationQualityLow = 2.3;
            reportData.Parameter.ApplicationQualityMedium = 3.1;
            reportData.Parameter.ApplicationQualityVeryLow = 1.5;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == 60017);
            if (res != null) res.DetailResult.Grade = 2.70;

            var component = new ApplicationQualityType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("medium", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestGoodContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationQualityGood = 3.7;
            reportData.Parameter.ApplicationQualityLow = 2.3;
            reportData.Parameter.ApplicationQualityMedium = 3.1;
            reportData.Parameter.ApplicationQualityVeryLow = 1.5;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == 60017);
            if (res != null) res.DetailResult.Grade = 3.35;

            var component = new ApplicationQualityType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("good", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestVeryGoodContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationQualityGood = 3.7;
            reportData.Parameter.ApplicationQualityLow = 2.3;
            reportData.Parameter.ApplicationQualityMedium = 3.1;
            reportData.Parameter.ApplicationQualityVeryLow = 1.5;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == 60017);
            if (res != null) res.DetailResult.Grade = 4.00;

            var component = new ApplicationQualityType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("very good", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNoResult()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationQualityGood = 3.7;
            reportData.Parameter.ApplicationQualityLow = 2.3;
            reportData.Parameter.ApplicationQualityMedium = 3.1;
            reportData.Parameter.ApplicationQualityVeryLow = 1.5;
            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            if (snap != null) snap.BusinessCriteriaResults = null;
            var component = new ApplicationQualityType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

       
    }
}
