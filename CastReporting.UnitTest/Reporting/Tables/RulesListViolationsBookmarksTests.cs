using System.Collections.Generic;
using System.Drawing;
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
        // - test with no count / count = 2
        // - test for metrics by standard tag / bc / tc / metric ids

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
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


            };

            TestUtility.AssertTableContent(table, expectedData, 1, 75);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(70, cellsProperties.Count);
            Assert.AreEqual(Color.Gray, cellsProperties[0].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[1].BackgroundColor);
            Assert.AreEqual(Color.LightGray, cellsProperties[2].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[3].BackgroundColor);
            Assert.AreEqual(Color.LightGray, cellsProperties[4].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[5].BackgroundColor);
            Assert.AreEqual(Color.LightGray, cellsProperties[6].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[7].BackgroundColor);
            Assert.AreEqual(Color.Gainsboro, cellsProperties[8].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[9].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[10].BackgroundColor);
            Assert.AreEqual(Color.Lavender, cellsProperties[11].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[12].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[13].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[14].BackgroundColor);
            Assert.AreEqual(Color.LightYellow, cellsProperties[15].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[16].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[17].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[18].BackgroundColor);

            Assert.AreEqual(Color.Gray, cellsProperties[35].BackgroundColor);
        }

    }
}

