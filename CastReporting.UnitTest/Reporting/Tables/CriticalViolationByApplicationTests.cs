using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class CriticalViolationByApplicationTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);

            var component = new CriticalVIolationByApplication();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SHOW_PREVIOUS", "1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robu.", "Efcy", "Secu.", "Trans.", "Chang." });
            expectedData.AddRange(new List<string> { "Current Version", "75", "14", "43", "8", "6", "31" });
            expectedData.AddRange(new List<string> { "   Added", "0", "+5", "0", "0", "+3", "0" });
            expectedData.AddRange(new List<string> { "   Removed", "-22", "-2", "-2", "-1", "-1", "-19" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        public void TestTwoSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new CriticalVIolationByApplication();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SHOW_PREVIOUS", "1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robu.", "Efcy", "Secu.", "Trans.", "Chang." });
            expectedData.AddRange(new List<string> { "Current Version", "75", "14", "43", "8", "6", "31" });
            expectedData.AddRange(new List<string> { "   Added", "0", "+5", "0", "0", "+3", "0" });
            expectedData.AddRange(new List<string> { "   Removed", "-22", "-2", "-2", "-1", "-1", "-19" });
            expectedData.AddRange(new List<string> { "Previous Version", "97", "16", "45", "9", "7", "50" });
            TestUtility.AssertTableContent(table, expectedData, 7, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        public void TestTwoSnapshotNoShowPrevious()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new CriticalVIolationByApplication();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robu.", "Efcy", "Secu.", "Trans.", "Chang." });
            expectedData.AddRange(new List<string> { "Current Version", "75", "14", "43", "8", "6", "31" });
            expectedData.AddRange(new List<string> { "   Added", "0", "+5", "0", "0", "+3", "0" });
            expectedData.AddRange(new List<string> { "   Removed", "-22", "-2", "-2", "-1", "-1", "-19" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
