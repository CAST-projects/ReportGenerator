using System.Collections.Generic;
using CastReporting.Reporting.Block.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class CastComplexityTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestOneSnapshot()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new CastComplexity();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "CAST Complexity", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,881", "n/a", "n/a", "n/a", "86.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1,167", "n/a", "n/a", "n/a", "11.3 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "172", "n/a", "n/a", "n/a", "1.67 %" });
            expectedData.AddRange(new List<string> { "Very High Complexity", "71", "n/a", "n/a", "n/a", "0.69 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastComplexity();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "CAST Complexity", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,881", "8,824", "+57", "+0.65 %", "86.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1,167", "1,140", "+27", "+2.37 %", "11.3 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "172", "170", "+2", "+1.18 %", "1.67 %" });
            expectedData.AddRange(new List<string> { "Very High Complexity", "71", "68", "+3", "+4.41 %", "0.69 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);


        }
    }
}
