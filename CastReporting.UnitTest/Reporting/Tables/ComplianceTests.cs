using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ComplianceTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new Compliance();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "", "Programming Practices", "Architectural Design", "Documentation"});
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.93", "2.10", "2.65" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestTwoSnapshots()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new Compliance();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "", "Programming Practices", "Architectural Design", "Documentation" });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.93", "2.10", "2.65" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.91", "2.10", "2.47" });
            expectedData.AddRange(new List<string> { "Variation", "+0.88 %", "-0.20 %", "+7.60 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestShortHeader()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new Compliance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"HEADER", "SHORT"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "", "Prog.", "Arch.", "Doc." });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.93", "2.10", "2.65" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.91", "2.10", "2.47" });
            expectedData.AddRange(new List<string> { "Variation", "+0.88 %", "-0.20 %", "+7.60 %" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);

        }
    }
}
