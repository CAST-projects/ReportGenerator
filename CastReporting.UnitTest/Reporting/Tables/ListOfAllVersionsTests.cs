using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ListOfAllVersionsTests
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
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new ListOfAllVersions();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshot Label", "Snapshot Date" });
            expectedData.AddRange(new List<string> { "8.3.ra", "Apr 24 2017" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTCmodules.json", "Data")]
        public void TestTwoSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            CastDate previousDate = new CastDate { Time = 1492380000000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               @".\Data\ModulesCoCRA.json", @".\Data\PreviousBCTCmodules.json", "AED/applications/3/snapshots/3", "Snap3_CAIP-8.2.4_RG-1.4.1", "8.2.4", previousDate);

            var component = new ListOfAllVersions();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshot Label", "Snapshot Date" });
            expectedData.AddRange(new List<string> { "8.3.ra", "Apr 24 2017" });
            expectedData.AddRange(new List<string> { "8.2.4", "Apr 17 2017" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ModulesCoCRA.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTCmodules.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTCmodules.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            CastDate previousDate = new CastDate { Time = 1492380000000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                @".\Data\ModulesCoCRA.json", @".\Data\CurrentBCTCmodules.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
                @".\Data\ModulesCoCRA.json", @".\Data\PreviousBCTCmodules.json", "AED/applications/3/snapshots/3", "Snap3_CAIP-8.2.4_RG-1.4.1", "8.2.4", previousDate);

            var component = new ListOfAllVersions();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshot Label", "Snapshot Date" });
            expectedData.AddRange(new List<string> { "8.3.ra", "Apr 24 2017" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
