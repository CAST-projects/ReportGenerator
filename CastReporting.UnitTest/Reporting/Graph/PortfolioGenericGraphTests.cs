using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class PortfolioGenericGraphTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        /*
         * Configuration : TABLE;PF_GENERIC_GRAPH;COL1=xx,COL11=xx,ROW1=xx,ROW11=xx,
         * 
         * xx = to choose between PERIODS, METRICS, APPLICATIONS, VIOLATIONS, CRITICAL_VIOLATIONS
         * if PERIODS, parameters PERIODS= should be added, with CURRENT|PREVIOUS|EVOL|EVOL_PERCENT, and PERIOD_DURATION=99 should be added with 99 corresponding to the number of months a period has (3 months by default)
         * if APPLICATIONS, parameter APPLICATIONS=ALL or application name should be added
         * if VIOLATIONS or CRITICAL_VIOLATIONS, a METRICS= should be added, if not, TQI will be taken of no METRICS in COL1, COL11, ROW1, ROW11, else health factor, also VIOLATIONS= or CRITICAL_VIOLATIONS= with TOTAL, ADDED, REMOVED or ALL (as for GENERIC_TABLE)
         * if METRICS, parameter METRICS= should be added, if not, health factor. METRICS can be grouped like for GENERIC_TABLE component. If no APPLICATIONS, a parameter AGGREGATORS should be added, containing the list of AGGREGATORS (must be AVERAGE or SUM) corresponding to the list of METRICS
         * for example, if METRICS=60017,68001,66024 then AGGREGATORS=AVERAGE,SUM,AVERAGE
         * for groups, you can precise METRICS=HEALTH_FACTOR,TECHNICAL_SIZING then AGGREGATORS=AVERAGE,SUM, but for groups, by default AVERAGE willbe taken for groups of quality indicators, and SUM for sizing measures or background facts
         * 
         */

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestApplicationsBC()
        {
            // snap results : AAD/applications/37/results?snapshots=-2&metrics=60013,68001,10151,10202,67013&select=evolutionSummary
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            TestUtility.PreparePortfSnapshots(reportData);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "BUSINESS_CRITERIA"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Transferability", "Changeability", "Robustness", "Efficiency", "SEI Maintainability", "Security", "Total Quality Index", "Programming Practices", "Architectural Design", "Documentation" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "3.07", "2.55", "3.32", "2.65", "3.31", "3.30", "3.03" ,"3.32", "1.81", "2.57" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "2.97", "3.37", "3.68", "2.71", "3.34", "3.73", "3.30", "3.36", "3.24", "2.17" });
            TestUtility.AssertTableContent(table, expectedData, 11, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestApplicationsTCPrev()
        {
            // snap results : AAD/applications/37/results?snapshots=-2&metrics=60013,68001,10151,10202,67013&select=evolutionSummary
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            TestUtility.PreparePortfSnapshots(reportData);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "TECHNICAL_CRITERIA"},
                {"PERIODS", "PREVIOUS" },
                {"PERIOD_DURATION", "6" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Architecture - Multi-Layers and Data Access", "Programming Practices - OO Inheritance and Polymorphism" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "1.00", "4.00" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestApplicationsSize()
        {
            // snap results : AAD/applications/37/results?snapshots=-2&metrics=60013,68001,10151,10202,67013&select=evolutionSummary
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            TestUtility.PreparePortfSnapshots(reportData);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "TECHNICAL_SIZING"},
                {"PERIODS", "CURRENT" },
                {"PERIOD_DURATION", "4" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Number of Code Lines" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "29486" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "38103" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestCritViolApp()
        {
            // snap results : AAD/applications/37/results?snapshots=-2&metrics=60013,68001,10151,10202,67013&select=evolutionSummary
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            TestUtility.PreparePortfSnapshots(reportData);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CRITICAL_VIOLATIONS"},
                {"ROW1", "APPLICATIONS"},
                {"APPLICATIONS", "ALL"},
                {"CRITICAL_VIOLATIONS", "ALL"},
                { "METRICS", "60017"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Total Critical Violations", "Added Critical Violations", "Removed Critical Violations" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "41", "0", "33" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "80", "9", "174" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

    }
}
