using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using CastReporting.Domain;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Repositories;
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
            try
            {
                IEnumerable<T> res = serializer.ReadObject(ms) as IEnumerable<T>;
                return res;
            }
            finally
            {
                ms.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleFile"></param>
        /// <param name="count"></param>
        /// <param name="propNames"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetCsvSampleResult<T>(string sampleFile, int count, params string[] propNames) where T : new()
        {
            var csvString = File.ReadAllText(sampleFile);

            var serializer = new CsvSerializer<T>();
            return serializer.ReadObjects(csvString, count, propNames);

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
            ReportData reportData = new ReportData
            {
                Parameter = new ReportingParameter(),
                CurrencySymbol = "$"
            };

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

            Application appli = new Application
            {
                Name = appliName,
                Href = currentHref,
                AdgDatabase = "my_central",
                AdgWebSite = "my_adg_website",
                Version = "8.2.4"
            };

            reportData.CurrentSnapshot.Annotation.Date = currentDate;
            List<Snapshot> snapshotList = new List<Snapshot> {reportData.CurrentSnapshot};
            if (reportData.PreviousSnapshot != null)
            {
                reportData.PreviousSnapshot.Annotation.Date = previousDate;
                snapshotList.Add(reportData.PreviousSnapshot);
            }
            
            appli.Snapshots = snapshotList;
            reportData.Application = appli;
            return reportData;

        }

        public static ReportData AddApplicationComplexity(ReportData reportData, string currentJsonComplexity, string previousJsonComplexity)
        {
            if (currentJsonComplexity != null)
            {
                var currentComplexityResultsList = new List<ApplicationResult>();
                currentComplexityResultsList.AddRange(GetSampleResult<Result>(currentJsonComplexity).SelectMany(_ => _.ApplicationResults));
                reportData.CurrentSnapshot.CostComplexityResults = currentComplexityResultsList;
            }
            if (previousJsonComplexity != null)
            {
                var previousComplexityResultsList = new List<ApplicationResult>();
                previousComplexityResultsList.AddRange(GetSampleResult<Result>(previousJsonComplexity).SelectMany(_ => _.ApplicationResults));
                reportData.PreviousSnapshot.CostComplexityResults = previousComplexityResultsList;
            }
            return reportData;
        }

        public static ReportData AddApplicationActionPlan(ReportData reportData, string currentJsonActionPlan, string previousJsonActionPlan)
        {
            if (currentJsonActionPlan != null)
            {
                var currentActionPlanSummary = new List<ActionPlan>();
                currentActionPlanSummary.AddRange(GetSampleResult<ActionPlan>(currentJsonActionPlan));
                reportData.CurrentSnapshot.ActionsPlan = currentActionPlanSummary;
            }
            if (previousJsonActionPlan != null)
            {
                var previousActionPlanSummary = new List<ActionPlan>();
                previousActionPlanSummary.AddRange(GetSampleResult<ActionPlan>(previousJsonActionPlan));
                reportData.PreviousSnapshot.ActionsPlan = previousActionPlanSummary;
            }
            return reportData;
        }

        public static ReportData AddCriticalRuleViolations(ReportData reportData, int bcid, string currentJsonCriticalRuleViolations, string previousJsonCriticalRuleViolations)
        {
            if (currentJsonCriticalRuleViolations != null)
            {
                var currentCriticalRuleViolations = new List<ApplicationResult>();
                var bcres = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == bcid);
                var results = GetSampleResult<Result>(currentJsonCriticalRuleViolations);
                foreach (var _result in results)
                {
                    currentCriticalRuleViolations.AddRange(_result.ApplicationResults);
                }
                if (bcres != null) bcres.CriticalRulesViolation = currentCriticalRuleViolations;
            }
            if (previousJsonCriticalRuleViolations != null)
            {
                var previousCriticalRuleViolations = new List<ApplicationResult>();
                var bcres = reportData.PreviousSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == bcid);
                var results = GetSampleResult<Result>(previousJsonCriticalRuleViolations);
                foreach (var _result in results)
                {
                    previousCriticalRuleViolations.AddRange(_result.ApplicationResults);
                }
                if (bcres != null) bcres.CriticalRulesViolation = previousCriticalRuleViolations;
            }
            return reportData;
        }

        public static ReportData AddSameCriticalRuleViolationsForAllBC(ReportData reportData, string currentJsonCriticalRuleViolations, string previousJsonCriticalRuleViolations)
        {
            int[] bizCrit = { 60011, 60012, 60013, 60014, 60015, 60016, 60017, 66031, 66032, 66033 };
            foreach (int bizId in bizCrit)
            {
                reportData = AddCriticalRuleViolations(reportData, bizId, currentJsonCriticalRuleViolations, previousJsonCriticalRuleViolations);
            }
            return reportData;
        }

        public static ReportData AddNonCriticalRuleViolations(ReportData reportData, int bcid, string currentJsonNonCriticalRuleViolations, string previousJsonNonCriticalRuleViolations)
        {
            if (currentJsonNonCriticalRuleViolations != null)
            {
                var currentNonCriticalRuleViolations = new List<ApplicationResult>();
                var bcres = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == bcid);
                var results = GetSampleResult<Result>(currentJsonNonCriticalRuleViolations);
                foreach (var _result in results)
                {
                    currentNonCriticalRuleViolations.AddRange(_result.ApplicationResults);
                }
                if (bcres != null) bcres.NonCriticalRulesViolation = currentNonCriticalRuleViolations;
            }
            if (previousJsonNonCriticalRuleViolations != null)
            {
                var previousNonCriticalRuleViolations = new List<ApplicationResult>();
                var bcres = reportData.PreviousSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == bcid);
                var results = GetSampleResult<Result>(previousJsonNonCriticalRuleViolations);
                foreach (var _result in results)
                {
                    previousNonCriticalRuleViolations.AddRange(_result.ApplicationResults);
                }
                if (bcres != null) bcres.NonCriticalRulesViolation = previousNonCriticalRuleViolations;
            }
            return reportData;
        }

        public static ReportData AddSameNonCriticalRuleViolationsForAllBC(ReportData reportData, string currentJsonNonCriticalRuleViolations, string previousJsonNonCriticalRuleViolations)
        {
            int[] bizCrit = { 60011, 60012, 60013, 60014, 60015, 60016, 60017, 66031, 66032, 66033 };
            foreach (int bizId in bizCrit)
            {
                reportData = AddNonCriticalRuleViolations(reportData, bizId, currentJsonNonCriticalRuleViolations, previousJsonNonCriticalRuleViolations);
            }
            return reportData;
        }

        public static ReportData AddSizingResults(ReportData reportData, string currentJsonSizingResults, string previousJsonSizingResults)
        {
            if (currentJsonSizingResults != null)
            {
                var results = GetSampleResult<Result>(currentJsonSizingResults).ToList();
                List<ApplicationResult> sizingAppRes = reportData.CurrentSnapshot.SizingMeasuresResults.ToList();
                foreach (var res in results)
                {
                    foreach (var appres in res.ApplicationResults)
                    {
                        var szres = sizingAppRes.FirstOrDefault(_ => _.Reference.Key == appres.Reference.Key);
                        if (szres != null) sizingAppRes.Remove(szres);
                        sizingAppRes.Add(appres);
                    }
                }
                reportData.CurrentSnapshot.SizingMeasuresResults = sizingAppRes;
            }
            if (previousJsonSizingResults != null)
            {
                var results = GetSampleResult<Result>(previousJsonSizingResults).ToList();
                List<ApplicationResult> sizingAppRes = reportData.PreviousSnapshot.SizingMeasuresResults.ToList();
                foreach (var res in results)
                {
                    foreach (var appres in res.ApplicationResults)
                    {
                        var szres = sizingAppRes.FirstOrDefault(_ => _.Reference.Key == appres.Reference.Key);
                        if (szres != null) sizingAppRes.Remove(szres);
                        sizingAppRes.Add(appres);
                    }
                }
                reportData.PreviousSnapshot.SizingMeasuresResults = sizingAppRes;
            }
            return reportData;
        }

        public static ReportData AddQIBusinessCriteriaConfiguration(ReportData reportData, string qiBusinessCriteriaConfJSON)
        {
            if (qiBusinessCriteriaConfJSON == null) return reportData;
            List<QIBusinessCriteria> _qiBizCrit = new List<QIBusinessCriteria>();
            var results = GetSampleResult<QIBusinessCriteria>(qiBusinessCriteriaConfJSON).ToList();
            _qiBizCrit.AddRange(results);
            reportData.CurrentSnapshot.QIBusinessCriterias = _qiBizCrit;
            if (reportData.PreviousSnapshot != null) reportData.PreviousSnapshot.QIBusinessCriterias = _qiBizCrit;
            return reportData;
        }

        public static Snapshot AddSameTechCritRulesViolations(Snapshot snapshot, string jsonFile)
        {
            foreach (ApplicationResult techCritRes in snapshot.TechnicalCriteriaResults)
            {
                techCritRes.RulesViolation = GetSampleResult<Result>(jsonFile).SelectMany(x => x.ApplicationResults).ToList();
            }
            return snapshot;
        }

        public static Snapshot AddTechCritRulesViolations(Snapshot snapshot, string jsonFile, int tcid)
        {
            if (jsonFile == null) return snapshot;
            var results = GetSampleResult<ApplicationResult>(jsonFile).ToList();
            var _firstOrDefault = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == tcid);
            if (_firstOrDefault != null) _firstOrDefault.RulesViolation = results;
            return snapshot;
        }

        public static ReportData AddStandardTags(ReportData reportData, string stdTagsJsonFile)
        {
            reportData.Application.StandardTags = GetSampleResult<StandardTag>(stdTagsJsonFile).ToList();
            return reportData;
        }


        public static ReportData PrepaPortfolioReportData(string applicationsJSON, List<string> snapshotsJSON, List<string> snapshotsResultsJSON)
        {
            ReportData reportData = new ReportData
            {
                Applications = GetSampleResult<Application>(applicationsJSON).ToArray(),
                IgnoresApplications = new string[0],
                IgnoresSnapshots = new string[0],
                Parameter = new ReportingParameter()
            };

            List<Snapshot> snapList = new List<Snapshot>();
            int i = 0;
            foreach (Application _application in reportData.Applications)
            {
                _application.Snapshots = GetSampleResult<Snapshot>(snapshotsJSON[i]);
                var snapshotResults = GetSampleResult<Result>(snapshotsResultsJSON[i]).ToList();
                var _applicationSnapshots = _application.Snapshots.ToList();
                foreach (Snapshot snap in _applicationSnapshots)
                {
                    var businessCriteriaResults = new List<ApplicationResult>();
                    var technicalCriteriaResults = new List<ApplicationResult>();
                    var qualityRulesResults = new List<ApplicationResult>();
                    var sizingMeasures = new List<ApplicationResult>();
                    foreach (var snapAppRes in snapshotResults)
                    {
                        if (snapAppRes.Snapshot.Id != snap.Id) continue;
                        foreach (var appRes in snapAppRes.ApplicationResults)
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
                        snap.BusinessCriteriaResults = businessCriteriaResults;
                        snap.TechnicalCriteriaResults = technicalCriteriaResults;
                        snap.QualityRulesResults = qualityRulesResults;
                        snap.SizingMeasuresResults = sizingMeasures;
                        snapList.Add(snap);
                    }
                }
                i++;
            }
            reportData.Snapshots = snapList.ToArray();
            return reportData;
        }

        public static ReportData PrepaEmptyPortfolioReportData()
        {
            ReportData reportData = new ReportData
            {
                Applications = new Application[0],
                IgnoresApplications = new[] {"Appli Ignor 1", "Appli 2 Ignore"},
                IgnoresSnapshots = new[] {"Snap Ignor 1", "Snap 2 Ignore"}
            };

            return reportData;
        }

        public static void PreparePortfSnapshots(ReportData reportData)
        {
            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-2) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-15) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots1 = new Snapshot[2];
            _snapshots1[0] = _snap0;
            _snapshots1[1] = _snap1;
            reportData.Applications[0].Snapshots = _snapshots1;

            Snapshot _snap2 = reportData.Applications[1].Snapshots.FirstOrDefault();
            TimeSpan time2 = DateTime.Now.AddMonths(-1) - date;
            CastDate _date2 = new CastDate { Time = time2.TotalMilliseconds };
            if (_snap2 == null) Assert.Fail();
            _snap2.Annotation.Date = _date2;

            Snapshot _snap3 = reportData.Applications[1].Snapshots.ElementAt(1);
            TimeSpan time3 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date3 = new CastDate { Time = time3.TotalMilliseconds };
            Debug.Assert(_snap3 != null, "_snap3 != null");
            _snap3.Annotation.Date = _date3;

            Snapshot[] _snapshots2 = new Snapshot[2];
            _snapshots2[0] = _snap2;
            _snapshots2[1] = _snap3;
            reportData.Applications[1].Snapshots = _snapshots2;

            Snapshot[] _snapshots = new Snapshot[4];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            _snapshots[2] = _snap2;
            _snapshots[3] = _snap3;
            reportData.Snapshots = _snapshots;
        }

    }
}
