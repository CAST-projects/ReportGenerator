using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class IdNameIndicatorMappingTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        public void TestIdNameIndicatorMapping()
        {
            CastDate currentDate = new CastDate {Time = 1492984800000};
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                null, null, null, null, null, null);

            var component = new IdNameIndicatorMapping();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Id" });
            expectedData.AddRange(new List<string> { "TechnicalQualityIndex", "60017"});
            expectedData.AddRange(new List<string> { "Security", "60016" });
            expectedData.AddRange(new List<string> { "Robustness", "60013" });
            expectedData.AddRange(new List<string> { "Performance", "60014" });
            expectedData.AddRange(new List<string> { "Changeability", "60012" });
            expectedData.AddRange(new List<string> { "Transferability", "60011" });
            expectedData.AddRange(new List<string> { "ProgrammingPractices", "66031" });
            expectedData.AddRange(new List<string> { "ArchitecturalDesign", "66032" });
            expectedData.AddRange(new List<string> { "Documentation", "66033" });
            expectedData.AddRange(new List<string> { "SEIMaintainability", "60015" });
            expectedData.AddRange(new List<string> { "CostComplexityDistribution", "67001" });
            expectedData.AddRange(new List<string> { "CyclomaticComplexityDistribution", "65501" });
            expectedData.AddRange(new List<string> { "OOComplexityDistribution", "65701" });
            expectedData.AddRange(new List<string> { "SQLComplexityDistribution", "65801" });
            expectedData.AddRange(new List<string> { "CouplingDistribution", "65350" });
            expectedData.AddRange(new List<string> { "ClassFanOutDistribution", "66020" });
            expectedData.AddRange(new List<string> { "ClassFanInDistribution", "66021" });
            expectedData.AddRange(new List<string> { "SizeDistribution", "65105" });

            TestUtility.AssertTableContent(table, expectedData, 2, 37);
        }
    }
}
