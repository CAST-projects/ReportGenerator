using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TopRiskiestComponentsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Component60016Snap.json", "Data")]
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

            var component = new CastReporting.Reporting.Block.Table.TopRiskiestComponents();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SRC","SEC" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Artifact Name", "PRI"});
            expectedData.AddRange(new List<string> { "central.dss_metric_results", "229,656" });
            expectedData.AddRange(new List<string> { "central.dss_history", "180,488" });
            expectedData.AddRange(new List<string> { "central.dss_snapshots", "140,952" });
            expectedData.AddRange(new List<string> { "measure.dss_history", "140,504" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.dumpStack", "122,280" });
            TestUtility.AssertTableContent(table, expectedData, 2, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Component60016Snap.json", "Data")]
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

            var component = new CastReporting.Reporting.Block.Table.TopRiskiestComponents();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SRC","SEC" },
                {"COUNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Artifact Name", "PRI" });
            expectedData.AddRange(new List<string> { "central.dss_metric_results", "229,656" });
            expectedData.AddRange(new List<string> { "central.dss_history", "180,488" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Component60016ModSnap.json", "Data")]
        public void TestModuleContent()
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

            var component = new CastReporting.Reporting.Block.Table.TopRiskiestComponents();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SRC","SEC" },
                {"MOD","4" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Artifact Name", "PRI" });
            expectedData.AddRange(new List<string> { "measure.dss_history", "140,504" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.dumpStack", "122,280" });
            expectedData.AddRange(new List<string> { "measure.dss_snapshots", "56,224" });
            expectedData.AddRange(new List<string> { "com.castsoftware.aad.common.AadCommandLine.logInBase", "54,120" });
            expectedData.AddRange(new List<string> { "measure.dss_objects", "42,504" });
            TestUtility.AssertTableContent(table, expectedData, 2, 7);
        }

    }
}

