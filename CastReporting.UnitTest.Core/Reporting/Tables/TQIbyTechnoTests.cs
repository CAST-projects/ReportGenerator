using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TQIbyTechnoTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCtechno.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
               null, @".\Data\CurrentBCtechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };
            var component = new TQIbyTechno();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Techno", "TQI" });
            expectedData.AddRange(new List<string> { ".NET", "2.79" });
            expectedData.AddRange(new List<string> { "JEE", "2.98" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "2.79" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCtechno.json", "Data")]
        public void TestAnotherBC()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentBCtechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TQIbyTechno();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60016" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Techno", "Security" });
            expectedData.AddRange(new List<string> { ".NET", "3.21" });
            expectedData.AddRange(new List<string> { "JEE", "3.31" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "3.12" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
