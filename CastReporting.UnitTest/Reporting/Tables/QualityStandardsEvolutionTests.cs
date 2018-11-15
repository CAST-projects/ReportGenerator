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
        public void TestStgTagCWE()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection()
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
                "CWE-22","0","0","0",
                "CWE-78","7","7","5",
                "CWE-79","7","7","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 4);

        }
    }
}

