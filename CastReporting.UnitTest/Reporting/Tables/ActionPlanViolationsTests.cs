using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ActionPlanViolationsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
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

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","4" },
                {"NAME","SHORT" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Object Name", "Comment", "Priority", "Status", "Last Updated" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Eval", "do what you can", "low", "added", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "GetCategories", "do what you can", "moderate", "pending", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "GetBackgroundFacts", "do what you can", "extreme", "solved", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "WorksheetAccessorExt", "to do", "high", "added", "Aug 11 2017" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestLongNamesAll()
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

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Object Name", "Comment", "Priority", "Status", "Last Updated" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Cast.Util.ExpressionEvaluator.Eval", "do what you can", "low", "added", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.CastDomainBLL.GetCategories", "do what you can", "moderate", "pending", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "do what you can", "extreme", "solved", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Builder.WorksheetAccessorExt", "to do", "high", "added", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper", "to do", "high", "solved", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.UI.WPF.Utilities.PasswordBoxAssistant", "to do", "high", "pending", "Aug 11 2017" });
            TestUtility.AssertTableContent(table, expectedData, 6, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNoOption()
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

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolations();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Object Name", "Comment", "Priority", "Status", "Last Updated" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Cast.Util.ExpressionEvaluator.Eval", "do what you can", "low", "added", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.CastDomainBLL.GetCategories", "do what you can", "moderate", "pending", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "do what you can", "extreme", "solved", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Builder.WorksheetAccessorExt", "to do", "high", "added", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper", "to do", "high", "solved", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.UI.WPF.Utilities.PasswordBoxAssistant", "to do", "high", "pending", "Aug 11 2017" });
            TestUtility.AssertTableContent(table, expectedData, 6, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestFilterAdded()
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

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"FILTER","ADDED" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Object Name", "Comment", "Priority", "Status", "Last Updated" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Cast.Util.ExpressionEvaluator.Eval", "do what you can", "low", "added", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Builder.WorksheetAccessorExt", "to do", "high", "added", "Aug 11 2017" });
            TestUtility.AssertTableContent(table, expectedData, 6, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestFilterPending()
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

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"FILTER","PENDING" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Object Name", "Comment", "Priority", "Status", "Last Updated" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.CastDomainBLL.GetCategories", "do what you can", "moderate", "pending", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.UI.WPF.Utilities.PasswordBoxAssistant", "to do", "high", "pending", "Aug 11 2017" });
            TestUtility.AssertTableContent(table, expectedData, 6, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestFilterSolved()
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

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"FILTER","SOLVED" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "Object Name", "Comment", "Priority", "Status", "Last Updated" });
            expectedData.AddRange(new List<string> { "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "do what you can", "extreme", "solved", "Aug 11 2017" });
            expectedData.AddRange(new List<string> { "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper", "to do", "high", "solved", "Aug 11 2017" });
            TestUtility.AssertTableContent(table, expectedData, 6,3);
        }

    }
}

