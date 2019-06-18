using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TopRiskiestTransactionsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60016Snap.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.TopRiskiestTransactions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SRC","SEC" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Transaction Entry Point", "TRI"});
            expectedData.AddRange(new List<string> { "Save", "12,208" });
            expectedData.AddRange(new List<string> { "SaveJavaProcedureParams", "12,208" });
            expectedData.AddRange(new List<string> { "Save", "11,658" });
            expectedData.AddRange(new List<string> { "SaveJavaProcedureParams", "11,608" });
            expectedData.AddRange(new List<string> { "Save", "10,894" });
            expectedData.AddRange(new List<string> { "saveJavaProcedureParams", "10,894" });
            expectedData.AddRange(new List<string> { "SaveObject", "10,344" });
            expectedData.AddRange(new List<string> { "SaveParameters", "10,344" });
            expectedData.AddRange(new List<string> { "SaveDynamicLinkCC", "8,592" });
            expectedData.AddRange(new List<string> { "SaveModified", "7,768" });
            TestUtility.AssertTableContent(table, expectedData, 2, 11);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60016Snap.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.TopRiskiestTransactions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SRC","SEC" },
                {"COUNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Transaction Entry Point", "TRI" });
            expectedData.AddRange(new List<string> { "Save", "12,208" });
            expectedData.AddRange(new List<string> { "SaveJavaProcedureParams", "12,208" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

    }
}

