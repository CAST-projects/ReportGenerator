using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TechnoLoCTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoC();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "LoC"});
            expectedData.AddRange(new List<string> { "SQL Analyzer", "24,670" });
            expectedData.AddRange(new List<string> { ".NET", "24,446" });
            expectedData.AddRange(new List<string> { "JEE", "13,311" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "LoC" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "24,670" });
            expectedData.AddRange(new List<string> { ".NET", "24,446" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestNoSize()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoC();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"NOSIZE","" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name" });
            expectedData.AddRange(new List<string> { "SQL Analyzer" });
            expectedData.AddRange(new List<string> { ".NET"});
            expectedData.AddRange(new List<string> { "JEE" });
            TestUtility.AssertTableContent(table, expectedData, 1, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
