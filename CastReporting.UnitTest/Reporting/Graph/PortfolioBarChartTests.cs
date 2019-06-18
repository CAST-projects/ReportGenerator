using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class PortfolioBarChartTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1SnapResults.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2SnapResults.json", "Data")]
        public void TestContentBusinessCriteria()
        {
            /*
             * Configuration : GRAPH;PF_BAR_CHART;METRIC=60014
             * AADApplications.json : AAD2/applications
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             * AADApplication2Snap.json : AAD2/applications/24/snapshots/12
             * AADApplication2SnapResults.json : AAD2/applications/24/snapshots/12/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             */
            List<string> snapList = new List<string>{ @".\Data\AADApplication1Snap.json" , @".\Data\AADApplication2Snap.json"};
            List<string> snapResultsList = new List<string> { @".\Data\AADApplication1SnapResults.json", @".\Data\AADApplication2SnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioBarChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRIC", "60014"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Efficiency" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "1.8780487804878" });
            expectedData.AddRange(new List<string> { "Big Ben", "1.32464620041293" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1SnapResults.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2SnapResults.json", "Data")]
        public void TestContentTechnicalCriteria()
        {
            /*
             * Configuration : GRAPH;PF_BAR_CHART;METRIC=61004
             * AADApplications.json : AAD2/applications
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             * AADApplication2Snap.json : AAD2/applications/24/snapshots/12
             * AADApplication2SnapResults.json : AAD2/applications/24/snapshots/12/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             */
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADApplication1SnapResults.json", @".\Data\AADApplication2SnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioBarChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRIC", "61004"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Architecture - OS and Platform Independence" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "3.94396369907646" });
            expectedData.AddRange(new List<string> { "Big Ben", "3.49944922722343" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1SnapResults.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2SnapResults.json", "Data")]
        public void TestContentQualityRule()
        {
            /*
             * Configuration : GRAPH;PF_BAR_CHART;METRIC=7856
             * AADApplications.json : AAD2/applications
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             * AADApplication2Snap.json : AAD2/applications/24/snapshots/12
             * AADApplication2SnapResults.json : AAD2/applications/24/snapshots/12/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             */
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADApplication1SnapResults.json", @".\Data\AADApplication2SnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioBarChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRIC", "7856"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Avoid Tables with more than 20 columns on an OLTP system (7856)" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "2.88888888888889" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1SnapResults.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2SnapResults.json", "Data")]
        public void TestContentSizingMeasure()
        {
            /*
             * Configuration : GRAPH;PF_BAR_CHART;METRIC=10151
             * AADApplications.json : AAD2/applications
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             * AADApplication2Snap.json : AAD2/applications/24/snapshots/12
             * AADApplication2SnapResults.json : AAD2/applications/24/snapshots/12/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             */
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADApplication1SnapResults.json", @".\Data\AADApplication2SnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioBarChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRIC", "10151"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Number of Code Lines" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "89848" });
            expectedData.AddRange(new List<string> { "Big Ben", "219490" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1SnapResults.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2SnapResults.json", "Data")]
        [DeploymentItem(@".\Data\BusinessValue.json", "Data")]
        public void TestContentBackgroundFact()
        {
            /*
             * Configuration : GRAPH;PF_BAR_CHART;METRIC=66061
             * AADApplications.json : AAD2/applications
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4
             * AADApplication1Snap.json : AAD2/applications/3/snapshots/4/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             * AADApplication2Snap.json : AAD2/applications/24/snapshots/12
             * AADApplication2SnapResults.json : AAD2/applications/24/snapshots/12/results?quality-indicators=(60014,61004,550,7654,7856)&sizing-measures=(10151,68001,10202,67210,67011)
             */
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADApplication1SnapResults.json", @".\Data\AADApplication2SnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioBarChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRIC", "66061"}
            };

            // Needed for background facts, as there are retrieved one by one by url request
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Business Value" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "3" });
            expectedData.AddRange(new List<string> { "Big Ben", "3" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

    }
}
