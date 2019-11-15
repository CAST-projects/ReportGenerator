using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class CriteriaAndGradeTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QIBusinessCriteriaConf.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData = TestUtility.AddQIBusinessCriteriaConfiguration(reportData, @".\Data\QIBusinessCriteriaConf.json");

            var component = new CriteriaAndGrade();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "60011" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical criterion name", "Grade" });
            expectedData.AddRange(new List<string> { "Architecture - Object-level Dependencies", "2.93" });
            expectedData.AddRange(new List<string> { "Complexity - Algorithmic and Control Structure Complexity", "3.44" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QIBusinessCriteriaConf.json", "Data")]
        public void TestTwoSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddQIBusinessCriteriaConfiguration(reportData, @".\Data\QIBusinessCriteriaConf.json");

            var component = new CriteriaAndGrade();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "60016" },
                {"COUNT", "3" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical criterion name", "Grade", "Evolution" });
            expectedData.AddRange(new List<string> { "Architecture - Architecture Models Automated Checks", "3.56", "+32.3 %" });
            expectedData.AddRange(new List<string> { "Architecture - Multi-Layers and Data Access", "1.00", "0 %" });
            expectedData.AddRange(new List<string> { "Architecture - OS and Platform Independence", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\QIBusinessCriteriaConf.json", "Data")]
        public void TestNoCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddQIBusinessCriteriaConfiguration(reportData, @".\Data\QIBusinessCriteriaConf.json");

            var component = new CriteriaAndGrade();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66033"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical criterion name", "Grade", "Evolution" });
            expectedData.AddRange(new List<string> { "Documentation - Automated Documentation", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Documentation - Bad Comments", "4.00", "+22.3 %" });
            expectedData.AddRange(new List<string> { "Documentation - Naming Convention Conformity", "3.67", "+1.66 %" });
            expectedData.AddRange(new List<string> { "Documentation - Style Conformity", "1.59", "-1.85 %" });
            expectedData.AddRange(new List<string> { "Documentation - Volume of Comments", "1.36", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 3, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
