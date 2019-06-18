using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class MetricTopArtefactsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017.json", "Data")]
        [DeploymentItem(@".\Data\topArtefacts7212.csv", "Data")]
        public void TestOneRule()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017.json", null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.MetricTopArtifact();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" },
                {"IDX","0" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Sample Violating Artefacts for Rule 'Avoid instantiations inside loops'", "# 8 of 8"});
            expectedData.AddRange(new List<string> { "CastReporting.BLL.Computing.RulesViolationUtility.GetNbViolationByRule","" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Block.Graph.PieModuleArtifact.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Block.Table.MetricTopArtifact.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Builder.BlockProcessing.GraphBlock.ApplyContent", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Builder.ExcelDocumentBuilder.UpdateMergedCellReferences", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Helper.GenericContent.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Helper.OptionsHelper.GetIntListOption", "" });
            expectedData.AddRange(new List<string> { "CastReporting.UnitTest.Reporting.TestUtility.PrepaPortfolioReportData", "" });
            TestUtility.AssertTableContent(table, expectedData, 2, 8);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017extract.json", "Data")]
        [DeploymentItem(@".\Data\topArtefacts7212.csv", "Data")]
        [DeploymentItem(@".\Data\topArtefacts3576.csv", "Data")]
        public void TestAllRules()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017extract.json", null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.MetricTopArtifact();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Sample Violating Artefacts for Rule 'Avoid instantiations inside loops'", "# 8 of 8" });
            expectedData.AddRange(new List<string> { "CastReporting.BLL.Computing.RulesViolationUtility.GetNbViolationByRule", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Block.Graph.PieModuleArtifact.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Block.Table.MetricTopArtifact.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Builder.BlockProcessing.GraphBlock.ApplyContent", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Builder.ExcelDocumentBuilder.UpdateMergedCellReferences", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Helper.GenericContent.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Helper.OptionsHelper.GetIntListOption", "" });
            expectedData.AddRange(new List<string> { "CastReporting.UnitTest.Reporting.TestUtility.PrepaPortfolioReportData", "" });
            expectedData.AddRange(new List<string> { "Sample Violating Artefacts for Rule 'Avoid declaring public Fields'", "# 5 of 5" });
            expectedData.AddRange(new List<string> { "CastReporting.BLL.Computing.DTO.EvolutionResult.type", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Builder.ExcelDocumentBuilder.reportData", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Helper.GenericContent.ObjConfig.Parameters", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.ReportingModel.ReportData.RuleExplorer", "" });
            expectedData.AddRange(new List<string> { "CastReporting.UI.WPF.Commands.MenuCommand.OpenHelp", "" });
            TestUtility.AssertTableContent(table, expectedData, 2, 14);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017.json", "Data")]
        [DeploymentItem(@".\Data\topArtefacts7212.csv", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017.json", null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.MetricTopArtifact();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60017" },
                {"IDX","0" },
                {"COUNT","3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Sample Violating Artefacts for Rule 'Avoid instantiations inside loops'", "# 3 of 8" });
            expectedData.AddRange(new List<string> { "CastReporting.BLL.Computing.RulesViolationUtility.GetNbViolationByRule", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Block.Graph.PieModuleArtifact.Content", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Block.Table.MetricTopArtifact.Content", "" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\cc60017.json", "Data")]
        [DeploymentItem(@".\Data\topArtefacts3576.csv", "Data")]
        public void TestIdx()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddSameCriticalRuleViolationsForAllBC(reportData, @".\Data\cc60017.json", null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.MetricTopArtifact();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60017" },
                {"IDX","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Sample Violating Artefacts for Rule 'Avoid declaring public Fields'", "# 5 of 5" });
            expectedData.AddRange(new List<string> { "CastReporting.BLL.Computing.DTO.EvolutionResult.type", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Builder.ExcelDocumentBuilder.reportData", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.Helper.GenericContent.ObjConfig.Parameters", "" });
            expectedData.AddRange(new List<string> { "CastReporting.Reporting.ReportingModel.ReportData.RuleExplorer", "" });
            expectedData.AddRange(new List<string> { "CastReporting.UI.WPF.Commands.MenuCommand.OpenHelp", "" });
            TestUtility.AssertTableContent(table, expectedData, 2, 5);
        }

    }
}

