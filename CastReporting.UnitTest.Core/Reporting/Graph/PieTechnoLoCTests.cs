using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class PieTechnoLoCTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\TechSizeModTechnoCurrent.json", "Data")]
        public void TestContent()
        {
            /*
             * Configuration : TABLE;GENERIC_GRAPH;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=60014|60017|60013,SNAPSHOTS=CURRENT
             * @".\Data\TechSizeModTechnoCurrent.json" => http://localhost:7070/CAST-RESTAPI/rest/AED/applications/3/snapshots/4/results?sizing-measures=(10151,10152)&modules=($all)&technologies=($all)
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\TechSizeModTechnoCurrent.json", "AED/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL" };
            ReportingParameter repParam = new ReportingParameter {NbResultDefault = 5};
            reportData.Parameter = repParam;

            var component = new PieTechnoLoC();

            var table = component.Content(reportData, null);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Name", "LoC"});
            expectedData.AddRange(new List<string> { "JEE", "89848" });
            expectedData.AddRange(new List<string> { "PL/SQL", "0" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\TechSizeModTechnoCurrent.json", "Data")]
        public void TestContentWithCount()
        {
            /*
             * Configuration : TABLE;GENERIC_GRAPH;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=60014|60017|60013,SNAPSHOTS=CURRENT
             * @".\Data\TechSizeModTechnoCurrent.json" => http://localhost:7070/CAST-RESTAPI/rest/AED/applications/3/snapshots/4/results?sizing-measures=(10151,10152)&modules=($all)&technologies=($all)
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\TechSizeModTechnoCurrent.json", "AED/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL" };
            ReportingParameter repParam = new ReportingParameter { NbResultDefault = 5 };
            reportData.Parameter = repParam;

            var component = new PieTechnoLoC();

            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT", "1"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "LoC" });
            expectedData.AddRange(new List<string> { "JEE", "89848" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }

    }
}
