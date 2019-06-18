using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class ApplicationRuleTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestCurrentBC()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=60014,FORMAT=N2
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
             */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "CURRENT"},
                {"ID", "60014"},
                {"FORMAT", "N2"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("2.59", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestPreviousTC()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=PREVIOUS,ID=61001,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "PREVIOUS"},
                {"ID", "61001"},
                {"FORMAT", "N2"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("1.00", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestCurrentQR()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=7254,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "CURRENT"},
                {"ID", "7254"},
                {"FORMAT", "N2"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("3.71", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        public void TestCurrentSizing()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,SZID=10151,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "CURRENT"},
                {"SZID", "10151"},
                {"FORMAT", "N0"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("104,851", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        public void TestPreviousSizing()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=PREVIOUS,SZID=10154,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "PREVIOUS"},
                {"SZID", "10154"},
                {"FORMAT", "N0"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("485", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        public void TestNoPreviousSizing()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=PREVIOUS,SZID=10154,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "PREVIOUS"},
                {"SZID", "10154"},
                {"FORMAT", "N0"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        [DeploymentItem(@".\Data\BackFacts.json", "Data")]
        public void TestCurrentBackFact()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,BFID=66061,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            // Needed for background facts, as there are retrieved one by one by url request
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection,reportData.CurrentSnapshot);
            
            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "CURRENT"},
                {"BFID", "66061"},
                {"FORMAT", "N0"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("2", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestMillionId()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=60014,FORMAT=N2
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
             */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new ApplicationRule();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SNAPSHOT", "CURRENT"},
                {"ID", "11203569"},
                {"FORMAT", "N2"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("1.92", str);
        }

    }
}
