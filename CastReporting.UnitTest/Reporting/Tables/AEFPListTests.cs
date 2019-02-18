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
            WSConnection connection = new WSConnection()
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
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Name", "# of FPs", "Complexity Factor", "Updated Artifacts", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Added Data Function AEFP", "COI5F21", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COCPB10.COI5F21", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Added Data Function AEFP", "CSI0F61", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI0F61", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Added Data Function AEFP", "CSI1F41", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F41", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Modified Data Function AEFP", "CSI1F51", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F51", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Modified Data Function AEFP", "CSI1F62", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F62", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "EKPO", "unresolvedObjects/SAP_TABLE/EKPO", "3", "0.4", "-", "SAP unresolved Table", "Abap_castpubs", "SAP SQL" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "COI1F91", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI1F91", "2", "0.4", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "COI2F01", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F01", "2", "0.4", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "COI2F11", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F11", "2", "0.4", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Modified Data Function AEFP", "COI1F71", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB18.COI1F71", "1", "0.25", "0", "Cobol File Link", "Presales", "Cobol" });
            TestUtility.AssertTableContent(table, expectedData, 9, 11);
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
            WSConnection connection = new WSConnection()
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
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Name", "# of FPs", "Complexity Factor", "Updated Artifacts", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Added Data Function AEFP", "COI5F21", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COCPB10.COI5F21", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Added Data Function AEFP", "CSI0F61", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI0F61", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Added Data Function AEFP", "CSI1F41", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].CSVAB482.CSI1F41", "5", "1.0", "-", "Cobol File Link", "Presales", "Cobol" });
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
            WSConnection connection = new WSConnection()
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
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Name", "# of FPs", "Complexity Factor", "Updated Artifacts", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Modified Transactional AEFP", "ERCO0CP2", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0CP2", "4", "0.5", "1", "JCL Job", "Presales", "JCL" });
            expectedData.AddRange(new List<string> { "Modified Transactional AEFP", "ERCO0MPY", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0MPY", "4", "0.5", "0", "JCL Job", "Presales", "JCL" });
            expectedData.AddRange(new List<string> { "Modified Transactional AEFP", "ERCS0NPA", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCS0NPA", "4", "0.5", "2", "JCL Job", "Presales", "JCL" });
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
            WSConnection connection = new WSConnection()
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
            expectedData.AddRange(new List<string> { "Element Type", "Function Name", "Object Name", "# of FPs", "Complexity Factor", "Updated Artifacts", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "EKPO", "unresolvedObjects/SAP_TABLE/EKPO", "3", "0.4", "-", "SAP unresolved Table", "Abap_castpubs", "SAP SQL" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "COI1F91", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI1F91", "2", "0.4", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "COI2F01", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F01", "2", "0.4", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Deleted Data Function AEFP", "COI2F11", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\PROG-BATCH].COVAB24.COI2F11", "2", "0.4", "-", "Cobol File Link", "Presales", "Cobol" });
            expectedData.AddRange(new List<string> { "Deleted Transactional AEFP", "ERCO0DPA", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0DPA", "2", "0.25", "4", "JCL Job", "Presales", "JCL" });
            expectedData.AddRange(new List<string> { "Deleted Transactional AEFP", "ERCO0SP2", "[c:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Big Ben\\Presales\\JCL].ERCO0SP2", "2", "0.25", "1", "JCL Job", "Presales", "JCL" });
            TestUtility.AssertTableContent(table, expectedData, 9, 7);
        }
    }
}
