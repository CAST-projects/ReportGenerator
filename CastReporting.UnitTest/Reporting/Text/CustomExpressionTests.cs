using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class CustomExpressionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestCurrentContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new CustomExpression();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PARAMS", "QR a QR b"},
                {"EXPR", "(a+b)/2"},
                {"a","60012"},
                {"b", "60013"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("2.56", str);

            Dictionary<string, string> config2 = new Dictionary<string, string>
            {
                {"PARAMS", "QR a QR b"},
                {"EXPR", "(a+b)/2"},
                {"a","60012"},
                {"b", "60013"},
                {"SNAPSHOT", "CURRENT" }
            };
            var str2 = component.Content(reportData, config2);
            Assert.AreEqual("2.56", str2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestPreviousContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);

            var component = new CustomExpression();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PARAMS", "QR a QR b"},
                {"EXPR", "(a+b)/2"},
                {"a","60012"},
                {"b", "60013"},
                {"SNAPSHOT", "PREVIOUS" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("2.54", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestSzContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new CustomExpression();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PARAMS", "SZ a SZ b"},
                {"EXPR", "b/a"},
                {"a","10151"},
                {"b", "68001"},
                {"FORMAT", "N0" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("11", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\BusinessValue.json", "Data")]
        public void TestBfContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CustomExpression();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PARAMS", "BF a SZ b"},
                {"EXPR", "b/a"},
                {"a","66061"},
                {"b", "10151"},
                {"FORMAT", "N0" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("7,087", str);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\BusinessValue.json", "Data")]
        public void TestErrorContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new CustomExpression();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PARAMS", "BC a BC b"},
                {"EXPR", "(a+b)/2"},
                {"a","60012"},
                {"b", "61111"}
            };
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var str = component.Content(reportData, config);
            Assert.AreEqual("No data found", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestDivideByZero()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new CustomExpression();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PARAMS", "SZ a"},
                {"EXPR", "b/0"},
                {"a","10151"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("No data found", str);
        }
    }
}
