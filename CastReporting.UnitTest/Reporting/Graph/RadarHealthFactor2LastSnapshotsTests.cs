using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class RadarHealthFactor2LastSnapshotsTests
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
            var component = new RadarHealthFactor2LastSnapshots();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2" });
            expectedData.AddRange(new List<string> { "Trans.", "2.92" });
            expectedData.AddRange(new List<string> { "Chang.", "1.93" });
            expectedData.AddRange(new List<string> { "Robu.", "3.19" });
            expectedData.AddRange(new List<string> { "Efcy", "2.59" });
            expectedData.AddRange(new List<string> { "Secu.", "3.17" });
            TestUtility.AssertTableContent(table, expectedData, 2, 6);

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

            var component = new RadarHealthFactor2LastSnapshots();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { "Trans.", "2.92", "2.82" });
            expectedData.AddRange(new List<string> { "Chang.", "1.93", "1.93" });
            expectedData.AddRange(new List<string> { "Robu.", "3.19", "3.15" });
            expectedData.AddRange(new List<string> { "Efcy", "2.59", "2.6" });
            expectedData.AddRange(new List<string> { "Secu.", "3.17", "3.13" });
            TestUtility.AssertTableContent(table, expectedData, 3, 6);

        }
    }
}
