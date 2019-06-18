using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class IfpugFunctionsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\IfpugFunctions.csv", "Data")]
        public void TestAllIfpugFunctions()
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

            var component = new IfpugFunctions();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Object Name", "# of FPs", "FP Details", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES", "5", "DET: 4, RET: 1, EIF: 5 (External)",  "Oracle table",  "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES_DESC", "5", "DET: 3, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.ZONES_TO_GEO_ZONES_TAX_TPL", "5", "DET: 6, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\fancontent.jsp]", "5", "DET: 12, FTR: 2 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\productslider.jsp]", "5", "DET: 25, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-central\\WebContent\\merchantstore\\editmerchantstore.jsp]", "4", "DET: 6, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-central\\WebContent\\profile\\userList.jsp]", "4", "DET: 6, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            TestUtility.AssertTableContent(table, expectedData, 7, 8);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\IfpugFunctionsNew.csv", "Data")]
        public void TestIfpugDataFunctions()
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

            var component = new IfpugFunctions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "TYPE", "DF"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Object Name", "# of FPs", "FP Details", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES", "5", "DET: 4, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES_DESC", "5", "DET: 3, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.ZONES_TO_GEO_ZONES_TAX_TPL", "5", "DET: 6, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\IfpugFunctionsNew.csv", "Data")]
        public void TestIfpugTransactionsFunctions()
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

            var component = new IfpugFunctions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"TYPE","TF" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Object Name", "# of FPs", "FP Details", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\fancontent.jsp]", "5", "DET: 12, FTR: 2 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\productslider.jsp]", "5", "DET: 25, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-central\\WebContent\\merchantstore\\editmerchantstore.jsp]", "4", "DET: 6, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-central\\WebContent\\profile\\userList.jsp]", "4", "DET: 6, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            TestUtility.AssertTableContent(table, expectedData, 7, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\IfpugFunctionsNew.csv", "Data")]
        public void TestCountIfpugFunctions()
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

            var component = new IfpugFunctions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "COUNT", "4"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Element Type", "Object Name", "# of FPs", "FP Details", "Object Type", "Module Name", "Technology" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES", "5", "DET: 4, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES_DESC", "5", "DET: 3, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.ZONES_TO_GEO_ZONES_TAX_TPL", "5", "DET: 6, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\fancontent.jsp]", "5", "DET: 12, FTR: 2 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            TestUtility.AssertTableContent(table, expectedData, 7, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\IfpugFunctions.csv", "Data")]
        public void TestNoHeaderIfpugFunctions()
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

            var component = new IfpugFunctions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"HEADER","NO" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES", "5", "DET: 4, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.PRODUCTS_OPTIONS_VALUES_DESC", "5", "DET: 3, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Data Function", "CASTDB.SHOPIZER.ZONES_TO_GEO_ZONES_TAX_TPL", "5", "DET: 6, RET: 1, EIF: 5 (External)", "Oracle table", "eCommerce full content", "PL/SQL" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\fancontent.jsp]", "5", "DET: 12, FTR: 2 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-shop\\WebContent\\integration\\portlets\\productslider.jsp]", "5", "DET: 25, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-central\\WebContent\\merchantstore\\editmerchantstore.jsp]", "4", "DET: 6, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            expectedData.AddRange(new List<string> { "Transaction", "[C:\\CASTMS73\\Deploy\\eCommerce\\Java\\sm-central\\WebContent\\profile\\userList.jsp]", "4", "DET: 6, FTR: 1 (Output or Inquiry)", "eFile", "eCommerce full content", "JEE" });
            TestUtility.AssertTableContent(table, expectedData, 7, 7);
            Assert.IsFalse(table.HasColumnHeaders);
        }

    }
}
