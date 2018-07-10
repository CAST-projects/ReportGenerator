using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class QualityRuleViolationsBookmarksTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        // TODO :
        // - test with no count / count = 2
        // - test with shortname / fullname
        // - test with current / previous snapshot
        // - test with only one snapshot (previous does not exists= => no status column)

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestOneSnapshotShortNames()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","1" },
                {"NAME","SHORT" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "Violation #1",
                "Object Name: adg_central_grades_std",
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

            TestUtility.AssertTableContent(table, expectedData, 1, 27);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(26, cellsProperties.Count);
            Assert.AreEqual(Color.Gainsboro, cellsProperties[0].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[1].BackgroundColor);
            Assert.AreEqual(Color.Lavender, cellsProperties[2].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[3].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[4].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[5].BackgroundColor);
            Assert.AreEqual(Color.LightYellow, cellsProperties[6].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[7].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[8].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[9].BackgroundColor);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestOneSnapshotFullNames()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","1" },
                {"NAME","FULL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "Violation #1",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
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

            TestUtility.AssertTableContent(table, expectedData, 1, 27);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(26, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestTwoSnapshotNoCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);
            WSConnection connection = new WSConnection()
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"NAME","SHORT" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(136, table.NbRows);
            Assert.AreEqual("Violation #1", table.Data.ElementAt(1));
            Assert.AreEqual("Object Name: adg_central_grades_std", table.Data.ElementAt(2));
            Assert.AreEqual("Status: added", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #4", table.Data.ElementAt(82));
            Assert.AreEqual("Object Name: adg_m_central_grades_std", table.Data.ElementAt(83));
            Assert.AreEqual("Status: updated", table.Data.ElementAt(84));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(135, cellsProperties.Count);
            Assert.AreEqual(Color.Gainsboro, cellsProperties[0].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[1].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[2].BackgroundColor);
            Assert.AreEqual(Color.Lavender, cellsProperties[3].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[4].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[5].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[6].BackgroundColor);
            Assert.AreEqual(Color.LightYellow, cellsProperties[7].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[8].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[9].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[10].BackgroundColor);

            Assert.AreEqual(Color.Gainsboro, cellsProperties[81].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[82].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[83].BackgroundColor);
            Assert.AreEqual(Color.Lavender, cellsProperties[84].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[85].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[85].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[87].BackgroundColor);
            Assert.AreEqual(Color.LightYellow, cellsProperties[88].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[89].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[90].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[91].BackgroundColor);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestAllViolations()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"NAME","SHORT" },
                {"COUNT","-1" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(183, table.NbRows);
            Assert.AreEqual("Violation #1", table.Data.ElementAt(1));
            Assert.AreEqual("Object Name: adg_central_grades_std", table.Data.ElementAt(2));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #7", table.Data.ElementAt(157));
            Assert.AreEqual("Object Name: adgc_delta_debt_removed", table.Data.ElementAt(158));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java", table.Data.ElementAt(175));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(182, cellsProperties.Count);
            Assert.AreEqual(Color.Gainsboro, cellsProperties[0].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[1].BackgroundColor);
            Assert.AreEqual(Color.Lavender, cellsProperties[2].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[3].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[4].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[5].BackgroundColor);
            Assert.AreEqual(Color.LightYellow, cellsProperties[6].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[7].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[8].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[9].BackgroundColor);

            Assert.AreEqual(Color.Gainsboro, cellsProperties[156].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[157].BackgroundColor);

            Assert.AreEqual(Color.Lavender, cellsProperties[174].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[175].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[176].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[177].BackgroundColor);
            Assert.AreEqual(Color.LightYellow, cellsProperties[178].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[179].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[180].BackgroundColor);
            Assert.AreEqual(Color.White, cellsProperties[181].BackgroundColor);

        }
    }
}

