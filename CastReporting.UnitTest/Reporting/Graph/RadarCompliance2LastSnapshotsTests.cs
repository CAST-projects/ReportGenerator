using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class RadarCompliance2LastSnapshotsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new RadarCompliance2LastSnapshots();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2" });
            expectedData.AddRange(new List<string> { "Prog.", "2.93" });
            expectedData.AddRange(new List<string> { "Arch.", "2.1" });
            expectedData.AddRange(new List<string> { "Doc.", "2.65" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestTwoSnapshots()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new RadarCompliance2LastSnapshots();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { "Prog.", "2.93", "2.91" });
            expectedData.AddRange(new List<string> { "Arch.", "2.1", "2.1" });
            expectedData.AddRange(new List<string> { "Doc.", "2.65", "2.47" });
            TestUtility.AssertTableContent(table, expectedData, 3, 4);

        }
    }
}
