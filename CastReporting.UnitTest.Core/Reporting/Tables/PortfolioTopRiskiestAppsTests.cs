using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class PortfolioTopRiskiestAppsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24Snap12Results.json", "Data")]
        public void TestContentEfficiency()
        {
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3Snap4Results.json", @".\Data\AAD2App24Snap12Results.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioTopRiskiestApps();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ALT","60014" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Application", "Critical Violations", "Efficiency", "Snapshot Date" });
            expectedData.AddRange(new List<string> { "Big Ben", "151", "1.32", "Nov 15 2013" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "198", "1.88", "Jul 13 2016" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24Snap12Results.json", "Data")]
        public void TestContentNoBcid()
        {
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3Snap4Results.json", @".\Data\AAD2App24Snap12Results.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioTopRiskiestApps();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Application", "Critical Violations", "TQI", "Snapshot Date" });
            expectedData.AddRange(new List<string> { "Big Ben", "616", "2.23", "Nov 15 2013" });
            expectedData.AddRange(new List<string> { "AppliAEPtran", "1,560", "2.52", "Jul 13 2016" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication2Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App24Snap12Results.json", "Data")]
        public void TestContentLimitCount()
        {
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3Snap4Results.json", @".\Data\AAD2App24Snap12Results.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioTopRiskiestApps();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "ALT", "60017"},
                {"COUNT", "1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Application", "Critical Violations", "TQI", "Snapshot Date" });
            expectedData.AddRange(new List<string> { "Big Ben", "616", "2.23", "Nov 15 2013" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
