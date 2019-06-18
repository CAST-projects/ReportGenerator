using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ViolationsListByBCTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestShortNamesLimit()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","4" },
                {"NAME","SHORT" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "PRI", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "unchanged", "1,288", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "Helper", "unchanged" });
            expectedData.AddRange(new List<string> { "added", "336", "added", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "ViewModel", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "168", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "Interfaces", "unchanged" });
            expectedData.AddRange(new List<string> { "updated", "168", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "Builder", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestAdded2BC()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" },
                {"BCID","60014|60011" },
                {"FILTER","ADDED" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "PRI", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "added", "2,960", "n/a", "pending", "Avoid instantiations inside loops", "Efficiency", "CastReporting.UnitTest.Reporting.TestUtility.PrepaPortfolioReportData", "updated" });
            expectedData.AddRange(new List<string> { "added", "1,600", "n/a", "pending", "Avoid instantiations inside loops", "Efficiency", "CastReporting.Reporting.Builder.BlockProcessing.GraphBlock.ApplyContent", "unchanged" });
            expectedData.AddRange(new List<string> { "added", "1,600", "n/a", "pending", "Avoid instantiations inside loops", "Efficiency", "CastReporting.Reporting.Builder.BlockProcessing.GraphBlock.UpdateCachedValues", "updated" });
            expectedData.AddRange(new List<string> { "added", "160", "n/a", "pending", "Avoid instantiations inside loops", "Efficiency", "CastReporting.Reporting.Block.Graph.TrendMetricId.Content", "updated" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestUnchanged3BC()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" },
                {"BCID","60014|60016|60013" },
                {"FILTER","UNCHANGED" },
                {"NAME","FULL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "PRI", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "unchanged", "320", "n/a", "pending", "Avoid instantiations inside loops", "Efficiency", "CastReporting.Reporting.Block.Table.TechnicalCriteriaRules.Content", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "320", "n/a", "pending", "Avoid instantiations inside loops", "Efficiency", "CastReporting.Reporting.Block.Table.TechnoLoC.Content", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "1,288", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.Reporting.Helper", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "168", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.Mediation.Interfaces", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "4,452", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Robustness", "CastReporting.Reporting.Builder.BlockProcessing", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "126", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Robustness", "CastReporting.Mediation.Interfaces", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "84", "added", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Robustness", "CastReporting.UI.WPF", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 8, 8);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestTQI()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"BCID","60017" },
                {"NAME","SHORT" },
                {"FILTER","ALL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "added", "n/a", "pending", "Avoid instantiations inside loops", "Total Quality Index", "GetNbViolationByRule", "updated" });
            expectedData.AddRange(new List<string> { "unchanged", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Total Quality Index", "Interfaces", "unchanged" });
            expectedData.AddRange(new List<string> { "added", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Total Quality Index", "Reporting", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "n/a", "pending", "Avoid instantiations inside loops", "Total Quality Index", "Content", "updated" });
            expectedData.AddRange(new List<string> { "unchanged", "n/a", "pending", "Avoid instantiations inside loops", "Total Quality Index", "Content", "updated" });
            expectedData.AddRange(new List<string> { "unchanged", "n/a", "pending", "Avoid instantiations inside loops", "Total Quality Index", "Content", "updated" });
            expectedData.AddRange(new List<string> { "unchanged", "n/a", "pending", "Avoid instantiations inside loops", "Total Quality Index", "Content", "unchanged" });
            expectedData.AddRange(new List<string> { "added", "n/a", "pending", "Avoid instantiations inside loops", "Total Quality Index", "Content", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 7, 9);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNonCriticalAdded()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" },
                {"BCID","60016" },
                {"FILTER","ADDED" },
                {"VIOLATIONS", "ALL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "PRI", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "added", "336", "added", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.UI.WPF.ViewModel", "unchanged" });
            expectedData.AddRange(new List<string> { "added", "112", "added", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.UI.WPF", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 8, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestCriticalUnchangedModule()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" },
                {"BCID","60016" },
                {"FILTER", "UNCHANGED" },
                {"VIOLATIONS", "CRITICAL" },
                {"MODULE", "JSPBookDemo" }
            };

            Module mod = new Module
            {
                Name = "JSPBookDemo",
                Href = "AED/modules/6/snapshots/6"
            };
            reportData.CurrentSnapshot.Modules = new List<Module> {mod};

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "PRI", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "unchanged", "1,288", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.Reporting.Helper", "unchanged" });
            expectedData.AddRange(new List<string> { "unchanged", "168", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.Mediation.Interfaces", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 8, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNonCriticalAddedUpdated()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.ViolationsListByBC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" },
                {"BCID","60016" },
                {"FILTER","ADDED|UPDATED" },
                {"VIOLATIONS", "ALL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violation Status", "PRI", "Exclusion Status", "Action Status", "Rule Name", "Business criterion name", "Object Name", "Object Status" });
            expectedData.AddRange(new List<string> { "added", "336", "added", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.UI.WPF.ViewModel", "unchanged" });
            expectedData.AddRange(new List<string> { "added", "112", "added", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.UI.WPF", "unchanged" });
            expectedData.AddRange(new List<string> { "updated", "168", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.Reporting.Builder", "unchanged" });
            expectedData.AddRange(new List<string> { "updated", "112", "n/a", "n/a", "Avoid cyclical calls and inheritances between namespaces content", "Security", "CastReporting.Reporting", "unchanged" });
            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

    }
}

