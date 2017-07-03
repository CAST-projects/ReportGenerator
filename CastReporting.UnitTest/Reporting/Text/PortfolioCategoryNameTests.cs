using System.Collections.Generic;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class PortfolioCategoryNameTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        public void TestContent()
        {
            ReportData reportData = TestUtility.PrepaEmptyPortfolioReportData();
            reportData.Category = "UnitTests";

            var component = new PortfolioCategoryName();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("UnitTests", str);
        }

        [TestMethod]
        public void TestNoCategory()
        {
            ReportData reportData = TestUtility.PrepaEmptyPortfolioReportData();

            var component = new PortfolioCategoryName();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("All", str);
        }

        [TestMethod]
        public void TestNoResult()
        {
            var component = new PortfolioCategoryName();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(null, config);
            Assert.AreEqual("n/a", str);
        }

       
    }
}
