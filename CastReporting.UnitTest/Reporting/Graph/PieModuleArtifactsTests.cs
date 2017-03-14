using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class PieModuleArtifactsTests
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

            ReportingParameter repParam = new ReportingParameter {NbResultDefault = 5};
            reportData.Parameter = repParam;

            var component = new PieModuleArtifact();

            var table = component.Content(reportData, null);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> {"Name", "Artifacts"});
            expectedData.AddRange(new List<string> { "sm-core/AppliAEPtran/Shopizer_src content", "5131" });
            expectedData.AddRange(new List<string> { "sm-central/AppliAEPtran/Shopizer_src content", "4344" });
            expectedData.AddRange(new List<string> { "sm-shop/AppliAEPtran/Shopizer_src content", "801" });
            expectedData.AddRange(new List<string> { "SHOPIZER/AppliAEPtran/Shopizer_sql content", "15" });
            TestUtility.AssertTableContent(table, expectedData, 2, 5);
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

            ReportingParameter repParam = new ReportingParameter { NbResultDefault = 5 };
            reportData.Parameter = repParam;

            var component = new PieModuleArtifact();

            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT", "2"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Artifacts" });
            expectedData.AddRange(new List<string> { "sm-core/AppliAEPtran/Shopizer_src content", "5131" });
            expectedData.AddRange(new List<string> { "sm-central/AppliAEPtran/Shopizer_src content", "4344" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

    }
}
