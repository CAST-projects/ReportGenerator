using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TechnoLoCEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        [DeploymentItem(@".\Data\PreviousTechSizeResultsModTechno.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, @".\Data\PreviousTechSizeResultsModTechno.json", "AED/applications/3/snapshots/3", "Snap3_CAIP-8.2.4_RG-1.4.1", "8.2.4", previousDate);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };
            reportData.PreviousSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoCEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current LoC", "Previous LoC", "Evolution", "% Evolution"});
            expectedData.AddRange(new List<string> { "SQL Analyzer", "24,670", "24,440", "+230", "+0.94 %" });
            expectedData.AddRange(new List<string> { ".NET", "24,446", "18,012", "+6,434", "+35.7 %" });
            expectedData.AddRange(new List<string> { "JEE", "13,311", "11,716", "+1,595", "+13.6 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestNoPrevious()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoCEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current LoC", "Previous LoC", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "24,670", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { ".NET", "24,446", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "JEE", "13,311", "n/a", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        [DeploymentItem(@".\Data\PreviousTechSizeResultsModTechno.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, @".\Data\PreviousTechSizeResultsModTechno.json", "AED/applications/3/snapshots/3", "Snap3_CAIP-8.2.4_RG-1.4.1", "8.2.4", previousDate);
            reportData.CurrentSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };
            reportData.PreviousSnapshot.Technologies = new[] { ".NET", "JEE", "SQL Analyzer" };

            var component = new TechnoLoCEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT", "1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current LoC", "Previous LoC", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "SQL Analyzer", "24,670", "24,440", "+230", "+0.94 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
