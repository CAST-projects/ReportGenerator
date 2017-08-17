using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class PortfolioGenericTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        /*
         * Configuration : TABLE;PF_GENERIC_TABLE;COL1=xx,COL11=xx,ROW1=xx,ROW11=xx,
         * 
         * xx = to choose between PERIODS, METRICS, APPLICATIONS, VIOLATIONS, CRITICAL_VIOLATIONS
         * if PERIODS, parameters PERIODS= should be added, with CURRENT|PREVIOUS|EVOL|EVOL_PERCENT, and PERIOD_DURATION=99 should be added with 99 corresponding to the number of months a period has (3 months by default)
         * if APPLICATIONS, parameter APPLICATIONS=ALL or application name should be added
         * if VIOLATIONS or CRITICAL_VIOLATIONS, a METRICS= should be added, if not, TQI will be taken of no METRICS in COL1, COL11, ROW1, ROW11, else health factor, also VIOLATIONS= or CRITICAL_VIOLATIONS= with TOTAL, ADDED, REMOVED or ALL (as for GENERIC_TABLE)
         * if METRICS, parameter METRICS= should be added, if not, health factor. METRICS can be grouped like for GENERIC_TABLE component. If no APPLICATIONS, a parameter AGGREGATORS should be added, containing the list of AGGREGATORS (must be AVERAGE or SUM) corresponding to the list of METRICS
         * for example, if METRICS=60017,68001,66024 then AGGREGATORS=AVERAGE,SUM,AVERAGE
         * for groups, you can precise METRICS=HEALTH_FACTOR,TECHNICAL_SIZING then AGGREGATORS=AVERAGE,SUM, but for groups, by default AVERAGE willbe taken for groups of quality indicators, and SUM for sizing measures or background facts
         * 
         */

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestMetricsBCOnePeriodOneApp()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-1) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            Debug.Assert(_snap0 != null, "_snap0 != null");
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            Debug.Assert(_snap1 != null, "_snap1 != null");
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "PERIODS"},
                {"METRICS", "60014|60017|60013"},
                {"AGGREGATORS", "AVERAGE" },
                {"PERIODS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Periods", "Efficiency", "Total Quality Index", "Robustness"});
            expectedData.AddRange(new List<string> {"Current Period", "1.84", "2.63", "3.10"});
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestMetricsBCEvolOneApp()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-2) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            Debug.Assert(_snap0 != null, "_snap0 != null");
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-15) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            Debug.Assert(_snap1 != null, "_snap1 != null");
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "PERIODS"},
                {"METRICS", "60014|60017|60013"},
                {"AGGREGATORS", "AVERAGE" },
                {"PERIODS", "ALL"},
                {"PERIOD_DURATION", "6" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Periods", "Efficiency", "Total Quality Index", "Robustness" });
            expectedData.AddRange(new List<string> { "Current Period", "1.84", "2.63", "3.10" });
            expectedData.AddRange(new List<string> { "Previous Period", "1.85", "2.62", "3.10" });
            expectedData.AddRange(new List<string> { "Evolution", "-0.01", "0.01", "0.00" });
            expectedData.AddRange(new List<string> { "% Evolution", "-0.54 %", "+0.38 %", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestMetricsBCEvolTwoApp()
        {
            // snap results : AAD/applications/37/results?snapshots=-2&metrics=60013,68001,10151,10202,67013&select=evolutionSummary
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-2) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            Debug.Assert(_snap0 != null, "_snap0 != null");
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-15) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            Debug.Assert(_snap1 != null, "_snap1 != null");
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots1 = new Snapshot[2];
            _snapshots1[0] = _snap0;
            _snapshots1[1] = _snap1;
            reportData.Applications[0].Snapshots = _snapshots1;

            Snapshot _snap2 = reportData.Applications[1].Snapshots.FirstOrDefault();
            TimeSpan time2 = DateTime.Now.AddMonths(-1) - date;
            CastDate _date2 = new CastDate { Time = time2.TotalMilliseconds };
            Debug.Assert(_snap2 != null, "_snap2 != null");
            _snap2.Annotation.Date = _date2;

            Snapshot _snap3 = reportData.Applications[1].Snapshots.ElementAt(1);
            TimeSpan time3 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date3 = new CastDate { Time = time3.TotalMilliseconds };
            Debug.Assert(_snap3 != null, "_snap3 != null");
            _snap3.Annotation.Date = _date3;

            Snapshot[] _snapshots2 = new Snapshot[2];
            _snapshots2[0] = _snap2;
            _snapshots2[1] = _snap3;
            reportData.Applications[1].Snapshots = _snapshots2;

            Snapshot[] _snapshots = new Snapshot[4];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            _snapshots[2] = _snap2;
            _snapshots[3] = _snap3;
            reportData.Snapshots = _snapshots;
            
            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "PERIODS"},
                {"METRICS", "60013|10151"},
                {"AGGREGATORS", "AVERAGE|SUM" },
                {"PERIODS", "ALL"},
                {"PERIOD_DURATION", "3" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Periods", "Robustness", "Number of Code Lines" });
            expectedData.AddRange(new List<string> { "Current Period", "3.50", "67,589" });
            expectedData.AddRange(new List<string> { "Previous Period", "3.35", "60,603" });
            expectedData.AddRange(new List<string> { "Evolution", "0.15", "6,986" });
            expectedData.AddRange(new List<string> { "% Evolution", "+4.48 %", "+11.5 %" });
            TestUtility.AssertTableContent(table, expectedData, 3, 5);
        }

    }
}
