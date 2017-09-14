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
            // snap results : AAD/applications/37/results?snapshots=-2&metrics=60013,68001,10151,10202,67013&select=evolutionSummary
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
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "BUSINESS_CRITERIA"},
                {"APPLICATIONS", "EACH" }
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
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "TECHNICAL_CRITERIA"},
                {"APPLICATIONS", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Architecture - Multi-Layers and Data Access", "Programming Practices - OO Inheritance and Polymorphism" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "1.00", "4.00" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "3.92", "4.00" });
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
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "TECHNICAL_SIZING"},
                {"APPLICATIONS", "EACH" }
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
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CRITICAL_VIOLATIONS"},
                {"ROW1", "APPLICATIONS"},
                {"APPLICATIONS", "EACH"},
                {"CRITICAL_VIOLATIONS", "ALL"},
                {"METRICS", "60017"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Total Critical Violations", "Added Critical Violations", "Removed Critical Violations" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "41", "0", "33" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "80", "9", "174" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample1()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "60016"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Security" });
            expectedData.AddRange(new List<string> { "2 Applications", "3.51" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample2()
        {
            //COL1=METRICS,ROW1=TECHNOLOGIES,METRICS=10151|60017,AGGREGATORS=SUM|AVG,TECHNOLOGIES=EACH
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "TECHNOLOGIES"},
                {"METRICS", "10151|60017"},
                {"AGGREGATORS", "SUM|AVERAGE"},
                {"TECHNOLOGIES", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Number of Code Lines", "Total Quality Index" });
            expectedData.AddRange(new List<string> { ".NET", "29486", "3.03" });
            expectedData.AddRange(new List<string> { "JEE", "12649", "3.30" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "25454", "3.48" });
            TestUtility.AssertTableContent(table, expectedData, 3, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample3()
        {
            // COL1=CRITICAL_VIOLATIONS,ROW1=APPLICATIONS,APPLICATIONS=ALL,CRITICAL_VIOLATIONS=ALL,METRICS=60017
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CRITICAL_VIOLATIONS"},
                {"ROW1", "APPLICATIONS"},
                {"APPLICATIONS", "ALL"},
                {"CRITICAL_VIOLATIONS", "ALL"},
                {"METRICS", "60017"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Total Critical Violations", "Added Critical Violations", "Removed Critical Violations" });
            expectedData.AddRange(new List<string> { "2 Applications", "121", "9", "207" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample4()
        {
            // COL1=CRITICAL_VIOLATIONS,ROW1=METRICS,METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS=ADDED|REMOVED
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CRITICAL_VIOLATIONS"},
                {"ROW1", "METRICS"},
                {"CRITICAL_VIOLATIONS", "ADDED|REMOVED"},
                {"METRICS", "HEALTH_FACTOR"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Added Critical Violations", "Removed Critical Violations" });
            expectedData.AddRange(new List<string> { "Transferability", "0", "6" });
            expectedData.AddRange(new List<string> { "Changeability", "0", "97" });
            expectedData.AddRange(new List<string> { "Robustness", "0", "10" });
            expectedData.AddRange(new List<string> { "Efficiency", "9", "106" });
            expectedData.AddRange(new List<string> { "Security", "0", "91" });
            TestUtility.AssertTableContent(table, expectedData, 3, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample5()
        {
            // COL1=APPLICATIONS,ROW1=METRICS,METRICS=60013|60014|60016,AGGREGATORS=AVERAGE,APPLICATIONS=ALL
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS"},
                {"ROW1", "METRICS"},
                {"APPLICATIONS", "ALL"},
                {"METRICS", "60013|60014|60016"},
                {"AGGREGATORS", "AVERAGE"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "2 Applications" });
            expectedData.AddRange(new List<string> { "Robustness", "3.50" });
            expectedData.AddRange(new List<string> { "Efficiency", "2.68" });
            expectedData.AddRange(new List<string> { "Security", "3.51" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample6()
        {
            // ROW1=TECHNOLOGIES,COL1=METRICS,TECHNOLOGIES=EACH,METRICS=10151,AGGREGATORS=SUM
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "TECHNOLOGIES"},
                {"METRICS", "10151"},
                {"AGGREGATORS", "SUM"},
                {"TECHNOLOGIES", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Number of Code Lines" });
            expectedData.AddRange(new List<string> { ".NET", "29486" });
            expectedData.AddRange(new List<string> { "JEE", "12649" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "25454" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestTechnoCritViol()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CRITICAL_VIOLATIONS"},
                {"ROW1", "TECHNOLOGIES"},
                {"CRITICAL_VIOLATIONS", "ALL"},
                {"TECHNOLOGIES", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Total Critical Violations", "Added Critical Violations", "Removed Critical Violations" });
            expectedData.AddRange(new List<string> { ".NET", "41", "0", "33" });
            expectedData.AddRange(new List<string> { "JEE", "44", "8", "172" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "36", "1", "2" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestAppTechnoCritViol()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericGraph();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS"},
                {"ROW1", "CRITICAL_VIOLATIONS"},
                {"ROW11", "TECHNOLOGIES"},
                {"CRITICAL_VIOLATIONS", "ADDED"},
                {"TECHNOLOGIES", "EACH"},
                {"APPLICATIONS", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Critical Violations", "ReportGenerator", "AADAEDAdmin" });
            expectedData.AddRange(new List<string> { "Added Critical Violations", " ", " " });
            expectedData.AddRange(new List<string> { "    .NET", "0", "n/a" });
            expectedData.AddRange(new List<string> { "    JEE", "n/a", "8" });
            expectedData.AddRange(new List<string> { "    SQL Analyzer", "n/a", "1" });
            TestUtility.AssertTableContent(table, expectedData, 3, 5);
        }

    }
}
