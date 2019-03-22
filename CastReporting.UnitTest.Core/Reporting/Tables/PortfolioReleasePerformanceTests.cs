using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class PortfolioReleasePerformanceTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestExample()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score" ,"Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "3.10","2.90","3.10","Good" });
            expectedData.AddRange(new List<string> { "Security", "2.93", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Transferability", "2.83", "2.90", "2.82", "Acceptable" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.64", "2.90", "2.65", "Poor" });
            expectedData.AddRange(new List<string> { "Documentation", "2.38", "2.90", "2.38", "Poor" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.45", "2.90", "2.51", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestSLA()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","5 20"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "3.10", "2.90", "3.10", "Good" });
            expectedData.AddRange(new List<string> { "Security", "2.93", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "2.90", "2.37", "Acceptable" });
            expectedData.AddRange(new List<string> { "Transferability", "2.83", "2.90", "2.82", "Good" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.64", "2.90", "2.65", "Acceptable" });
            expectedData.AddRange(new List<string> { "Documentation", "2.38", "2.90", "2.38", "Acceptable" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.45", "2.90", "2.51", "Acceptable" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestTargets()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","3.20 3.50 1.80 1.80 3.00 2.70 2.30 2.60"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "3.10", "3.20", "3.10", "Acceptable" });
            expectedData.AddRange(new List<string> { "Security", "2.93", "3.50", "2.97", "Poor" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "1.80", "1.84", "Good" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "1.80", "2.37", "Good" });
            expectedData.AddRange(new List<string> { "Transferability", "2.83", "3.00", "2.82", "Poor" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.64", "2.70", "2.65", "Good" });
            expectedData.AddRange(new List<string> { "Documentation", "2.38", "2.30", "2.38", "Good" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.45", "2.60", "2.51", "Acceptable" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestNoPrevious()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot[] _snapshots = new Snapshot[1];
            _snapshots[0] = _snap0;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "-", "2.90", "3.10", "Good" });
            expectedData.AddRange(new List<string> { "Security", "-", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "-", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "-", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Transferability", "-", "2.90", "2.82", "Acceptable" });
            expectedData.AddRange(new List<string> { "Programming Practices", "-", "2.90", "2.65", "Poor" });
            expectedData.AddRange(new List<string> { "Documentation", "-", "2.90", "2.38", "Poor" });
            expectedData.AddRange(new List<string> { "Architectural Design", "-", "2.90", "2.51", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestNoCurrent()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot[] _snapshots = new Snapshot[1];
            _snapshots[0] = _snap0;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;
            
            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "3.10", "2.90", "3.10", "Good" });
            expectedData.AddRange(new List<string> { "Security", "2.97", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.84", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.37", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Transferability", "2.82", "2.90", "2.82", "Acceptable" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.65", "2.90", "2.65", "Poor" });
            expectedData.AddRange(new List<string> { "Documentation", "2.38", "2.90", "2.38", "Poor" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.51", "2.90", "2.51", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestVeryOldCurrent()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot[] _snapshots = new Snapshot[1];
            _snapshots[0] = _snap0;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "3.10", "2.90", "3.10", "Good" });
            expectedData.AddRange(new List<string> { "Security", "2.97", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.84", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.37", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Transferability", "2.82", "2.90", "2.82", "Acceptable" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.65", "2.90", "2.65", "Poor" });
            expectedData.AddRange(new List<string> { "Documentation", "2.38", "2.90", "2.38", "Poor" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.51", "2.90", "2.51", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestOldPrevious()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "3.10", "2.90", "3.10", "Good" });
            expectedData.AddRange(new List<string> { "Security", "2.93", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Transferability", "2.83", "2.90", "2.82", "Acceptable" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.64", "2.90", "2.65", "Poor" });
            expectedData.AddRange(new List<string> { "Documentation", "2.38", "2.90", "2.38", "Poor" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.45", "2.90", "2.51", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void Test2Current()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddHours(-12) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot _snap2 = reportData.Applications[0].Snapshots.ElementAt(2);
            TimeSpan time2 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date2 = new CastDate { Time = time2.TotalMilliseconds };
            if (_snap2 == null) Assert.Fail();
            _snap2.Annotation.Date = _date2;

            Snapshot[] _snapshots = new Snapshot[3];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            _snapshots[2] = _snap2;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BF","2.90 2.90 2.90 2.90 2.90 2.90 2.90 2.90"},
                {"SLA","2 5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Business criterion name", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Robustness", "2.95", "2.90", "3.10", "Good" });
            expectedData.AddRange(new List<string> { "Security", "2.70", "2.90", "2.97", "Good" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.37", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Transferability", "2.86", "2.90", "2.82", "Acceptable" });
            expectedData.AddRange(new List<string> { "Programming Practices", "2.57", "2.90", "2.65", "Poor" });
            expectedData.AddRange(new List<string> { "Documentation", "2.46", "2.90", "2.38", "Poor" });
            expectedData.AddRange(new List<string> { "Architectural Design", "2.51", "2.90", "2.51", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 9);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
