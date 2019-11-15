using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class HealthFactorTests
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
            var component = new HealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.78", "3.19", "2.59", "3.17", "2.92", "1.93" });
            TestUtility.AssertTableContent(table, expectedData, 7, 2);
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

            var component = new HealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.78", "3.19", "2.59", "3.17", "2.92", "1.93" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.73", "3.15", "2.60", "3.13", "2.82", "1.93" });
            expectedData.AddRange(new List<string> { "% Evol.", "+1.60 %", "+1.46 %", "-0.49 %", "+1.36 %", "+3.48 %", "+0.14 %" });
            TestUtility.AssertTableContent(table, expectedData, 7,4);
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

            var component = new HealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"HEADER", "SHORT"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robu.", "Efcy", "Secu.", "Trans.", "Chang." });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.78", "3.19", "2.59", "3.17", "2.92", "1.93" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.73", "3.15", "2.60", "3.13", "2.82", "1.93" });
            expectedData.AddRange(new List<string> { "% Evol.", "+1.60 %", "+1.46 %", "-0.49 %", "+1.36 %", "+3.48 %", "+0.14 %" });
            TestUtility.AssertTableContent(table, expectedData, 7,4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestShowEvol()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new HealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SHOW_EVOL", "1"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.78", "3.19", "2.59", "3.17", "2.92", "1.93" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.73", "3.15", "2.60", "3.13", "2.82", "1.93" });
            expectedData.AddRange(new List<string> { "Evol.", "+0.04", "+0.05", "-0.01", "+0.04", "+0.10", "+0.00" });
            expectedData.AddRange(new List<string> { "% Evol.", "+1.60 %", "+1.46 %", "-0.49 %", "+1.36 %", "+3.48 %", "+0.14 %" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestShowEvolPercent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new HealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SHOW_EVOL", "1"},
                {"SHOW_EVOL_PERCENT", "0"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.78", "3.19", "2.59", "3.17", "2.92", "1.93" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.73", "3.15", "2.60", "3.13", "2.82", "1.93" });
            expectedData.AddRange(new List<string> { "Evol.", "+0.04", "+0.05", "-0.01", "+0.04", "+0.10", "+0.00" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestDoNotShowEvolNoPercent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new HealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"SHOW_EVOL", "0"},
                {"SHOW_EVOL_PERCENT", "0"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
            expectedData.AddRange(new List<string> { "V-1.5.0_Sprint 2_2", "2.78", "3.19", "2.59", "3.17", "2.92", "1.93" });
            expectedData.AddRange(new List<string> { "V-1.4.1", "2.73", "3.15", "2.60", "3.13", "2.82", "1.93" });
            TestUtility.AssertTableContent(table, expectedData, 7, 4);

        }

    }
}
