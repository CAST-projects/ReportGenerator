using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesListViolationsBookmarksTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        // TODO :
        // - test with no count / count = 1
        // - test for metrics by standard tag / bc / tc / metric ids

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestTCmetrics()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","61028" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Violations",
                "",
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "# Violations: 86",
                "Rationale: ",
                "Having an SQL query inside a loop is usually the source of performance and scalability problems especially if the number of iterations become very high (for example if it is dependent on the data returned from the database).\nThis iterative pattern has proved to be very dangerous for application performance and scalability. Database servers perform much better in set-oriented patterns rather than pure iterative ones.",
                "Description: ",
                "This metric retrieves all artifacts using at least one SQL query inside a loop statement.",
                "Remediation: ",
                "The remediation is often to replace the iterative approach based on a loop with a set-oriented one and thus modify the query.",
                "",
                "Violation #1    Avoid using SQL queries inside a loop",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "",
                "",
                "",
                "",
                "",
                "Objects in violation for rule Avoid Methods with a very low comment/code ratio",
                "# Violations: 128",
                "Rationale: ",
                "Maintainability of the code is facilitated if there is documentation in the code. This rule will ensure there are comments within the Artifact",
                "Description: ",
                "Methods should have at least a ratio comment/code > X %\nThe threshold is a parameter and can be changed at will.",
                "Remediation: ",
                "Enrich Artifact code with comments",
                "",
                "Violation #1    Avoid Methods with a very low comment/code ratio",
                "Object Name: com.castsoftware.aad.common.AadCommandLine.dumpStack",
                "Object Type: MyObjType",
                "File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp",
                "4904 :      m_bGridModified = FALSE;",
                "4905 :  }",
                "4906 : ",
                "4907 :  void CMetricTreePageDet::Validate()",
                "4908 :  {",
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;",
                "",
                "",
                "",
                ""
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 66);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(53, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestNocountMetric()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","7846" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(69, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Avoid Methods with a very low comment/code ratio", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 128", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #5    Avoid Methods with a very low comment/code ratio", table.Data.ElementAt(55));
            Assert.AreEqual("Object Name: com.castsoftware.aed.common.AedCommandLine.getFormattedMsg", table.Data.ElementAt(56));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(57));
            Assert.AreEqual("File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp", table.Data.ElementAt(58));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(58, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestMetricsStdTag()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","CWE" },
                {"COUNT", "-1" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(210, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Avoid using SQL queries inside a loop", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 86", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #7    Avoid using SQL queries inside a loop", table.Data.ElementAt(179));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adgc_delta_debt_removed", table.Data.ElementAt(180));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(181));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql", table.Data.ElementAt(182));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java", table.Data.ElementAt(198));
            Assert.AreEqual("1203 :         }", table.Data.ElementAt(205));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(197, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestBCmetrics()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(206, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Action Mappings should have few forwards", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 77", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #1    Action Mappings should have few forwards", table.Data.ElementAt(9));
            Assert.AreEqual("Violation #2    Action Mappings should have few forwards", table.Data.ElementAt(37));
            Assert.AreEqual("Objects in violation for rule Avoid accessing data by using the position and length", table.Data.ElementAt(69));
            Assert.AreEqual("# Violations: 6", table.Data.ElementAt(70));
            Assert.AreEqual("Objects in violation for rule Avoid artifacts having recursive calls", table.Data.ElementAt(138));
            Assert.AreEqual("# Violations: 12", table.Data.ElementAt(139));
            Assert.AreEqual("1203 :         }", table.Data.ElementAt(201));


            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(184, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestBCmetricsCritical()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"CRITICAL","true" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(68, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Action Mappings should have few forwards", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 77", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #1    Action Mappings should have few forwards", table.Data.ElementAt(9));
            Assert.AreEqual("Violation #2    Action Mappings should have few forwards", table.Data.ElementAt(37));


            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(60, cellsProperties.Count);
        }

    }
}

