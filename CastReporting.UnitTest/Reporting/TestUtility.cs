using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using CastReporting.Domain;
using CastReporting.Reporting.Languages;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting
{
    [TestClass]
    public static class TestUtility
    {
        public static void SetCulture(string cultureName)
        {
            CultureInfo ci = new CultureInfo(cultureName);
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            Labels.Culture = ci;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleFile"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetSampleResult<T>(string sampleFile) where T : class
        {
            var jsonString = File.ReadAllText(sampleFile);

            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<T>));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));


            return serializer.ReadObject(ms) as IEnumerable<T>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appliName">name of the application</param>
        /// <param name="moduleJson">json file path name containing modules definition for current snapshot</param>
        /// <param name="currentJson">json file path name containing results for the current snapshot</param>
        /// <param name="currentVersion">version for the current snapshot</param>
        /// <param name="currentHref">href for the current snapshot</param>
        /// <param name="currentName">name of the current snapshot</param>
        /// <param name="modulePrevJson">json file path name containing modules definition for previous snapshot</param>
        /// <param name="previousJson">json file path name containing results for the previous snapshot</param>
        /// <param name="previousHref">href for the previous snapshot</param>
        /// <param name="previousName">name for the previous snapshot</param>
        /// <param name="previousVersion">version for the previous snapshot</param>
        /// <returns>ReportData with modules and current and previous snapshots</returns>
        public static ReportData PrepaReportData(string appliName,
            string moduleJson, string currentJson, string currentHref, string currentName, string currentVersion,
            string modulePrevJson, string previousJson, string previousHref, string previousName, string previousVersion)
        {
            ReportData reportData = new ReportData();

            if (currentJson == null) return null;
            // Start Preparation of reportData
            Snapshot currentSnap = new Snapshot
            {
                Name = appliName,
                Href = currentHref,
                Annotation = new Annotation() { Name = currentName, Version = currentVersion }
            };
            if (moduleJson != null) currentSnap.Modules = GetSampleResult<Module>(moduleJson).ToList();
            var qualityIndicatorsResults = GetSampleResult<Result>(currentJson).ToList();
            var businessCriteriaResults = new List<ApplicationResult>();
            var technicalCriteriaResults = new List<ApplicationResult>();
            var qualityRulesResults = new List<ApplicationResult>();
            var sizingMeasures = new List<ApplicationResult>();

            foreach (var appRes in qualityIndicatorsResults.SelectMany(_ => _.ApplicationResults).ToList())
            {
                switch (appRes.Type)
                {
                    case "business-criteria": businessCriteriaResults.Add(appRes); break;
                    case "technical-criteria": technicalCriteriaResults.Add(appRes); break;
                    case "quality-rules": qualityRulesResults.Add(appRes); break;
                    case "technical-size-measures":
                    case "run-time-statistics":
                    case "technical-debt-statistics":
                    case "functional-weight-measures":
                    case "critical-violation-statistics":
                    case "violation-statistics":
                        sizingMeasures.Add(appRes);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            currentSnap.BusinessCriteriaResults = businessCriteriaResults;
            currentSnap.TechnicalCriteriaResults = technicalCriteriaResults;
            currentSnap.QualityRulesResults = qualityRulesResults;
            currentSnap.SizingMeasuresResults = sizingMeasures;

            reportData.CurrentSnapshot = currentSnap;

            if (previousJson == null) return reportData;
            Snapshot previousSnap = new Snapshot
            {
                Name = appliName,
                Href = previousHref,
                Annotation = new Annotation() { Name = previousName, Version = previousVersion }
            };
            if (modulePrevJson != null) previousSnap.Modules = GetSampleResult<Module>(modulePrevJson).ToList();
            var qualityIndicatorsResults2 = GetSampleResult<Result>(previousJson);
            var businessCriteriaResults2 = new List<ApplicationResult>();
            var technicalCriteriaResults2 = new List<ApplicationResult>();
            var qualityRulesResults2 = new List<ApplicationResult>();
            var sizingMeasures2 = new List<ApplicationResult>();

            foreach (var appRes in qualityIndicatorsResults2.SelectMany(_ => _.ApplicationResults).ToList())
            {
                switch (appRes.Type)
                {
                    case "business-criteria": businessCriteriaResults2.Add(appRes); break;
                    case "technical-criteria": technicalCriteriaResults2.Add(appRes); break;
                    case "quality-rules": qualityRulesResults2.Add(appRes); break;
                    case "technical-size-measures":
                    case "run-time-statistics":
                    case "technical-debt-statistics":
                    case "functional-weight-measures":
                    case "critical-violation-statistics":
                    case "violation-statistics":
                        sizingMeasures2.Add(appRes);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            previousSnap.BusinessCriteriaResults = businessCriteriaResults2;
            previousSnap.TechnicalCriteriaResults = technicalCriteriaResults2;
            previousSnap.QualityRulesResults = qualityRulesResults2;
            previousSnap.SizingMeasuresResults = sizingMeasures2;

            reportData.PreviousSnapshot = previousSnap;

            return reportData;
        }

        public static void AssertTableContent(TableDefinition table, List<string> expectedData, int nbCol, int nbRow)
        {
            Assert.AreEqual(nbCol, table.NbColumns);
            Assert.AreEqual(nbRow, table.NbRows);
            Assert.AreEqual(expectedData.Count, table.Data.Count());
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.AreEqual(expectedData[i], table.Data.ElementAt(i));
            }
        }

        public static ReportData PrepareApplicationReportData(string appliName,
            string moduleJson, string currentJson, string currentHref, string currentName, string currentVersion, CastDate currentDate,
            string modulePrevJson, string previousJson, string previousHref, string previousName, string previousVersion, CastDate previousDate)
        {
            ReportData reportData = PrepaReportData(appliName,
                moduleJson, currentJson, currentHref, currentName, currentVersion,
                modulePrevJson, previousJson, previousHref, previousName, previousVersion);

            Application appli = new Application();
            reportData.CurrentSnapshot.Annotation.Date = currentDate;
            reportData.PreviousSnapshot.Annotation.Date = previousDate;
            List<Snapshot> snapshotList = new List<Snapshot> {reportData.CurrentSnapshot, reportData.PreviousSnapshot};
            appli.Snapshots = snapshotList;
            reportData.Application = appli;
            return reportData;

        }

        public static ReportData PrepaPortfolioReportData(string applicationsJSON, List<string> snapshotsJSON, List<string> snapshotsResultsJSON)
        {
            ReportData reportData = new ReportData
            {
                Applications = GetSampleResult<Application>(applicationsJSON).ToArray()
            };
            int i = 0;
            foreach (Application _application in reportData.Applications)
            {
                _application.Snapshots = GetSampleResult<Snapshot>(snapshotsJSON[i]);
                var snapshotResults = GetSampleResult<Result>(snapshotsResultsJSON[i]);
                var businessCriteriaResults = new List<ApplicationResult>();
                var technicalCriteriaResults = new List<ApplicationResult>();
                var qualityRulesResults = new List<ApplicationResult>();
                var sizingMeasures = new List<ApplicationResult>();
                foreach (var appRes in snapshotResults.SelectMany(_ => _.ApplicationResults).ToList())
                {
                    switch (appRes.Type)
                    {
                        case "business-criteria": businessCriteriaResults.Add(appRes); break;
                        case "technical-criteria": technicalCriteriaResults.Add(appRes); break;
                        case "quality-rules": qualityRulesResults.Add(appRes); break;
                        case "technical-size-measures":
                        case "run-time-statistics":
                        case "technical-debt-statistics":
                        case "functional-weight-measures":
                        case "critical-violation-statistics":
                        case "violation-statistics":
                            sizingMeasures.Add(appRes);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
                Snapshot _snapshot = _application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                _snapshot.BusinessCriteriaResults = businessCriteriaResults;
                _snapshot.TechnicalCriteriaResults = technicalCriteriaResults;
                _snapshot.QualityRulesResults = qualityRulesResults;
                _snapshot.SizingMeasuresResults = sizingMeasures;
                
                i++;
            }
            return reportData;
        }

    }
}
