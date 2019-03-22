using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cast.Util.Date;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class PortfolioCritViolPerformanceTests
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
        public void TestContentTQI()
        {
            /*
             * AAD2App3Snap4Results : AAD2/applications/3/snapshots/4/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)
             * AAD2App24Snap12Results : AAD2/applications/24/snapshots/12/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)
             */
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

            var component = new PortfolioCritViolPerformance();
            var table = component.Content(reportData, null);

            int generateQuater = 6;
            string[] quarters = new string[generateQuater];
            DateTime _dateNow = DateTime.Now;
            int currentYear = _dateNow.Year;
            int currentQuarter = DateUtil.GetQuarter(_dateNow);
            for (int i = generateQuater; i > 0; i--)
            {
                quarters[i - 1] = currentYear + " Q" + currentQuarter;
                currentYear = DateUtil.GetPreviousQuarterYear(currentQuarter, currentYear);
                currentQuarter = DateUtil.GetPreviousQuarter(currentQuarter);
            }

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Critical Violations - Removed", "Critical Violations - Added", "Critical Violations - Total" });
            expectedData.AddRange(new List<string> { quarters[0], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[1], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[2], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[3], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[4], "-10", "11", "616" });
            expectedData.AddRange(new List<string> { quarters[5], "-42", "79", "1560" });

            TestUtility.AssertTableContent(table, expectedData, 4, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24Snap12Results.json", "Data")]
        public void TestContentEfficiency()
        {
            /*
             * AAD2App3Snap4Results : AAD2/applications/3/snapshots/4/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)
             * AAD2App24Snap12Results : AAD2/applications/24/snapshots/12/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)
             */
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

            var component = new PortfolioCritViolPerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID", "60014"}
            };
            var table = component.Content(reportData, config);

            int generateQuater = 6;
            string[] quarters = new string[generateQuater];
            DateTime _dateNow = DateTime.Now;
            int currentYear = _dateNow.Year;
            int currentQuarter = DateUtil.GetQuarter(_dateNow);
            for (int i = generateQuater; i > 0; i--)
            {
                quarters[i - 1] = currentYear + " Q" + currentQuarter;
                currentYear = DateUtil.GetPreviousQuarterYear(currentQuarter, currentYear);
                currentQuarter = DateUtil.GetPreviousQuarter(currentQuarter);
            }

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Critical Violations - Removed", "Critical Violations - Added", "Critical Violations - Total" });
            expectedData.AddRange(new List<string> { quarters[0], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[1], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[2], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[3], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[4], "-4", "1", "151" });
            expectedData.AddRange(new List<string> { quarters[5], "-38", "42", "198" });

            TestUtility.AssertTableContent(table, expectedData, 4, 7);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24Snap12Results.json", "Data")]
        public void TestContentSameQuarter()
        {
            /*
             * AAD2App3Snap4Results : AAD2/applications/3/snapshots/4/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)
             * AAD2App24Snap12Results : AAD2/applications/24/snapshots/12/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)
             */
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
            TimeSpan time1 = DateTime.Now - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;

            var component = new PortfolioCritViolPerformance();
            var table = component.Content(reportData, null);

            int generateQuater = 6;
            string[] quarters = new string[generateQuater];
            DateTime _dateNow = DateTime.Now;
            int currentYear = _dateNow.Year;
            int currentQuarter = DateUtil.GetQuarter(_dateNow);
            for (int i = generateQuater; i > 0; i--)
            {
                quarters[i - 1] = currentYear + " Q" + currentQuarter;
                currentYear = DateUtil.GetPreviousQuarterYear(currentQuarter, currentYear);
                currentQuarter = DateUtil.GetPreviousQuarter(currentQuarter);
            }

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Critical Violations - Removed", "Critical Violations - Added", "Critical Violations - Total" });
            expectedData.AddRange(new List<string> { quarters[0], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[1], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[2], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[3], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[4], "0", "0", "0" });
            expectedData.AddRange(new List<string> { quarters[5], "-52", "90", "2176" });

            TestUtility.AssertTableContent(table, expectedData, 4, 7);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3AllSnap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3AllSnapResults.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24AllSnap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24AllSnapResults.json", "Data")]
        public void TestContentAllQuarters()
        {
            /*
             * AAD2App3AllSnap : AAD2/applications/3/snapshots
             * AAD2App3AllSnapResults : AAD2/applications/3/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)&snapshots=($all)
             * AAD2App24AllSnap : AAD2/applications/24/snapshots
             * AAD2App24AllSnapResults : AAD2/applications/24/results?quality-indicators=(60017,60014)&sizing-measures=(10151,67013,10202,68901,68902,68001)&select=(evolutionSummary,violationRatio)&snapshots=($all)
             */
            List<string> snapList = new List<string> { @".\Data\AAD2App3AllSnap.json", @".\Data\AAD2App24AllSnap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3AllSnapResults.json", @".\Data\AAD2App24AllSnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.ElementAt(0);
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            Debug.Assert(_snap0 != null, "_snap0 != null");
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[1].Snapshots.ElementAt(0);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            Debug.Assert(_snap1 != null, "_snap1 != null");
            _snap1.Annotation.Date = _date1;

            Snapshot _snap2 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time2 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date2 = new CastDate { Time = time2.TotalMilliseconds };
            Debug.Assert(_snap2 != null, "_snap2 != null");
            _snap2.Annotation.Date = _date2;

            Snapshot _snap3 = reportData.Applications[1].Snapshots.ElementAt(1);
            TimeSpan time3 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date3 = new CastDate { Time = time3.TotalMilliseconds };
            Debug.Assert(_snap3 != null, "_snap3 != null");
            _snap3.Annotation.Date = _date3;

            Snapshot _snap4 = reportData.Applications[0].Snapshots.ElementAt(2);
            TimeSpan time4 = DateTime.Now.AddMonths(-9) - date;
            CastDate _date4 = new CastDate { Time = time4.TotalMilliseconds };
            Debug.Assert(_snap4 != null, "_snap4 != null");
            _snap4.Annotation.Date = _date4;

            Snapshot _snap5 = reportData.Applications[1].Snapshots.ElementAt(2);
            TimeSpan time5 = DateTime.Now.AddMonths(-12) - date;
            CastDate _date5 = new CastDate { Time = time5.TotalMilliseconds };
            Debug.Assert(_snap5 != null, "_snap5 != null");
            _snap5.Annotation.Date = _date5;

            Snapshot _snap6 = reportData.Applications[1].Snapshots.ElementAt(3);
            TimeSpan time6 = DateTime.Now.AddMonths(-15) - date;
            CastDate _date6 = new CastDate { Time = time6.TotalMilliseconds };
            Debug.Assert(_snap6 != null, "_snap6 != null");
            _snap6.Annotation.Date = _date6;

            // Should not appear in graph, out of time scope
            Snapshot _snap7 = reportData.Applications[0].Snapshots.ElementAt(3);
            TimeSpan time7 = DateTime.Now.AddMonths(-19) - date;
            CastDate _date7 = new CastDate { Time = time7.TotalMilliseconds };
            Debug.Assert(_snap7 != null, "_snap7 != null");
            _snap7.Annotation.Date = _date7;

            Snapshot[] _snapshots = new Snapshot[8];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            _snapshots[2] = _snap2;
            _snapshots[3] = _snap3;
            _snapshots[4] = _snap4;
            _snapshots[5] = _snap5;
            _snapshots[6] = _snap6;
            _snapshots[7] = _snap7;

            reportData.Snapshots = _snapshots;

            var component = new PortfolioCritViolPerformance();
            var table = component.Content(reportData, null);

            int generateQuater = 6;
            string[] quarters = new string[generateQuater];
            DateTime _dateNow = DateTime.Now;
            int currentYear = _dateNow.Year;
            int currentQuarter = DateUtil.GetQuarter(_dateNow);
            for (int i = generateQuater; i > 0; i--)
            {
                quarters[i - 1] = currentYear + " Q" + currentQuarter;
                currentYear = DateUtil.GetPreviousQuarterYear(currentQuarter, currentYear);
                currentQuarter = DateUtil.GetPreviousQuarter(currentQuarter);
            }

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Critical Violations - Removed", "Critical Violations - Added", "Critical Violations - Total" });
            expectedData.AddRange(new List<string> { quarters[0], "0", "122", "122" });
            expectedData.AddRange(new List<string> { quarters[1], "0", "20", "142" });
            expectedData.AddRange(new List<string> { quarters[2], "-14", "18", "1464" });
            expectedData.AddRange(new List<string> { quarters[3], "-11", "484", "615" });
            expectedData.AddRange(new List<string> { quarters[4], "-58", "118", "2139" });
            expectedData.AddRange(new List<string> { quarters[5], "-42", "79", "1560" });

            TestUtility.AssertTableContent(table, expectedData, 4, 7);
        }
    }
}
