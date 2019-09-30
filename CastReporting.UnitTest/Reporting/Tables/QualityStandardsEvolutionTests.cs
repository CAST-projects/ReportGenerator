using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class QualityStandardsEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWE()
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

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-2011-Top25","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        public void TestStgTagDetailSTIG()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8" },
                {"MORE","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8", "Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "STIG-V4R8-CAT1 ","0","0","0",
                "    STIG-V-70245 The application must protect the confidentiality and integrity of transmitted information.","0","0","0",
                "    STIG-V-70261 The application must protect from command injection.","0","0","0",
                "STIG-V4R8-CAT3 ","589","31","6",
                "    STIG-V-70385 The application development team must follow a set of coding standards.","589","31","6"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 6);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWEHeaders()
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

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" },
                {"LBL","violations" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-2011-Top25","Total Violations","Added Violations","Removed Violations",
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
            reportData.ServerVersion = "1.10.0.000";
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8" },
                {"MORE","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8", "Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "No data found","","",""
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 2);

        }

    }
}

