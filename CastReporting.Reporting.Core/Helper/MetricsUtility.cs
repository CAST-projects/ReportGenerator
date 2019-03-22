using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting
{
    public static class MetricsUtility
    {
        private const string ColorGainsboro = "Gainsboro";
        private const string ColorWhite = "White";
        private const string ColorLavander = "Lavender";
        private const string ColorLightYellow = "LightYellow";

        /// <summary>
        /// This method return the name of the metric from getting it in the results of the snapshot
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        public static string GetMetricName(ReportData reportData, Snapshot snapshot, string metricId)
        {
            string name = ((snapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault() ??
                            snapshot.TechnicalCriteriaResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault()) ??
                           snapshot.QualityRulesResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault()) ??
                          snapshot.SizingMeasuresResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
            if (snapshot.QualityRulesResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault() != null)
            {
                name = name + " (" + metricId + ")";
            }
            if (name != null) return name;
            var bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, metricId, true, true).FirstOrDefault();
            if (bfResult == null || !bfResult.ApplicationResults.Any()) return Constants.No_Value;
            name = bfResult.ApplicationResults[0].Reference.Name ?? Constants.No_Value;
            return name;
        }

        /// <summary>
        /// This method return the name, type and result (grade or value) of the metric from getting it in the results of the snapshot
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <param name="module"></param>
        /// <param name="technology"></param>
        /// <param name="format"></param> should be false for graph component
        /// <returns></returns>
        public static SimpleResult GetMetricNameAndResult(ReportData reportData, Snapshot snapshot, string metricId, Module module, string technology, bool format)
        {
            metricType type = metricType.NotKnown;
            Result bfResult = null;
            double? result = null;
            string resStr = string.Empty;

            string name = snapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
            if (name != null) type = metricType.BusinessCriteria;
            // if metricId is not a Business Criteria
            if (name == null)
            {
                name = snapshot.TechnicalCriteriaResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                if (name != null) type = metricType.TechnicalCriteria;
            }
            // if metricId is not a technical criteria
            if (name == null)
            {
                name = snapshot.QualityRulesResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                if (name != null) type = metricType.QualityRule;
            }
            // if metricId is not a quality rule
            if (name == null)
            {
                name = snapshot.SizingMeasuresResults.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                if (name != null) type = metricType.SizingMeasure;
            }
            // if metricId is not a sizing measure, perhaps a background fact
            if (name == null)
            {
                bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, metricId, true, true).FirstOrDefault();
                if (bfResult == null || !bfResult.ApplicationResults.Any()) return null;
                name = bfResult.ApplicationResults[0].Reference.Name;
                if (name != null) type = metricType.BackgroundFact;
            }
            // we don't know what is this metric
            if (name == null) return null;

            switch (type)
            {
                case metricType.BusinessCriteria:
                    if (module == null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                    }
                    else if (module != null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module == null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module != null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    resStr = result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                    break;
                case metricType.TechnicalCriteria:
                    if (module == null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                    }
                    else if (module != null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module == null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module != null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    resStr = result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                    break;
                case metricType.QualityRule:
                    if (module == null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                    }
                    else if (module != null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module == null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module != null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    resStr = result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                    
                    break;
                case metricType.SizingMeasure:
                    if (module == null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Value).FirstOrDefault();
                    }
                    else if (module != null && string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Value;
                    }
                    else if (module == null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Value;
                    }
                    else if (module != null && !string.IsNullOrEmpty(technology))
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Value;
                    }
                    resStr = format ? result?.ToString("N0") ?? Constants.No_Value : result?.ToString() ?? "0";
                    break;
                case metricType.BackgroundFact:
                    if (module == null && string.IsNullOrEmpty(technology))
                    {
                        result = bfResult?.ApplicationResults[0].DetailResult.Value;
                    }
                    else if (module != null && string.IsNullOrEmpty(technology))
                    {
                        result = bfResult?.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id)?
                            .DetailResult.Value;
                    }
                    else if (module == null && !string.IsNullOrEmpty(technology))
                    {
                        result = bfResult?.ApplicationResults[0].TechnologyResult.FirstOrDefault(_ => _.Technology == technology)?
                            .DetailResult.Value;
                    }
                    else if (module != null && !string.IsNullOrEmpty(technology))
                    {
                        result = bfResult?.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id)?
                            .TechnologyResults.FirstOrDefault(_ => _.Technology == technology)?
                            .DetailResult.Value;
                    }
                    resStr = format ? result?.ToString("N0") ?? Constants.No_Value : result?.ToString() ?? "0";
                    break;
                case metricType.NotKnown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (metricType.QualityRule == type)
            {
                name = name + " (" + metricId + ")";
            }
            SimpleResult res = new SimpleResult {name = name, type = type, result = result, resultStr = resStr};
            return res;
        }

        /// <summary>
        /// This method return the evolution of a metric between 2 snapshots results (name, type, current result, previous result, absolute evolution and percent evolution
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="curSnapshot"></param>
        /// <param name="prevSnapshot"></param>
        /// <param name="metricId"></param>
        /// <param name="evol"></param>
        /// <param name="module"></param>
        /// <param name="technology"></param>
        /// <param name="format"></param> format is true for table component and false for graph component
        /// <returns></returns>
        public static EvolutionResult GetMetricEvolution(ReportData reportData, Snapshot curSnapshot, Snapshot prevSnapshot, string metricId, bool evol, Module module, string technology, bool format)
        {
            SimpleResult curResult = null;
            SimpleResult prevResult = null;
            if (curSnapshot != null) curResult = GetMetricNameAndResult(reportData, curSnapshot, metricId, module, technology, format);
            if (prevSnapshot != null) prevResult = GetMetricNameAndResult(reportData, prevSnapshot, metricId, module, technology, format);
            if (!evol && (curResult?.result != null || prevResult?.result != null))
            {
                string name = curResult?.name ?? prevResult?.name ?? Constants.No_Value;
                metricType type = curResult?.type ?? prevResult?.type ?? metricType.NotKnown;
                string curRes = format ? Constants.No_Value : "0";
                string prevRes = format ? Constants.No_Value : "0";
                switch (type)
                {
                    case metricType.BusinessCriteria:
                    case metricType.TechnicalCriteria:
                    case metricType.QualityRule:
                        curRes = curResult?.result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                        prevRes = prevResult?.result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                        break;
                    case metricType.SizingMeasure:
                    case metricType.BackgroundFact:
                        if (format)
                        {
                            curRes = curResult?.result?.ToString("N0") ?? Constants.No_Value;
                            prevRes = prevResult?.result?.ToString("N0") ?? Constants.No_Value;
                        }
                        else
                        {
                            curRes = curResult?.result?.ToString() ?? "0";
                            prevRes = prevResult?.result?.ToString() ?? "0";

                        }
                        break;
                    case metricType.NotKnown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return new EvolutionResult
                {
                    name = name,
                    type = type,
                    curResult = curRes,
                    prevResult = prevRes,
                    evolution = format ? Constants.No_Value : "0",
                    evolutionPercent = format ? Constants.No_Value : "0"
                };
            }

            if (curResult?.result == null || prevResult?.result == null)
            {
                string name = curResult?.name ?? prevResult?.name ?? Constants.No_Value;
                metricType type = curResult?.type ?? prevResult?.type ?? metricType.NotKnown;
                string curRes = format ? Constants.No_Value : "0";
                string prevRes = format ? Constants.No_Value : "0";
                switch (type)
                {
                    case metricType.BusinessCriteria:
                    case metricType.TechnicalCriteria:
                    case metricType.QualityRule:
                        curRes = curResult?.result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                        prevRes = prevResult?.result?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                        break;
                    case metricType.SizingMeasure:
                    case metricType.BackgroundFact:
                        if (format)
                        {
                            curRes = curResult?.result?.ToString("N0") ?? Constants.No_Value;
                            prevRes = prevResult?.result?.ToString("N0") ?? Constants.No_Value;
                        }
                        else
                        {
                            curRes = curResult?.result?.ToString() ?? "0";
                            prevRes = prevResult?.result?.ToString() ?? "0";
                        }
                        break;
                    case metricType.NotKnown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new EvolutionResult
                {
                    name = name,
                    type = type,
                    curResult = curRes,
                    prevResult = prevRes,
                    evolution = format ? Constants.No_Value : "0",
                    evolutionPercent = format ? Constants.No_Value : "0"
                };

            }

            string evolution;
            string evolPercent;
            string finalCurRes;
            string finalPrevRes;
            double? evp;
            switch (curResult.type)
            {
                case metricType.BusinessCriteria:
                case metricType.TechnicalCriteria:
                case metricType.QualityRule:
                    if (curResult.result != null && prevResult.result != null)
                    {
                        finalCurRes = curResult.result.Value.ToString("N2");
                        finalPrevRes = prevResult.result.Value.ToString("N2");
                        evolution = (curResult.result - prevResult.result).Value.ToString("N2");
                        evp = Math.Abs((double) prevResult.result) > 0.0 ? (curResult.result - prevResult.result) / prevResult.result : null;
                        evolPercent = evp != null ? evp.FormatPercent() : format ? Constants.No_Value : "0";
                    }
                    else
                    {
                        finalCurRes = format ? Constants.No_Value : "0";
                        finalPrevRes = format ? Constants.No_Value : "0";
                        evolution = format ? Constants.No_Value : "0";
                        evolPercent = format ? Constants.No_Value : "0";
                    }
                    break;
                case metricType.SizingMeasure:
                case metricType.BackgroundFact:
                    if (curResult.result != null && prevResult.result != null)
                    {
                        if (format)
                        {
                            finalCurRes = curResult.result.Value.ToString("N0");
                            finalPrevRes = prevResult.result.Value.ToString("N0");
                            evolution = (curResult.result - prevResult.result).Value.ToString("N0");
                        }
                        else
                        {
                            finalCurRes = curResult.result.ToString();
                            finalPrevRes = prevResult.result.ToString();
                            evolution = (curResult.result - prevResult.result).ToString();
                        }
                        evp = Math.Abs((double) prevResult.result) > 0.0 ? (curResult.result - prevResult.result) / prevResult.result : null;
                        evolPercent = evp != null ? evp.FormatPercent() : format ? Constants.No_Value : "0";
                    }
                    else
                    {
                        finalCurRes = format ? Constants.No_Value : "0";
                        finalPrevRes = format ? Constants.No_Value : "0";
                        evolution = format ? Constants.No_Value : "0";
                        evolPercent = format ? Constants.No_Value : "0";
                    }
                    break;
                case metricType.NotKnown:
                    finalCurRes = format ? Constants.No_Value : "0";
                    finalPrevRes = format ? Constants.No_Value : "0";
                    evolution = format ? Constants.No_Value : "0";
                    evolPercent = format ? Constants.No_Value : "0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new EvolutionResult
            {
                name = curResult.name,
                type = curResult.type,
                curResult = finalCurRes,
                prevResult = finalPrevRes,
                evolution = evolution,
                evolutionPercent = evolPercent
            };
        }

        public static SimpleResult GetAggregatedMetric(ReportData reportData, Dictionary<Application, Snapshot> lastSnapshotList, string metricId, string techno, string aggregator, bool format)
        {

            List<SimpleResult> results = new List<SimpleResult>();
            foreach (Application application in reportData.Applications)
            {
                Snapshot appCurSnap;
                try
                {
                    appCurSnap = lastSnapshotList[application];
                }
                catch (KeyNotFoundException)
                {
                    appCurSnap = null;
                }
                SimpleResult appRes = GetMetricNameAndResult(reportData, appCurSnap, metricId, null, techno, format);
                results.Add(appRes);
            }

            string metName = results.FirstOrDefault()?.name;
            if (metName == null) return null;

            metricType metType = results.FirstOrDefault()?.type ?? metricType.NotKnown;

            double? curResult = 0;

            if (string.IsNullOrEmpty(aggregator))
            {
                switch (metType)
                {
                    case metricType.QualityRule:
                    case metricType.TechnicalCriteria:
                    case metricType.BusinessCriteria:
                        aggregator = "AVERAGE";
                        break;
                    case metricType.SizingMeasure:
                    case metricType.BackgroundFact:
                        aggregator = "SUM";
                        break;
                }
            }

            switch (aggregator)
            {
                case "SUM":
                    foreach (var _result in results)
                    {
                        curResult = _result.result != null ? curResult + _result.result : curResult;
                    }
                    break;
                case "AVERAGE":
                    int nbCurRes = 0;
                    foreach (var _result in results)
                    {
                        if (_result.result == null) continue;
                        curResult = curResult + _result.result;
                        nbCurRes++;
                    }
                    curResult = nbCurRes != 0 ? curResult / nbCurRes : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // format curResult and prevResult in case of metricType
            string res;
            switch (metType)
            {
                case metricType.BusinessCriteria:
                case metricType.TechnicalCriteria:
                case metricType.QualityRule:
                    res = curResult?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                    break;
                case metricType.SizingMeasure:
                case metricType.BackgroundFact:
                    res = format ? curResult?.ToString("N0") ?? Constants.No_Value : curResult?.ToString() ?? "0";
                    break;
                case metricType.NotKnown:
                    res = curResult?.ToString() ?? (format ? Constants.No_Value : "0");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new SimpleResult
            {
                name = metName,
                type = metType,
                resultStr = res
            };
        }

        public static List<string> BuildRulesList(ReportData reportData, List<string> metrics, bool critical)
        {
            List<string> qualityRules = new List<string>();

            foreach (string _metric in metrics)
            {
                int idx;
                // If metric can not be parsed as integer, this is potentially a string containing a standard tag for quality rule selection
                if (!int.TryParse(_metric, out idx))
                {
                    List<string> stdTagMetrics = reportData.SnapshotExplorer.GetQualityStandardsRulesList(reportData.CurrentSnapshot.Href, _metric);
                    if (stdTagMetrics != null) qualityRules.AddRange(stdTagMetrics);
                }
                else
                {
                    Snapshot snapshot = reportData.CurrentSnapshot;
                    int metricId = int.Parse(_metric);
                    string name = snapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == metricId).Select(_ => _.Reference.Name).FirstOrDefault();

                    if (name != null)
                    {
                        // This is a Business criteria

                        List<RuleDetails> rules =  reportData.RuleExplorer.GetRulesDetails(snapshot.DomainId, metricId, snapshot.Id).ToList();
                        qualityRules.AddRange(critical ? rules.Where(_ => _.Critical).Select(_ => _.Key.ToString()).ToList() : rules.Select(_ => _.Key.ToString()).ToList());
                    }
                    else
                    {
                        // if metricId is not a Business Criteria
                        name = snapshot.TechnicalCriteriaResults.Where(_ => _.Reference.Key == metricId).Select(_ => _.Reference.Name).FirstOrDefault();
                        if (name != null)
                        {
                            // This is a Technical criteria
                            List<Contributor> rules = reportData.RuleExplorer.GetRulesInTechnicalCriteria(snapshot.DomainId, _metric, snapshot.Id).ToList();
                            qualityRules.AddRange(critical ? rules.Where(_ => _.Critical).Select(_ => _.Key.ToString()).ToList() : rules.Select(_ => _.Key.ToString()));
                        }
                        else
                        {
                            // if metricId is not a Technical Criteria
                            name = snapshot.QualityRulesResults.Where(_ => _.Reference.Key == metricId).Select(_ => _.Reference.Name).FirstOrDefault();
                            if (name != null)
                            {
                                qualityRules.Add(_metric);
                            }
                        }

                    }

                }
            }

            return qualityRules.Distinct().ToList();
        }

        public static int PopulateViolationsBookmarks(ReportData reportData, Violation[] violations, int violationCounter, List<string> rowData, int cellidx, string ruleName, List<CellAttributes> cellProps, bool hasPreviousSnapshot, string domainId, string snapshotId, string metric)
        {
            foreach (Violation _violation in violations)
            {
                violationCounter++;
                rowData.Add("");
                cellidx++;
                rowData.Add(Labels.Violation + " #" + violationCounter + "    " + ruleName);
                cellProps.Add(new CellAttributes(cellidx, ColorGainsboro));
                cellidx++;
                rowData.Add(Labels.ObjectName + ": " + _violation.Component.Name);
                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                cellidx++;

                TypedComponent objectComponent = reportData.SnapshotExplorer.GetTypedComponent(reportData.CurrentSnapshot.DomainId, _violation.Component.GetComponentId(), reportData.CurrentSnapshot.GetId());
                rowData.Add(Labels.IFPUG_ObjectType + ": " + objectComponent.Type.Label);
                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                cellidx++;

                if (hasPreviousSnapshot)
                {
                    rowData.Add(Labels.Status + ": " + _violation.Diagnosis.Status);
                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                    cellidx++;
                }

                AssociatedValue associatedValue = reportData.SnapshotExplorer.GetAssociatedValue(domainId, _violation.Component.GetComponentId(), snapshotId, metric);
                if (associatedValue == null) continue;

                if (associatedValue.Type == null || associatedValue.Type.Equals("integer"))
                {
                    IEnumerable<IEnumerable<CodeBookmark>> bookmarks = associatedValue.Bookmarks;
                    if (bookmarks == null || !bookmarks.Any())
                    {
                        cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation);
                    }
                    else
                    {
                        foreach (IEnumerable<CodeBookmark> _codeBookmarks in bookmarks)
                        {
                            IEnumerable<CodeBookmark> _bookmarks = _codeBookmarks.ToList();
                            foreach (CodeBookmark _bookmark in _bookmarks)
                            {
                                rowData.Add(Labels.FilePath + ": " + _bookmark.CodeFragment.CodeFile.Name);
                                cellProps.Add(new CellAttributes(cellidx, ColorLavander));
                                cellidx++;
                                Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookmark, 3);
                                if (codeLines == null) continue;
                                foreach (KeyValuePair<int, string> codeLine in codeLines)
                                {
                                    rowData.Add(codeLine.Key + " : " + codeLine.Value);
                                    cellProps.Add(codeLine.Key >= _bookmark.CodeFragment.StartLine && codeLine.Key <= _bookmark.CodeFragment.EndLine
                                        ? new CellAttributes(cellidx, ColorLightYellow)
                                        : new CellAttributes(cellidx, ColorWhite));
                                    cellidx++;
                                }
                            }
                        }
                    }
                }

                if (associatedValue.Type != null && associatedValue.Type.Equals("path"))
                {
                    // manage case when type="path" and values contains the different path
                    AssociatedValuePath associatedValueEx = reportData.SnapshotExplorer.GetAssociatedValuePath(domainId, _violation.Component.GetComponentId(), snapshotId, metric);
                    IEnumerable<IEnumerable<CodeBookmark>> values = associatedValueEx?.Values;
                    if (values == null || !values.Any())
                    {
                        cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation);
                    }
                    else
                    {
                        int pathCounter = 0;
                        foreach (IEnumerable<CodeBookmark> _value in values)
                        {
                            pathCounter++;
                            IEnumerable<CodeBookmark> _bookmarksValue = _value.ToList();
                            rowData.Add(Labels.ViolationPath + " #" + pathCounter);
                            cellProps.Add(new CellAttributes(cellidx, ColorLavander));
                            cellidx++;
                            string previousFile = string.Empty;
                            foreach (CodeBookmark _bookval in _bookmarksValue)
                            {
                                if (string.IsNullOrEmpty(previousFile) || !previousFile.Equals(_bookval.CodeFragment.CodeFile.Name))
                                {
                                    previousFile = _bookval.CodeFragment.CodeFile.Name;
                                    rowData.Add(Labels.FilePath + ": " + _bookval.CodeFragment.CodeFile.Name);
                                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                                    cellidx++;
                                }

                                Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookval, 0);

                                foreach (KeyValuePair<int, string> codeLine in codeLines)
                                {
                                    rowData.Add(codeLine.Key + " : " + codeLine.Value);
                                    cellProps.Add(codeLine.Key == _bookval.CodeFragment.StartLine
                                        ? new CellAttributes(cellidx, ColorLightYellow)
                                        : new CellAttributes(cellidx, ColorWhite));
                                    cellidx++;
                                }
                            }
                            rowData.Add("");
                            cellidx++;
                        }
                    }
                }
                if (associatedValue.Type != null && associatedValue.Type.Equals("group"))
                {
                    // manage case when type="group" and values contains an array of array of components
                    AssociatedValueGroup associatedValueEx = reportData.SnapshotExplorer.GetAssociatedValueGroup(domainId, _violation.Component.GetComponentId(), snapshotId, metric);
                    IEnumerable<IEnumerable<CodeBookmark>> values = associatedValueEx?.Values;
                    if (values == null || !values.Any())
                    {
                        cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation);
                    }
                    else
                    {
                        foreach (IEnumerable<CodeBookmark> components in values)
                        {
                            foreach (CodeBookmark _component in components)
                            {
                                rowData.Add(_component.Component.Name);
                                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                                cellidx++;
                            }
                            rowData.Add("");
                            cellidx++;
                        }
                    }
                }
                if (associatedValue.Type != null && (associatedValue.Type.Equals("object") || associatedValue.Type.Equals("text") || associatedValue.Type.Equals("percentage")))
                {
                    // manage case when type="object", "text" or "percentage"
                    cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation);
                }

            }
            return cellidx;
        }

        private static int AddSourceCode(ReportData reportData, List<string> rowData, int cellidx, List<CellAttributes> cellProps, string domainId, string snapshotId, Violation violation)
        {
            List<Tuple<string, Dictionary<int, string>>> codes = reportData.SnapshotExplorer.GetSourceCode(domainId, snapshotId, violation.Component.GetComponentId(), 6);
            if (codes == null) return cellidx;

            foreach (Tuple<string, Dictionary<int, string>> _code in codes)
            {
                rowData.Add(Labels.FilePath + ": " + _code.Item1);
                cellProps.Add(new CellAttributes(cellidx, ColorLavander));
                cellidx++;

                Dictionary<int, string> codeLines = _code.Item2;
                if (codeLines == null) return cellidx;
                foreach (KeyValuePair<int, string> codeLine in codeLines)
                {
                    rowData.Add(codeLine.Key + " : " + codeLine.Value);
                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                    cellidx++;
                }
            }
            
            return cellidx;
        }

        public static string GetPropertyName(string prop)
        {
            switch (prop)
            {
                case "codeLines":
                    return Labels.codeLines;
                case "commentedCodeLines":
                    return Labels.commentedCodeLines;
                case "commentLines":
                    return Labels.commentLines;
                case "coupling":
                    return Labels.coupling;
                case "fanIn":
                    return Labels.fanIn;
                case "fanOut":
                    return Labels.fanOut;
                case "cyclomaticComplexity":
                    return Labels.cyclomaticComplexity;
                case "ratioCommentLinesCodeLines":
                    return Labels.ratioCommentLinesCodeLines;
                case "halsteadProgramLength":
                    return Labels.halsteadProgramLength;
                case "halsteadProgramVocabulary":
                    return Labels.halsteadProgramVocabulary;
                case "halsteadVolume":
                    return Labels.halsteadVolume;
                case "distinctOperators":
                    return Labels.distinctOperators;
                case "distinctOperands":
                    return Labels.distinctOperands;
                case "integrationComplexity":
                    return Labels.integrationComplexity;
                case "essentialComplexity":
                    return Labels.essentialComplexity;
                default:
                    return string.Empty;
            }
        }
    }
}