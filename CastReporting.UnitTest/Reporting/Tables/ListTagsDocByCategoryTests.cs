using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ListTagsDocByCategoryTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QualityStandardsCategorySTIGV4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();
            reportData.ServerVersion = "1.11.0.000";
            var component = new ListTagsDocByCategory();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"CAT","STIG-V4R8-CAT1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Standards","Definition","Applicability",
                "No data found","",""
            };

            TestUtility.AssertTableContent(table, expectedData, 3, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QualityStandardsCategorySTIGV4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestTagsDocForOneCategory()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ListTagsDocByCategory();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"CAT","STIG-V4R8-CAT1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Standards","Definition","Applicability",
                "STIG-V-69567","The application must only store cryptographic representations of passwords.","Not applicable",
                "STIG-V-69569","The application must transmit only cryptographically-protected passwords.","Applicable",
                "STIG-V-70149","The application, when utilizing PKI-based authentication, must validate certificates by constructing a certification path (which includes status information) to an accepted trust anchor.","Not applicable",
                "STIG-V-70157","The application must not display passwords/PINs as clear text.","Applicable"
            };

            TestUtility.AssertTableContent(table, expectedData, 3, 5);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QualityStandardsCategorySTIGV4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\QualityStandardsCategorySTIGV4R8CAT2.json", "Data")]
        [DeploymentItem(@".\Data\QualityStandardsCategorySTIGV4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestTagsDocForSeveralCategories()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new ListTagsDocByCategory();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"CAT","STIG-V4R8-CAT1|STIG-V4R8-CAT2|STIG-V4R8-CAT3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Standards","Definition","Applicability",
                "STIG-V4R8-CAT1","","",
                "STIG-V-69567","The application must only store cryptographic representations of passwords.","Not applicable",
                "STIG-V-69569","The application must transmit only cryptographically-protected passwords.","Applicable",
                "STIG-V-70149","The application, when utilizing PKI-based authentication, must validate certificates by constructing a certification path (which includes status information) to an accepted trust anchor.","Not applicable",
                "STIG-V-70157","The application must not display passwords/PINs as clear text.","Applicable",
                "STIG-V4R8-CAT2","","",
                "STIG-V-69241","The application must clear temporary storage and cookies when the session is terminated.","Applicable",
                "STIG-V-69243","The application must automatically terminate the non-privileged user session and log off non-privileged users after a 15 minute idle time period has elapsed.","Applicable",
                "STIG-V4R8-CAT3","","",
                "STIG-V-69249","The application must display an explicit logoff message to users indicating the reliable termination of authenticated communications sessions.","Not applicable"
            };

            TestUtility.AssertTableContent(table, expectedData, 3, 11);

        }

    }
}

