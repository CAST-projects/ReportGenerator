using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class DeltaComponentsListByStatusTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestNoConfig()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
               null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "get", "low risk", "low risk", "low risk", "very high risk", "moderate risk", "0", "CastReporting.BLL.Computing.ViolationSummaryModuleDTO.this.get" });
            expectedData.AddRange(new List<string> { "SetActionsPlan", "moderate risk", "low risk", "moderate risk", "moderate risk", "low risk", "0", "CastReporting.BLL.PortfolioSnapshotsBLL.SetActionsPlan" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.BLL.RuleBLL.GetRulesDetails" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "very high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.Domain.Interfaces.IRuleExplorer.GetRulesDetails" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNoConfigNoPrevious()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "Previous snapshot was not found.", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });

            TestUtility.AssertTableContent(table, expectedData, 8, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestLowComplexityDeleted()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COMPLEXITY", "low" },
                {"STATUS", "deleted" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "get", "low risk", "low risk", "low risk", "very high risk", "moderate risk", "0", "CastReporting.BLL.Computing.ViolationSummaryModuleDTO.this.get" });
            TestUtility.AssertTableContent(table, expectedData, 8, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestBadStatus()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STATUS", "removed" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "Selected status not allowed. Allowed status are : added, deleted or updated.", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });

            TestUtility.AssertTableContent(table, expectedData, 8, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestBadComplexity()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COMPLEXITY", "average" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "get", "low risk", "low risk", "low risk", "very high risk", "moderate risk", "0", "CastReporting.BLL.Computing.ViolationSummaryModuleDTO.this.get" });
            expectedData.AddRange(new List<string> { "SetActionsPlan", "moderate risk", "low risk", "moderate risk", "moderate risk", "low risk", "0", "CastReporting.BLL.PortfolioSnapshotsBLL.SetActionsPlan" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.BLL.RuleBLL.GetRulesDetails" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "very high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.Domain.Interfaces.IRuleExplorer.GetRulesDetails" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestModule()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                @".\Data\ModulesCoCRA.json", @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"MODULE", "ReportGenerator" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "get", "low risk", "low risk", "low risk", "very high risk", "moderate risk", "0", "CastReporting.BLL.Computing.ViolationSummaryModuleDTO.this.get" });
            expectedData.AddRange(new List<string> { "SetActionsPlan", "moderate risk", "low risk", "moderate risk", "moderate risk", "low risk", "0", "CastReporting.BLL.PortfolioSnapshotsBLL.SetActionsPlan" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.BLL.RuleBLL.GetRulesDetails" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "very high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.Domain.Interfaces.IRuleExplorer.GetRulesDetails" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestBadModule()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                @".\Data\ModulesCoCRA.json", @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"MODULE", "Module 2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "Module not found in this application.", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            TestUtility.AssertTableContent(table, expectedData, 8, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestTechnology()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.Application.Technologies = new[] { "JEE" };

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"TECHNOLOGY", "JEE" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "get", "low risk", "low risk", "low risk", "very high risk", "moderate risk", "0", "CastReporting.BLL.Computing.ViolationSummaryModuleDTO.this.get" });
            expectedData.AddRange(new List<string> { "SetActionsPlan", "moderate risk", "low risk", "moderate risk", "moderate risk", "low risk", "0", "CastReporting.BLL.PortfolioSnapshotsBLL.SetActionsPlan" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.BLL.RuleBLL.GetRulesDetails" });
            expectedData.AddRange(new List<string> { "GetRulesDetails", "very high risk", "low risk", "low risk", "low risk", "low risk", "0", "CastReporting.Domain.Interfaces.IRuleExplorer.GetRulesDetails" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestBadTechnology()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.Application.Technologies = new[] { "JEE" };

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"TECHNOLOGY", ".NET" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "Technology not found in this application.", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            TestUtility.AssertTableContent(table, expectedData, 8, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestLimitResults()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "get", "low risk", "low risk", "low risk", "very high risk", "moderate risk", "0", "CastReporting.BLL.Computing.ViolationSummaryModuleDTO.this.get" });
            expectedData.AddRange(new List<string> { "SetActionsPlan", "moderate risk", "low risk", "moderate risk", "moderate risk", "low risk", "0", "CastReporting.BLL.PortfolioSnapshotsBLL.SetActionsPlan" });
            TestUtility.AssertTableContent(table, expectedData, 8, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "1.5.0", "V-1.5.0", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "1.4.0", "V-1.4.0", previousDate);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.ServerVersion = "1.8.0.772";

            var component = new CastReporting.Reporting.Block.Table.DeltaComponentsListByStatus();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Object Name", "Complexity", "SQL Complexity", "Granularity", "Lack of comments", "Coupling", "Number of object updates", "Object full name" });
            expectedData.AddRange(new List<string> { "No data found", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });

            TestUtility.AssertTableContent(table, expectedData, 8, 2);
        }
    }
}

