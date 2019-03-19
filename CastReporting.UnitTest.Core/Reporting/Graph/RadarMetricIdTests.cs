using System.Collections.Generic;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class RadarMetricIdTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        public void TestCurrentSnapshot()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=60017,SNAPSHOT=CURRENT
             * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60017"},
                {"SNAPSHOT", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2" });
            expectedData.AddRange(new List<string> { "TQI", "2.7781377635159" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestPreviousSnapshot()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=60017,SNAPSHOT=PREVIOUS
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
            */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60017"},
                {"SNAPSHOT", "PREVIOUS"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.4.1" });
            expectedData.AddRange(new List<string> { "TQI", "2.60905438431695" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestBothSnapshots()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=60017,SNAPSHOT=PREVIOUS
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
            */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60017"},
                {"SNAPSHOT", "BOTH"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { "TQI", "2.7781377635159", "2.60905438431695" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestNoSnapshot()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=60017,SNAPSHOT=PREVIOUS
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
            */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60017"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "V-1.5.0_Sprint 2_2", "V-1.4.1" });
            expectedData.AddRange(new List<string> { "TQI", "2.7781377635159", "2.60905438431695" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestBusinessCriteria()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=60011,SNAPSHOT=CURRENT
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60011"},
                {"SNAPSHOT", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "v1.1.4" });
            expectedData.AddRange(new List<string> { "Trsf", "3.11741064909927" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestTechnicalCriteria()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=61007,SNAPSHOT=CURRENT
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "61007"},
                {"SNAPSHOT", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "v1.1.4" });
            expectedData.AddRange(new List<string> { "Documentation - Bad Comments", "3.65239043824701" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestQualityRule()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=4656,SNAPSHOT=CURRENT
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "4656"},
                {"SNAPSHOT", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "v1.1.4" });
            expectedData.AddRange(new List<string> { "Avoid declaring throwing an exception and not throwing it", "3.03308487982963" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Metrics.json", "Data")]
        public void TestSizingMeasure()
        {
            /*
             * Configuration : TABLE;RADAR_METRIC_ID;ID=10151,SNAPSHOT=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);
            var component = new RadarMetricId();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "10151"},
                {"SNAPSHOT", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { null, "4" });
            expectedData.AddRange(new List<string> { "LOCs", "104851" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

    }
}
