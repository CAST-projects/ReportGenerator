using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class AETPListTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsTechnical.csv", "Data")]
        public void TestAETPDefaultConfig()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new AETPList();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Object full name", "Object Type", "Status", "Effort complexity", "Equivalence ratio", "AEP" });
            expectedData.AddRange(new List<string> { "browserid.js", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\browserid.js]", "eFile", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "browserID", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\browserid.js].browserID", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "drawCal", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].drawCal", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "fnInit", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].fnInit", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "getDays", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].getDays", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "getMonthName", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].getMonthName", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "leapYear", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].leapYear", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "setCal", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].setCal", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "onclick", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].COMPONENT.onclick", "eEvent", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "start", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].COMPONENT.start", "eMethod", "added", "0.05", "45.43", "2.27" });
            TestUtility.AssertTableContent(table, expectedData, 7, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsTechnical.csv", "Data")]
        public void TestAETPCount()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new AETPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "COUNT", "3"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Object full name", "Object Type", "Status", "Effort complexity", "Equivalence ratio", "AEP" });
            expectedData.AddRange(new List<string> { "browserid.js", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\browserid.js]", "eFile", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "browserID", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\browserid.js].browserID", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "drawCal", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].drawCal", "eFunction", "added", "0.05", "45.43", "2.27" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsTechnical.csv", "Data")]
        public void TestAETPCountUnlimited()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new AETPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "COUNT", "-1"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Object full name", "Object Type", "Status", "Effort complexity", "Equivalence ratio", "AEP" });
            expectedData.AddRange(new List<string> { "browserid.js", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\browserid.js]", "eFile", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "browserID", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\browserid.js].browserID", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "drawCal", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].drawCal", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "fnInit", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].fnInit", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "getDays", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].getDays", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "getMonthName", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].getMonthName", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "leapYear", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].leapYear", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "setCal", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].setCal", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "onclick", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].COMPONENT.onclick", "eEvent", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "start", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].COMPONENT.start", "eMethod", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "stop", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].COMPONENT.stop", "eMethod", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "getDir", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].getDir", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "onClick", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].onClick", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "putDir", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].putDir", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "setDir", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].setDir", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "start", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].start", "eFunction", "added", "0.05", "45.43", "2.27" });
            expectedData.AddRange(new List<string> { "stop", "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\composant.htc].stop", "eFunction", "added", "0.05", "45.43", "2.27" });
            TestUtility.AssertTableContent(table, expectedData, 7, 18);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsTechnical.csv", "Data")]
        public void TestAETPBadVersion()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            reportData.ServerVersion = "1.8.0.999";
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new AETPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "COUNT", "-1"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Object full name", "Object Type", "Status", "Effort complexity", "Equivalence ratio", "AEP" });
            expectedData.AddRange(new List<string> { "No data found", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            TestUtility.AssertTableContent(table, expectedData, 7, 2);
        }

    }
}
