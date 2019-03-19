using System.Collections.Generic;
using CastReporting.Reporting.Block.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class BubbleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Metrics2.json", "Data")]
        public void TestContentSnapshot()
        {
            /*
             * Configuration : TABLE;GENERIC_GRAPH;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=TECHNICAL_DEBT,SNAPSHOTS=CURRENT
            * DreamTeamSnap4Metrics2.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,60017,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
            */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Metrics2.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new Bubble();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TQI", "Technical Debt ($)", "Size (kLoC)" });
            expectedData.AddRange(new List<string> { "2.1", "1503963.75", "104851" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);
            
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1_Extended.json", "Data")]
        public void TestContentSnapshotModule()
        {
            /*
             * Configuration : TABLE;GENERIC_GRAPH;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=TECHNICAL_DEBT,SNAPSHOTS=CURRENT
            * Snapshot_QIresults1_Extended.json : extension of Snapshot_QIresults1.json with missing values for this component
            */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\Snapshot_QIresults1_Extended.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new Bubble();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"M", "5"}
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TQI", "Technical Debt ($)", "Size (kLoC)" });
            expectedData.AddRange(new List<string> { "2.68", "269396.75", "12345" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);

        }
    }
}
