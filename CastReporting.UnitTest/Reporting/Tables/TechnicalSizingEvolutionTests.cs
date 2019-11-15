using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TechnicalSizingEvolutionTests
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

            var component = new TechnicalSizingEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "Evolution", "% Evolution"});
            expectedData.AddRange(new List<string> { "LoC", "62,427", "54,168", "+8,259", "+15.3 %" });
            expectedData.AddRange(new List<string> { "   Files", "476", "375", "+101", "+26.9 %" });
            expectedData.AddRange(new List<string> { "   Classes", "351", "291", "+60", "+20.6 %" });
            expectedData.AddRange(new List<string> { "SQL Art.", "553", "550", "+3", "+0.55 %" });
            expectedData.AddRange(new List<string> { "   Tables", "349", "345", "+4", "+1.16 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 6);
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

            var component = new TechnicalSizingEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Current", "Previous", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "LoC", "62,427", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "   Files", "476", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "   Classes", "351", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "SQL Art.", "553", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "   Tables", "349", "n/a", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 5, 6);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
