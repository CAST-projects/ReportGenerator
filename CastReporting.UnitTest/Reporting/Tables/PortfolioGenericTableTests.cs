using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class PortfolioGenericTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        /*
         * Configuration : TABLE;PF_GENERIC_TABLE;COL1=xx,COL11=xx,ROW1=xx,ROW11=xx,
         * 
         * xx = to choose between METRICS, APPLICATIONS, VIOLATIONS, CRITICAL_VIOLATIONS
         * if APPLICATIONS, parameter APPLICATIONS=EACH or application name should be added
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
        public void TestMetricsBCPortfolio()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "60014|60017|60013"},
                {"AGGREGATORS", "AVERAGE" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Applications", "Efficiency", "Total Quality Index", "Robustness"});
            expectedData.AddRange(new List<string> {"2 Applications", "2.68", "3.16", "3.50"});
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestMetricsSeveralBCEachApp()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "60014|60017|60013"},
                {"AGGREGATORS", "AVERAGE" },
                {"APPLICATIONS", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Efficiency", "Total Quality Index", "Robustness" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "2.65", "3.03", "3.32" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "2.71", "3.30", "3.68" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestMetricsSZBCPortfolio()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS"},
                {"ROW1", "METRICS"},
                {"METRICS", "60013|10151"},
                {"AGGREGATORS", "AVERAGE|SUM" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "2 Applications" });
            expectedData.AddRange(new List<string> { "Robustness", "3.50" });
            expectedData.AddRange(new List<string> { "Number of Code Lines", "67,589" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }
        
        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestMetricsHFPortfolio()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "HEALTH_FACTOR"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "2 Applications", "3.02", "2.96", "3.50", "2.68", "3.51" });
            TestUtility.AssertTableContent(table, expectedData, 6, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestMetricsFuncWeightTechDebtEachApp()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "FUNCTIONAL_WEIGHT|TECHNICAL_DEBT"},
                {"APPLICATIONS", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "OMG-Compliant Automated Function Points", "Technical Debt" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "477", "420,178" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "13,732", "175,144" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestMetricsViolStats()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "VIOLATION|CRITICAL_VIOLATION"},
                {"APPLICATIONS", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Number of violations to critical quality rules per KLOC (average)" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "1" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "2" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestViolAppli()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS" },
                {"ROW1", "VIOLATIONS"},
                {"APPLICATIONS", "ReportGenerator"},
                {"VIOLATIONS", "ADDED|REMOVED" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violations", "ReportGenerator" });
            expectedData.AddRange(new List<string> { "Added Violations", "1,691" });
            expectedData.AddRange(new List<string> { "Removed Violations", "241" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample1()
        {
            // ROW1= APPLICATIONS,COL1=METRICS,METRICS=HEALTH_FACTOR, APPLICATIONS=EACH
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS" },
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "HEALTH_FACTOR"},
                {"APPLICATIONS", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "3.07", "2.55", "3.32", "2.65", "3.30" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "2.97", "3.37", "3.68", "2.71", "3.73" });
            TestUtility.AssertTableContent(table, expectedData, 6, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample2()
        {
            // ROW1=APPLICATIONS, COL1=CRITICAL_VIOLATIONS,CRITICAL_VIOLATIONS =ALL,APPLICATIONS=EACH
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CRITICAL_VIOLATIONS" },
                {"ROW1", "APPLICATIONS"},
                {"CRITICAL_VIOLATIONS", "ALL"},
                {"APPLICATIONS", "EACH" }
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
        public void TestSample3()
        {
            // COL1=METRICS,ROW1=CRITICAL_VIOLATIONS,ROW11=APPLICATIONS,METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS =ADDED,APPLICATIONS=EACH
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS" },
                {"ROW1", "CRITICAL_VIOLATIONS"},
                {"ROW11", "APPLICATIONS"  },
                {"METRICS", "HEALTH_FACTOR"},
                {"CRITICAL_VIOLATIONS","ADDED"  },
                {"APPLICATIONS", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Critical Violations", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Added Critical Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    ReportGenerator", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    AADAEDAdmin", "0", "0", "0", "9", "0" });
            TestUtility.AssertTableContent(table, expectedData, 6, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample4()
        {
            // COL1=APPLICATIONS,ROW1=METRICS,METRICS=TECHNICAL_SIZING, APPLICATIONS=ALL,AGGREGATORS=SUM
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS" },
                {"ROW1", "METRICS"},
                {"METRICS", "TECHNICAL_SIZING"},
                {"AGGREGATORS","SUM"  },
                {"APPLICATIONS", "ALL" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "2 Applications" });
            expectedData.AddRange(new List<string> { "Number of Code Lines", "67,589" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\BusinessValue.json", "Data")]
        public void TestSample5()
        {
            // COL1=TECHNOLOGIES,ROW1=METRICS,METRICS=10151|10107|10152|10154|10161,AGGREGATORS=SUM,TECHNOLOGIES=EACH
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "TECHNOLOGIES" },
                {"ROW1", "METRICS"},
                {"METRICS", "10151|10152|10154|10161"},
                {"AGGREGATORS","SUM"  },
                {"TECHNOLOGIES", "EACH" }
            };

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
            expectedData.AddRange(new List<string> { "Metrics", ".NET", "JEE", "SQL Analyzer" });
            expectedData.AddRange(new List<string> { "Number of Code Lines", "29,486", "12,649", "25,454" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample6()
        {
            // COL1=METRICS,ROW1=TECHNOLOGIES,METRICS=HEALTH_FACTORS,TECHNOLOGIES=EACH
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS" },
                {"ROW1", "TECHNOLOGIES"},
                {"METRICS", "HEALTH_FACTOR"},
                {"TECHNOLOGIES", "EACH" }
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { ".NET", "3.07", "2.55", "3.32", "2.65", "3.30" });
            expectedData.AddRange(new List<string> { "JEE", "2.98", "3.32", "3.66", "2.76", "3.71" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "3.74", "3.92", "3.86", "2.51", "4.00" });
            TestUtility.AssertTableContent(table, expectedData, 6, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestSample7()
        {
            // COL1=TECHNOLOGIES,ROW1=APPLICATIONS,TECHNOLOGIES=EACH,APPLICATIONS=EACH,METRICS=10151
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "TECHNOLOGIES" },
                {"ROW1", "APPLICATIONS"},
                {"METRICS", "10151"},
                {"TECHNOLOGIES", "EACH" },
                {"APPLICATIONS", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Applications", ".NET", "JEE", "SQL Analyzer" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "29,486", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "AADAEDAdmin", "n/a", "12,649", "25,454" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestAppTechnosViolations()
        {
            // COL1=TECHNOLOGIES,ROW1=APPLICATIONS,TECHNOLOGIES=EACH,APPLICATIONS=EACH,METRICS=10151
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "TECHNOLOGIES" },
                {"ROW1", "VIOLATIONS"},
                {"ROW11", "APPLICATIONS"},
                {"VIOLATIONS", "TOTAL"},
                {"TECHNOLOGIES", "EACH" },
                {"APPLICATIONS", "EACH"}
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violations", ".NET", "JEE", "SQL Analyzer" });
            expectedData.AddRange(new List<string> { "Total Violations", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    ReportGenerator", "5,278", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    AADAEDAdmin", "n/a", "3,615", "101" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestTechnosViolations()
        {
            // COL1=TECHNOLOGIES,ROW1=APPLICATIONS,TECHNOLOGIES=EACH,APPLICATIONS=EACH,METRICS=10151
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "TECHNOLOGIES" },
                {"ROW1", "VIOLATIONS"},
                {"VIOLATIONS", "ALL"},
                {"TECHNOLOGIES", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violations", ".NET", "JEE", "SQL Analyzer" });
            expectedData.AddRange(new List<string> { "Total Violations", "5,278", "3,615", "101" });
            expectedData.AddRange(new List<string> { "Added Violations", "1,691", "1,690", "8" });
            expectedData.AddRange(new List<string> { "Removed Violations", "241", "1,503", "5" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestCustomExpressionPortfolioLevel()
        {
            // COL1=APPLICATIONS,ROW1=METRICS,METRICS=TECHNICAL_SIZING, APPLICATIONS=ALL,AGGREGATORS=SUM
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS" },
                {"ROW1", "CUSTOM_EXPRESSIONS"},
                {"CUSTOM_EXPRESSIONS", "a/b"},
                {"PARAMS", "SZ a SZ b" },
                {"a", "10151" },
                {"b", "10202" },
                {"AGGREGATORS", "AVERAGE" },
                {"APPLICATIONS", "ALL" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Value", "2 Applications" });
            expectedData.AddRange(new List<string> { "a/b", "32.30" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestCustomExpressionEachApplications()
        {
            // COL1=APPLICATIONS,ROW1=METRICS,METRICS=TECHNICAL_SIZING, APPLICATIONS=ALL,AGGREGATORS=SUM
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "APPLICATIONS" },
                {"ROW1", "CUSTOM_EXPRESSIONS"},
                {"CUSTOM_EXPRESSIONS", "a/b"},
                {"PARAMS", "SZ a SZ b" },
                {"a", "10151" },
                {"b", "10202" },
                {"APPLICATIONS", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Value", "ReportGenerator", "AADAEDAdmin" });
            expectedData.AddRange(new List<string> { "a/b", "61.82", "2.77" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestCustomExpressionEachTechnologies()
        {
            // COL1=APPLICATIONS,ROW1=METRICS,METRICS=TECHNICAL_SIZING, APPLICATIONS=ALL,AGGREGATORS=SUM
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS" },
                {"ROW1", "TECHNOLOGIES"},
                {"CUSTOM_EXPRESSIONS", "a/b|c"},
                {"PARAMS", "SZ a SZ b QR c" },
                {"a", "10151" },
                {"b", "10202" },
                {"c", "60012" },
                {"TECHNOLOGIES", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "a/b", "c" });
            expectedData.AddRange(new List<string> { ".NET", "61.82", "2.55" });
            expectedData.AddRange(new List<string> { "JEE", "102.84", "3.32" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "1,018.16", "3.92" });
            TestUtility.AssertTableContent(table, expectedData, 3, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestCustomExpressionEachApplicationEachTechnologies()
        {
            // COL1=APPLICATIONS,ROW1=METRICS,METRICS=TECHNICAL_SIZING, APPLICATIONS=ALL,AGGREGATORS=SUM
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            reportData.Applications[0].Technologies = new[] { ".NET" };
            reportData.Applications[1].Technologies = new[] { "JEE", "SQL Analyzer" };

            var component = new PortfolioGenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS" },
                {"ROW1", "TECHNOLOGIES"},
                {"ROW11", "APPLICATIONS"},
                {"CUSTOM_EXPRESSIONS", "a/b|c"},
                {"PARAMS", "SZ a SZ b QR c" },
                {"a", "10151" },
                {"b", "10202" },
                {"c", "60012" },
                {"APPLICATIONS", "EACH" },
                {"TECHNOLOGIES", "EACH" }
            };

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "a/b", "c" });
            expectedData.AddRange(new List<string> { ".NET", " ", " " });
            expectedData.AddRange(new List<string> { "    ReportGenerator", "61.82", "2.55" });
            expectedData.AddRange(new List<string> { "    AADAEDAdmin", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "JEE", " ", " " });
            expectedData.AddRange(new List<string> { "    ReportGenerator", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    AADAEDAdmin", "102.84", "3.32" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", " ", " " });
            expectedData.AddRange(new List<string> { "    ReportGenerator", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    AADAEDAdmin", "1,018.16", "3.92" });
            TestUtility.AssertTableContent(table, expectedData, 3, 10);
        }
    }
}
