using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class CriticalViolationByModuleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new CriticalViolationByModule();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "TQI", "Robu.", "Efcy", "Secu.", "Trans.", "Chang." });
            expectedData.AddRange(new List<string> { "Current", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "230", "5", "180", "73", "2", "45" });
            expectedData.AddRange(new List<string> { "AED-Admin", "374", "2", "339", "22", "1", "33" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "76", "16", "42", "10", "6", "33" });
            expectedData.AddRange(new List<string> { "Added", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "0", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "AED-Admin", "32", "0", "10", "0", "0", "22" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "16", "2", "8", "2", "0", "8" });
            expectedData.AddRange(new List<string> { "Removed", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "AAD-Admin", "0", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "AED-Admin", "17", "0", "0", "0", "0", "17" });
            expectedData.AddRange(new List<string> { "ReportGenerator", "14", "9", "5", "8", "1", "1" });
            TestUtility.AssertTableContent(table, expectedData, 7, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
