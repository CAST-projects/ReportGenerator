
using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class TrendMetricIdTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        [DeploymentItem(@".\Data\BusinessValue.json", "Data")]
        [DeploymentItem(@".\Data\BackFacts.json", "Data")]
        public void TestContent()
        {
            /*
             * Configuration : TABLE;TREND_METRIC_ID;QID=60017|61007|4556,SID=10151,BID=66061
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            CastDate currentDate = new CastDate {Time = 1468360800000};
            CastDate previousDate = new CastDate { Time = 1463090400000 };

            ReportData reportData = TestUtility.PrepareApplicationReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4", currentDate,
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3", previousDate);

            var component = new TrendMetricId();

            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"QID", "60017|61007|4656"},
                {"SID", "10151"},
                {"BID", "66061"}
            };
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Total Quality Index", "Documentation - Bad Comments", "Avoid declaring throwing an exception and not throwing it", "Business Value" });
            expectedData.AddRange(new List<string> { "42503", "1.94", "1.94", "1.94", "2" });
            expectedData.AddRange(new List<string> { "42564", "1.94", "1.94", "1.94", "2" });
            TestUtility.AssertTableContent(table, expectedData, 5, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        public void TestSizingMeasure()
        {
            /*
             * Configuration : TABLE;TREND_METRIC_ID;SID=10151|
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */
            CastDate currentDate = new CastDate { Time = 1468360800000 };
            CastDate previousDate = new CastDate { Time = 1463090400000 };

            ReportData reportData = TestUtility.PrepareApplicationReportData("AppliAEP",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4", currentDate,
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1", previousDate);

            var component = new TrendMetricId();

            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SID", "10151|10107|10152|67210|67011"}
            };

            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Number of Code Lines", "Number of Comment Lines", "Number of Artifacts" });
            expectedData.AddRange(new List<string> { "42503", "10626", "10626", "10626" });
            expectedData.AddRange(new List<string> { "42564", "10626", "10626", "10626" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

        }
    }
}
