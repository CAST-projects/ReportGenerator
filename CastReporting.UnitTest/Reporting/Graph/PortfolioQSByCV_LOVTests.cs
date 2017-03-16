using System.Collections.Generic;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class PortfolioQSByCV_LOVTests
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
        public void TestContentTwoApps()
        {
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json", @".\Data\AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3Snap4Results.json", @".\Data\AAD2App24Snap12Results.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADApplications.json", snapList, snapResultsList);

            var component = new PortfolioQsbyCvLov();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TQI", "Critical Violations/kLoC", "Automated Function Points", "Application" });
            expectedData.AddRange(new List<string> { "2.52", "17.36", "1455", "AppliAEPtran" });
            expectedData.AddRange(new List<string> { "2.23", "2.81", "2781", "Big Ben" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADOneApplication.json", "Data")]
        [DeploymentItem(@".\Data\AADApplication1Snap.json", "Data")]
        [DeploymentItem(@".\Data\AAD2App3Snap4Results.json", "Data")]
        public void TestContentOneApp()
        {
            List<string> snapList = new List<string> { @".\Data\AADApplication1Snap.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AAD2App3Snap4Results.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADOneApplication.json", snapList, snapResultsList);

            var component = new PortfolioQsbyCvLov();
            var table = component.Content(reportData, null);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TQI", "Critical Violations/kLoC", "Automated Function Points", "Application" });
            expectedData.AddRange(new List<string> { "2.52", "17.36", "1455", "AppliAEPtran" });
            expectedData.AddRange(new List<string> { "0", "0", "0", "" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }
    }
}
