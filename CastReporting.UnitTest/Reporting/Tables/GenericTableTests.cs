using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class GenericTableTests
    {
        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        public void TestSample1()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=60014|60017|60013,SNAPSHOTS=CURRENT
             * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60014|60017|60013"},
                {"SNAPSHOTS", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Snapshots", "Efficiency", "Total Quality Index", "Robustness"});
            expectedData.AddRange(new List<string> {"PreVersion 1.5.0 sprint 2 shot 2 - V-1.5.0_Sprint 2_2", "2.59", "2.78", "3.19"});
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestSample2()
        {
            /*
            * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=60014|60017|60013,SNAPSHOTS=CURRENT|PREVIOUS
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
            */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60014|60017|60013"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Snapshots", "Efficiency", "Total Quality Index", "Robustness"});
            expectedData.AddRange(new List<string> {"PreVersion 1.5.0 sprint 2 shot 2 - V-1.5.0_Sprint 2_2", "2.59", "2.78", "3.19"});
            expectedData.AddRange(new List<string> {"PreVersion 1.4.1 before release - V-1.4.1", "2.61", "2.61", "2.88"});
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestSample3()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Snapshots", "Transferability", "Changeability", "Robustness", "Efficiency", "Security"});
            expectedData.AddRange(new List<string> {"Snap_v1.1.4 - v1.1.4", "3.12", "2.98", "2.55", "1.88", "1.70"});
            expectedData.AddRange(new List<string> {"Snap_v1.1.3 - v1.1.3", "3.13", "2.98", "2.55", "1.88", "1.70"});
            TestUtility.AssertTableContent(table, expectedData, 6, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Modules1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestSample4()
        {
            /*
             * Configuration = TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,ROW11=MODULES,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @".\Data\Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"ROW11", "MODULES"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    SHOPIZER/AppliAEPtran/Shopizer_sql content", "3.36", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "    sm-central/AppliAEPtran/Shopizer_src content", "3.00", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "    sm-core/AppliAEPtran/Shopizer_src content", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "    sm-shop/AppliAEPtran/Shopizer_src content", "2.96", "3.04", "2.51", "1.84", "1.76" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    SHOPIZER/AppliAEPtran/Shopizer_sql content", "3.37", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "    sm-central/AppliAEPtran/Shopizer_src content", "3.01", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "    sm-core/AppliAEPtran/Shopizer_src content", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "    sm-shop/AppliAEPtran/Shopizer_src content", "2.96", "3.03", "2.51", "1.84", "1.76" });
            TestUtility.AssertTableContent(table, expectedData, 6, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Modules1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestSample5()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=MODULES,ROW11=SNAPSHOTS,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @".\Data\Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "MODULES"},
                {"ROW11", "SNAPSHOTS"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "SHOPIZER/AppliAEPtran/Shopizer_sql content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "3.36", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "3.37", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "sm-central/AppliAEPtran/Shopizer_src content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "3.00", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "3.01", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "sm-core/AppliAEPtran/Shopizer_src content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "sm-shop/AppliAEPtran/Shopizer_src content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "2.96", "3.04", "2.51", "1.84", "1.76" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "2.96", "3.03", "2.51", "1.84", "1.76" });
            TestUtility.AssertTableContent(table, expectedData, 6, 13);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Modules1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestSample6()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,COL11=MODULES,ROW1=SNAPSHOTS,METRICS=60017|60014,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @".\Data\Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"COL11", "MODULES"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots",
                "Total Quality Index - SHOPIZER/AppliAEPtran/Shopizer_sql content",
                "Total Quality Index - sm-central/AppliAEPtran/Shopizer_src content",
                "Total Quality Index - sm-core/AppliAEPtran/Shopizer_src content",
                "Total Quality Index - sm-shop/AppliAEPtran/Shopizer_src content",
                "Efficiency - SHOPIZER/AppliAEPtran/Shopizer_sql content",
                "Efficiency - sm-central/AppliAEPtran/Shopizer_src content",
                "Efficiency - sm-core/AppliAEPtran/Shopizer_src content",
                "Efficiency - sm-shop/AppliAEPtran/Shopizer_src content"});
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4","3.47","2.53", "2.68", "2.49","3.26", "1.84", "1.88", "1.84"});
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", "3.47", "2.53", "2.68", "2.49", "3.26", "1.84", "1.88", "1.84" });
            TestUtility.AssertTableContent(table, expectedData, 9, 3);
            
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample7.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample7.json", "Data")]
        public void TestSample7()
        {
            /*
             * Configuration :TABLE;GENERIC_TABLE;COL1=MODULES,COL11=METRICS,ROW1=SNAPSHOTS,METRICS=60017|60014,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * ModulesDreamTeam.json : AED3/applications/7/modules
             * DreamTeamSnap4Sample7.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             * DreamTeamSnap1Sample7.json : AED3/applications/7/snapshots/3/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
               @".\Data\ModulesDreamTeam.json", @".\Data\DreamTeamSnap4Sample7.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
               @".\Data\ModulesDreamTeam.json", @".\Data\DreamTeamSnap1Sample7.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "MODULES"},
                {"COL11", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots",
                "Adg - Total Quality Index", "Adg - Efficiency",
                "Central - Total Quality Index", "Central - Efficiency",
                "DssAdmin - Total Quality Index", "DssAdmin - Efficiency",
                "Pchit - Total Quality Index", "Pchit - Efficiency"});
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_4 - 4", "2.35", "2.64", "2.40", "1.71", "3.08", "3.31", "2.62", "2.14" });
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_1 - 1", "2.43", "2.64", "2.40", "1.71", "3.08", "3.32", "2.63", "2.09" });
            TestUtility.AssertTableContent(table, expectedData, 9, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample7.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample7.json", "Data")]
        public void TestSample8()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=MODULES,COL11=METRICS,ROW1=SNAPSHOTS,ROW11=TECHNOLOGIES,
             * METRICS=60017|60014,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL,TECHNOLOGIES=ALL
             * ModulesDreamTeam.json : AED3/applications/7/modules
             * DreamTeamSnap4Sample7.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             * DreamTeamSnap1Sample7.json : AED3/applications/7/snapshots/3/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
               @".\Data\ModulesDreamTeam.json", @".\Data\DreamTeamSnap4Sample7.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
               @".\Data\ModulesDreamTeam.json", @".\Data\DreamTeamSnap1Sample7.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] {"JEE", "PL/SQL", "C++", ".NET"};
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "MODULES"},
                {"COL11", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"ROW11", "TECHNOLOGIES"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"},
                {"TECHNOLOGIES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots",
                "Adg - Total Quality Index", "Adg - Efficiency",
                "Central - Total Quality Index", "Central - Efficiency",
                "DssAdmin - Total Quality Index", "DssAdmin - Efficiency",
                "Pchit - Total Quality Index", "Pchit - Efficiency"});
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_4 - 4", " ", " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "2.35", "2.64", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "n/a", "n/a", "2.40", "1.71", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    C++", "n/a", "n/a", "n/a", "n/a", "3.08", "3.31", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    .NET", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "2.62", "2.14" });
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_1 - 1", " ", " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "2.43", "2.64", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "n/a", "n/a", "2.40", "1.71", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    C++", "n/a", "n/a", "n/a", "n/a", "3.08", "3.32", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    .NET", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "2.63", "2.09" });
            TestUtility.AssertTableContent(table, expectedData, 9, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample9.json", "Data")]
        public void TestSample9()
        {
            /*
             * Configuration :TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=CRITICAL_VIOLATIONS,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Sample9.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
              null, @".\Data\DreamTeamSnap4Sample9.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
              null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "CRITICAL_VIOLATIONS"},
                {"SNAPSHOTS", "CURRENT"}
            };
            TestUtility.SetCulture("fr-FR");
            var tableFr = component.Content(reportData, config);
            var expectedDataFr = new List<string>();
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques", "23", "160", "187", "206", "200" });
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques ajoutées", "0", "6", "11", "20", "22" });
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques supprimées", "0", "0", "0", "1", "0" });
            TestUtility.AssertTableContent(tableFr, expectedDataFr, 6, 4);

            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Critical Violations", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Total Critical Violations", "23", "160", "187", "206", "200" });
            expectedData.AddRange(new List<string> { "Added Critical Violations", "0", "6", "11", "20", "22" });
            expectedData.AddRange(new List<string> { "Removed Critical Violations", "0", "0", "0", "1", "0" });
            TestUtility.AssertTableContent(table, expectedData, 6, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample10.json", "Data")]
        public void TestSample10()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=CRITICAL_VIOLATIONS,ROW11=MODULES,
             * METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS=ALL,MODULES=ALL,SNAPSHOTS=CURRENT
             * ModulesDreamTeam.json : AED3/applications/7/modules
             * DreamTeamSnap4Sample10.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)&modules=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
             @".\Data\ModulesDreamTeam.json", @".\Data\DreamTeamSnap4Sample10.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
             null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "VIOLATIONS"},
                {"ROW11", "MODULES"},
                {"METRICS", "HEALTH_FACTOR" },
                {"VIOLATIONS", "ALL" },
                {"MODULES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violations", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Total Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Adg", "20,765", "8,907", "4,018", "754", "1,394" });
            expectedData.AddRange(new List<string> { "    Central", "930", "854", "569", "559", "129" });
            expectedData.AddRange(new List<string> { "    DssAdmin", "3,377", "3,856", "1,251", "80", "851" });
            expectedData.AddRange(new List<string> { "    Pchit", "256", "247", "132", "58", "78" });
            expectedData.AddRange(new List<string> { "Added Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Adg", "915", "534", "306", "94", "144" });
            expectedData.AddRange(new List<string> { "    Central", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    DssAdmin", "14", "12", "6", "0", "2" });
            expectedData.AddRange(new List<string> { "    Pchit", "111", "104", "66", "30", "35" });
            expectedData.AddRange(new List<string> { "Removed Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Adg", "375", "287", "203", "101", "104" });
            expectedData.AddRange(new List<string> { "    Central", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    DssAdmin", "46", "35", "31", "0", "19" });
            expectedData.AddRange(new List<string> { "    Pchit", "31", "15", "15", "6", "9" });
            TestUtility.AssertTableContent(table, expectedData, 6, 16);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample11.json", "Data")]
        public void TestSample11()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=TECHNOLOGIES,ROW11=CRITICAL_VIOLATIONS,
             * METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS =ADDED|REMOVED,TECHNOLOGIES=ALL,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Sample11.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
            null, @".\Data\DreamTeamSnap4Sample11.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
            null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "TECHNOLOGIES"},
                {"ROW11", "CRITICAL_VIOLATIONS"},
                {"METRICS", "HEALTH_FACTOR" },
                {"CRITICAL_VIOLATIONS", "ADDED|REMOVED" },
                {"TECHNOLOGIES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "JEE", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "3", "11", "17", "22" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "PL/SQL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "C++", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { ".NET", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "3", "0", "3", "0" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "1", "0" });
            TestUtility.AssertTableContent(table, expectedData, 6, 13);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        public void TestSample12()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=10151|10107|10152|10154|10161,SNAPSHOTS=ALL
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "10151|10107|10152|10154|10161" },
                {"SNAPSHOTS", "ALL"}
            };
            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Metrics", "ADGAutoSnap_Dream Team_4 - 4", "ADGAutoSnap_Dream Team_1 - 1", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> {"Number of Code Lines", "104,851", "92,762", "12,089", "+13.0 %" });
            expectedData.AddRange(new List<string> {"Number of Comment Lines", "10,626", "10,009", "617", "+6.16 %" });
            expectedData.AddRange(new List<string> {"Number of Artifacts",  "6,727", "6,089", "638", "+10.5 %" });
            expectedData.AddRange(new List<string> {"Number of Files", "579", "485", "94", "+19.4 %" });
            expectedData.AddRange(new List<string> {"Number of Methods", "6,477", "5,883", "594", "+10.1 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4TechnicalDebt.json", "Data")]
        public void TestTechnicalSizingType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=TECHNICAL_DEBT,SNAPSHOTS=CURRENT
             * DreamTeamSnap4TechnicalDebt.json : AED3/applications/7/snapshots/15/results?sizing-measures=(technical-debt-statistics)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4TechnicalDebt.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "TECHNICAL_DEBT" },
                {"SNAPSHOTS", "CURRENT"}
            };
            
            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(5, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("Technical Debt"));
            Assert.IsTrue(data.Contains("Technical Debt density"));
            Assert.IsTrue(data.Contains("Technical Debt of added Violations"));
            Assert.IsTrue(data.Contains("Technical Debt of removed Violations"));
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("Robustness"));

        }
    }
}
