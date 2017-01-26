using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {

            const double pValue = 1.1401;
            // ReSharper disable once UnreachableCode
            const string sign = (pValue > 0) ? "+" : "";
            var roundedValue = Math.Round(pValue, 4);
            NumberFormatInfo nfi = (NumberFormatInfo) CultureInfo.CurrentCulture.NumberFormat.Clone();
            var tmp = roundedValue*100;
            nfi.PercentDecimalDigits = (Math.Abs(tmp%1) < 0 || tmp >= 100) ? 0 : (tmp >= 0.1) ? 2 : 1;
            // ReSharper disable once UnusedVariable
            var r = sign + roundedValue.ToString("P", nfi);

        }

        [TestMethod]
        public void TestMatrice()
        {
            var key = Tuple.Create(1234, "JEE", 60017);
            var values = new Dictionary<Tuple<int, string, int>, double> {[key] = 2.35};

            var key2 = Tuple.Create(1234, "JEE", 60017);
            Console.WriteLine(values[key]);
            Console.WriteLine(values[key2]);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Modules1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestContent()
        {
            ReportData reportData = new ReportData();
            Snapshot currentSnap = new Snapshot
            {
                Name = "AppliAEP",
                Href = "AED3/applications/3/snapshots/4",
                Annotation = new Annotation() { Name = "Snap_v1.1.4", Version = "v1.1.4" }
            };

            var jsonString = File.ReadAllText(@".\Data\Modules1.json");
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Module>));
            currentSnap.Modules = serializer.ReadObject(ms) as IEnumerable<Module>;
            Assert.AreEqual(4, currentSnap.Modules?.Count());

            var jsonString2 = File.ReadAllText(@".\Data\Snapshot_QIresults1.json");
            MemoryStream ms2 = new MemoryStream(Encoding.Unicode.GetBytes(jsonString2));
            serializer = new DataContractJsonSerializer(typeof(IEnumerable<Result>));
           var qualityIndicatorsResults = serializer.ReadObject(ms2) as IEnumerable<Result>;

            var businessCriteriaResults = new List<ApplicationResult>();
            var technicalCriteriaResults = new List<ApplicationResult>();
            var qualityRulesResults = new List<ApplicationResult>();

            foreach (var appRes in qualityIndicatorsResults?.SelectMany(_ => _.ApplicationResults)?.ToList())
            {
                switch (appRes.Type)
                {
                    case "business-criteria": businessCriteriaResults.Add(appRes); break;
                    case "technical-criteria": technicalCriteriaResults.Add(appRes); break;
                    case "quality-rules": qualityRulesResults.Add(appRes); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            currentSnap.BusinessCriteriaResults = businessCriteriaResults;
            Assert.AreEqual(6, currentSnap.BusinessCriteriaResults?.Count());
            currentSnap.TechnicalCriteriaResults = technicalCriteriaResults;
            Assert.AreEqual(3, currentSnap.TechnicalCriteriaResults?.Count());
            currentSnap.QualityRulesResults = qualityRulesResults;
            Assert.AreEqual(4, currentSnap.QualityRulesResults?.Count());

            Snapshot previousSnap = new Snapshot
            {
                Name = "AppliAEP",
                Href = "AED3/applications/3/snapshots/3",
                Annotation = new Annotation() { Name = "Snap_v1.1.3", Version = "v1.1.3" }
            };
            previousSnap.Modules = currentSnap.Modules;

            var jsonString3 = File.ReadAllText(@".\Data\Snapshot_QIresults2.json");
            MemoryStream ms3 = new MemoryStream(Encoding.Unicode.GetBytes(jsonString3));
            serializer = new DataContractJsonSerializer(typeof(IEnumerable<Result>));
            var qualityIndicatorsResults2 = serializer.ReadObject(ms3) as IEnumerable<Result>;

            var businessCriteriaResults2 = new List<ApplicationResult>();
            var technicalCriteriaResults2 = new List<ApplicationResult>();
            var qualityRulesResults2 = new List<ApplicationResult>();

            foreach (var appRes in qualityIndicatorsResults2?.SelectMany(_ => _.ApplicationResults)?.ToList())
            {
                switch (appRes.Type)
                {
                    case "business-criteria": businessCriteriaResults2.Add(appRes); break;
                    case "technical-criteria": technicalCriteriaResults2.Add(appRes); break;
                    case "quality-rules": qualityRulesResults2.Add(appRes); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            previousSnap.BusinessCriteriaResults = businessCriteriaResults2;
            Assert.AreEqual(6, previousSnap.BusinessCriteriaResults?.Count());
            previousSnap.TechnicalCriteriaResults = technicalCriteriaResults2;
            Assert.AreEqual(3, previousSnap.TechnicalCriteriaResults?.Count());
            previousSnap.QualityRulesResults = qualityRulesResults2;
            Assert.AreEqual(4, previousSnap.QualityRulesResults?.Count());

            reportData.CurrentSnapshot = currentSnap;
            reportData.PreviousSnapshot = previousSnap;

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add("COL1","METRICS");
            config.Add("ROW1","SNAPSHOTS");
            config.Add("METRICS","HEALTH_FACTOR");
            config.Add("SNAPSHOTS","CURRENT");
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", "3.12", "2.98", "2.55", "1.88", "1.70" });
            Assert.AreEqual(6, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);
            Assert.AreEqual(expectedData.Count, table.Data.Count());
            for(int i=0; i < expectedData.Count; i++)
            {
                Assert.AreEqual(expectedData[i], table.Data.ElementAt(i));
            }

        }
    }
}
