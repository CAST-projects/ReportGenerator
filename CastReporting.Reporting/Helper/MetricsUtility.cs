using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting
{
    public static class MetricsUtility
    {
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
        /// <returns></returns>
        public static SimpleResult GetMetricNameAndResult(ReportData reportData, Snapshot snapshot, string metricId, Module module, string technology)
        {
            metricType type = metricType.NotKnown;
            Result bfResult = null;
            double? result = null;

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
                    break;
                case metricType.NotKnown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SimpleResult res = new SimpleResult {name = name, type = type, result = result};
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
            if (curSnapshot != null) curResult = GetMetricNameAndResult(reportData, curSnapshot, metricId, module, technology);
            if (prevSnapshot != null) prevResult = GetMetricNameAndResult(reportData, prevSnapshot, metricId, module, technology);
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

        public static EvolutionResult GetAggregatedMetricEvolution(ReportData reportData, Dictionary<Application, Snapshot> curPeriod, Dictionary<Application, Snapshot> prevPeriod, string metricId, string aggregator, bool evol, bool format)
        {

            // evol = false here because evolution is calculated after results by periods
            List<EvolutionResult> results = new List<EvolutionResult>();
            foreach (Application application in reportData.Applications)
            {
                Snapshot appCurSnap;
                Snapshot appPrevSnap;
                try
                {
                    appCurSnap = curPeriod[application];
                }
                catch (KeyNotFoundException)
                {
                    appCurSnap = null;
                }
                try
                {
                    appPrevSnap = prevPeriod[application];
                }
                catch (KeyNotFoundException)
                {
                    appPrevSnap = null;
                }

                EvolutionResult appRes = GetMetricEvolution(reportData, appCurSnap, appPrevSnap, metricId, false, null, string.Empty, format);
                results.Add(appRes);

            }

            string metName = results.FirstOrDefault()?.name;
            if (metName == null) return null;

            metricType metType = results.FirstOrDefault()?.type ?? metricType.NotKnown;

            double? curResult = 0;
            double? prevResult = 0;
            double? evolResult = 0;
            double? evolPercentResult = 0;


            switch (aggregator)
            {
                case "SUM":
                    foreach (EvolutionResult _evolutionResult in results)
                    {
                        double curTest;
                        if (double.TryParse(_evolutionResult.curResult, out curTest))
                        {
                            curResult += curTest;
                        }
                        double prevTest;
                        if (double.TryParse(_evolutionResult.prevResult, out prevTest))
                        {
                            prevResult += prevTest;
                        }
                    }
                    curResult = curResult ?? null;
                    prevResult = prevResult ?? null;
                    break;
                case "AVERAGE":
                    int nbCurRes = 0;
                    int nbPrevRes = 0;
                    foreach (EvolutionResult _evolutionResult in results)
                    {
                        double curTest;
                        if (double.TryParse(_evolutionResult.curResult, out curTest))
                        {
                            curResult += curTest;
                            nbCurRes++;
                        }
                        double prevTest;
                        if (double.TryParse(_evolutionResult.prevResult, out prevTest))
                        {
                            prevResult += prevTest;
                            nbPrevRes++;
                        }
                    }
                    curResult = nbCurRes != 0 ? curResult / nbCurRes : null;
                    prevResult = nbPrevRes != 0 ? prevResult / nbPrevRes : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (evol && prevResult != null)
            {
                evolResult = curResult - prevResult;
                evolPercentResult = (Math.Abs(prevResult.Value) > 0.0) ? (double?)((curResult.Value - prevResult.Value) / prevResult.Value) : null;
            }

            string curRes;
            string prevRes;
            string evolRes;
            string evolPercentRes;

            // format curResult and prevResult in case of metricType
            switch (metType)
            {
                case metricType.BusinessCriteria:
                case metricType.TechnicalCriteria:
                case metricType.QualityRule:
                    curRes = curResult?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                    prevRes = prevResult?.ToString("N2") ?? (format ? Constants.No_Value : "0");
                    evolRes = evol ? evolResult.Value.ToString("N2") : format ? Constants.No_Value : "0";
                    evolPercentRes = evol ? evolPercentResult.FormatPercent() : format ? Constants.No_Value : "0";
                    break;
                case metricType.SizingMeasure:
                case metricType.BackgroundFact:
                    curRes = curResult?.ToString("N0") ?? (format ? Constants.No_Value : "0");
                    prevRes = prevResult?.ToString("N0") ?? (format ? Constants.No_Value : "0");
                    evolRes = evol ? evolResult.Value.ToString("N0") : format ? Constants.No_Value : "0";
                    evolPercentRes = evol ? evolPercentResult.FormatPercent() : format ? Constants.No_Value : "0";
                    break;
                case metricType.NotKnown:
                    curRes = curResult?.ToString() ?? (format ? Constants.No_Value : "0");
                    prevRes = prevResult?.ToString() ?? (format ? Constants.No_Value : "0");
                    evolRes = evol ? evolResult.ToString() : format ? Constants.No_Value : "0";
                    evolPercentRes = evol ? evolPercentResult.FormatPercent() : format ? Constants.No_Value : "0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new EvolutionResult
            {
                name = metName,
                type = metType,
                curResult = curRes,
                prevResult = prevRes,
                evolution = evolRes,
                evolutionPercent = evolPercentRes
            };
        }

    }

}