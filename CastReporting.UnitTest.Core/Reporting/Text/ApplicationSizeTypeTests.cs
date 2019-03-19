using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class ApplicationSizeTypeTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestVerySmallContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationSizeLimitSupLarge = 3000000;
            reportData.Parameter.ApplicationSizeLimitSupMedium = 500000;
            reportData.Parameter.ApplicationSizeLimitSupSmall = 10000;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == 10151);
            if (res != null) res.DetailResult.Value = 5000;

            var component = new ApplicationSizeType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("small", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestMediumContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationSizeLimitSupLarge = 3000000;
            reportData.Parameter.ApplicationSizeLimitSupMedium = 500000;
            reportData.Parameter.ApplicationSizeLimitSupSmall = 10000;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == 10151);
            if (res != null) res.DetailResult.Value = 15000;

            var component = new ApplicationSizeType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("medium", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestLargeContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationSizeLimitSupLarge = 3000000;
            reportData.Parameter.ApplicationSizeLimitSupMedium = 500000;
            reportData.Parameter.ApplicationSizeLimitSupSmall = 10000;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == 10151);
            if (res != null) res.DetailResult.Value = 1000000;

            var component = new ApplicationSizeType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("large", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestExtraLargeContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.Parameter.ApplicationSizeLimitSupLarge = 3000000;
            reportData.Parameter.ApplicationSizeLimitSupMedium = 500000;
            reportData.Parameter.ApplicationSizeLimitSupSmall = 10000;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == 10151);
            if (res != null) res.DetailResult.Value = 7000000;

            var component = new ApplicationSizeType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("extra large", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNoResult()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            reportData.Parameter.ApplicationSizeLimitSupLarge = 3000000;
            reportData.Parameter.ApplicationSizeLimitSupMedium = 500000;
            reportData.Parameter.ApplicationSizeLimitSupSmall = 10000;

            Snapshot snap = reportData.Application.Snapshots.FirstOrDefault();
            ApplicationResult res = snap?.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == 10151);
            if (res != null) res.DetailResult = null;

            var component = new ApplicationSizeType();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

       
    }
}
