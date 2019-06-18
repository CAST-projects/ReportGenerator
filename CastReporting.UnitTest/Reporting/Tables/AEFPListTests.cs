using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class AEFPListTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsEvolutions.csv", "Data")]
        public void TestAEFPDefaultConfig()
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

            var component = new AEFPList();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Type", "Technology", "Module Name", "Object Name", "AEP", "Status", "Complexity Factor", "Updated Artifacts"});
            expectedData.AddRange(new List<string> { "Data Function", "COI5F21", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COCPB10.COI5F21", "5", "Added", "1.0", "-"});
            expectedData.AddRange(new List<string> { "Data Function", "CSI0F61", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI0F61", "5", "Added", "1.0", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "CSI1F41", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F41", "5", "Added", "1.0", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "CSI1F51", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F51", "5", "Modified", "1.0", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "CSI1F62", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F62", "5", "Modified", "1.0", "-" });
            expectedData.AddRange(new List<string> {"Data Function", "EKPO", "SAP unresolved Table", "SAP SQL", "Abap_castpubs", "unresolvedObjects/SAP_TABLE/EKPO", "3", "Deleted", "0.4", "-"});
            expectedData.AddRange(new List<string> {"Data Function", "COI1F91", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI1F91", "2", "Deleted", "0.4", "-"});
            expectedData.AddRange(new List<string> { "Data Function", "COI2F01", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F01", "2", "Deleted", "0.4", "-"});
            expectedData.AddRange(new List<string> { "Data Function", "COI2F11", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F11", "2", "Deleted", "0.4", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "COI1F71", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB18.COI1F71", "1", "Modified", "0.25", "0" });
            TestUtility.AssertTableContent(table, expectedData, 10, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsEvolutions.csv", "Data")]
        public void TestAEFPAddedDataFunction()
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

            var component = new AEFPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "TYPE", "DF"},
                { "STATUS", "ADDED"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Function Name", "Object Type", "Technology", "Module Name", "Object Name", "AEP", "Status", "Complexity Factor", "Updated Artifacts" });
            expectedData.AddRange(new List<string> { "COI5F21", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COCPB10.COI5F21", "5", "Added", "1.0", "-" });
            expectedData.AddRange(new List<string> { "CSI0F61", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI0F61", "5", "Added", "1.0", "-" });
            expectedData.AddRange(new List<string> { "CSI1F41", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F41", "5", "Added", "1.0", "-" });
            TestUtility.AssertTableContent(table, expectedData, 9, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsEvolutions.csv", "Data")]
        public void TestAEFPModifiedTransactional()
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

            var component = new AEFPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "TYPE", "TF"},
                { "STATUS", "MODIFIED"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Function Name", "Object Type", "Technology", "Module Name", "Object Name", "AEP", "Status", "Complexity Factor", "Updated Artifacts" });
            expectedData.AddRange(new List<string> { "ERCO0CP2", "JCL Job", "JCL", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0CP2", "4", "Modified", "0.5", "1" });
            expectedData.AddRange(new List<string> { "ERCO0MPY", "JCL Job", "JCL", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0MPY", "4", "Modified", "0.5", "0" });
            expectedData.AddRange(new List<string> { "ERCS0NPA", "JCL Job", "JCL", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCS0NPA", "4", "Modified", "0.5", "2" });
            TestUtility.AssertTableContent(table, expectedData, 9, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsEvolutions.csv", "Data")]
        public void TestAEFPAllDeleted()
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

            var component = new AEFPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "COUNT", "-1"},
                { "STATUS", "DELETED"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Type", "Technology", "Module Name", "Object Name", "AEP", "Status", "Complexity Factor", "Updated Artifacts" });
            expectedData.AddRange(new List<string> { "Data Function", "EKPO", "SAP unresolved Table", "SAP SQL", "Abap_castpubs", "unresolvedObjects/SAP_TABLE/EKPO", "3", "Deleted", "0.4", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "COI1F91", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI1F91", "2", "Deleted", "0.4", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "COI2F01", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F01", "2", "Deleted", "0.4", "-" });
            expectedData.AddRange(new List<string> { "Data Function", "COI2F11", "Cobol File Link", "Cobol", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F11", "2", "Deleted", "0.4", "-" });
            expectedData.AddRange(new List<string> { "Transactional", "ERCO0DPA", "JCL Job", "JCL", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0DPA", "2", "Deleted", "0.25", "4" });
            expectedData.AddRange(new List<string> { "Transactional", "ERCO0SP2", "JCL Job", "JCL", "Presales", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0SP2", "2", "Deleted", "0.25", "1" });
            TestUtility.AssertTableContent(table, expectedData, 10, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\OmgFunctionsEvolutions.csv", "Data")]
        public void TestAEFPBadVersion()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            reportData.ServerVersion = "1.8.0.456";
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new AEFPList();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "COUNT", "-1"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Type", "Technology", "Module Name", "Object Name", "AEP", "Status", "Complexity Factor", "Updated Artifacts" });
            expectedData.AddRange(new List<string> { "No data found", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            TestUtility.AssertTableContent(table, expectedData, 10, 2);
        }
    }
}
