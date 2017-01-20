using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using System;
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
        /// This method return the result (grade or value) of the metric from getting it in the results of the snapshot
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        public static string GetMetricResult(ReportData reportData, Snapshot snapshot, string metricId)
        {
            string result = ((snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Grade).FirstOrDefault()?.ToString("N2") ??
                       snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Grade).FirstOrDefault()?.ToString("N2")) ?? 
                      snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Grade).FirstOrDefault()?.ToString("N2")) ??
                     snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Value).FirstOrDefault()?.ToString("N0"); 
            if (result != null) return result;
            Result bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, metricId, true, true).FirstOrDefault();
            if (bfResult == null || !bfResult.ApplicationResults.Any()) return null;
            result = bfResult.ApplicationResults[0].DetailResult.Value?.ToString("N0") ?? Constants.No_Value;
            return result;
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
            if ( name != null) type = metricType.BusinessCriteria;
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
                    if (module == null && technology == string.Empty)
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                    }
                    else if (module != null && technology == string.Empty)
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module == null && technology != string.Empty)
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module != null && technology != string.Empty)
                    {
                        result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null )?.DetailResult.Grade;
                    }
                    break;
                case metricType.TechnicalCriteria:
                    if (module == null && technology == string.Empty)
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                                     .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                    }
                    else if (module != null && technology == string.Empty)
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module == null && technology != string.Empty)
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module != null && technology != string.Empty)
                    {
                        result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    break;
                case metricType.QualityRule:
                    if (module == null && technology == string.Empty)
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                    }
                    else if (module != null && technology == string.Empty)
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module == null && technology != string.Empty)
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    else if (module != null && technology != string.Empty)
                    {
                        result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                    }
                    break;
                case metricType.SizingMeasure:
                    if (module == null && technology == string.Empty)
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                            .Select(_ => _.DetailResult.Value).FirstOrDefault();
                    }
                    else if (module != null && technology == string.Empty)
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Value;
                    }
                    else if (module == null && technology != string.Empty)
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                            .SelectMany(_ => _.TechnologyResult)
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Value;
                    }
                    else if (module != null && technology != string.Empty)
                    {
                        result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                            .SelectMany(_ => _.ModulesResult)
                            .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                            .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Value;
                    }
                    break;
                case metricType.BackgroundFact:
                    if (module == null && technology == string.Empty)
                    {
                        result = bfResult?.ApplicationResults[0].DetailResult.Value;
                    }
                    else if (module != null && technology == string.Empty)
                    {
                        result = bfResult?.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id)?
                            .DetailResult.Value;
                    }
                    else if (module == null && technology != string.Empty)
                    {
                        result = bfResult?.ApplicationResults[0].TechnologyResult.FirstOrDefault(_ => _.Technology == technology)?
                            .DetailResult.Value;
                    }
                    else if (module != null && technology != string.Empty)
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

            SimpleResult res = new SimpleResult { name = name,type = type, result = result};
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
        /// <returns></returns>
        public static EvolutionResult GetMetricEvolution(ReportData reportData, Snapshot curSnapshot, Snapshot prevSnapshot, string metricId, bool evol, Module module, string technology)
        {
            SimpleResult curResult = null;
            SimpleResult prevResult = null;
            if (curSnapshot != null) curResult = GetMetricNameAndResult(reportData, curSnapshot, metricId,module,technology);
            if (prevSnapshot != null) prevResult = GetMetricNameAndResult(reportData, prevSnapshot, metricId, module, technology);
            if (!evol && (curResult?.result != null || prevResult?.result != null))
            {
                string name = curResult?.name ?? prevResult?.name ?? Constants.No_Value;
                metricType type = curResult?.type ?? prevResult?.type ?? metricType.NotKnown;
                string curRes = Constants.No_Value;
                string prevRes = Constants.No_Value;
                switch (type)
                {
                    case metricType.BusinessCriteria:
                    case metricType.TechnicalCriteria:
                    case metricType.QualityRule:
                        curRes = curResult?.result?.ToString("N2") ?? Constants.No_Value;
                        prevRes = prevResult?.result?.ToString("N2") ?? Constants.No_Value;
                        break;
                    case metricType.SizingMeasure:
                    case metricType.BackgroundFact:
                        curRes = curResult?.result?.ToString("N0") ?? Constants.No_Value;
                        prevRes = prevResult?.result?.ToString("N0") ?? Constants.No_Value;
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
                    evolution = Constants.No_Value,
                    evolutionPercent = Constants.No_Value
                };
            }

            if (curResult?.result == null || prevResult?.result == null)
            {
                string name = curResult?.name ?? prevResult?.name ?? Constants.No_Value;
                metricType type = curResult?.type ?? prevResult?.type ?? metricType.NotKnown;
                string curRes = Constants.No_Value;
                string prevRes = Constants.No_Value;
                switch (type)
                {
                    case metricType.BusinessCriteria:
                    case metricType.TechnicalCriteria:
                    case metricType.QualityRule:
                        curRes = curResult?.result?.ToString("N2") ?? Constants.No_Value;
                        prevRes = prevResult?.result?.ToString("N2") ?? Constants.No_Value;
                        break;
                    case metricType.SizingMeasure:
                    case metricType.BackgroundFact:
                        curRes = curResult?.result?.ToString("N0") ?? Constants.No_Value;
                        prevRes = prevResult?.result?.ToString("N0") ?? Constants.No_Value;
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
                    evolution = Constants.No_Value,
                    evolutionPercent = Constants.No_Value
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
                        evp = Math.Abs((double)prevResult.result) > 0.0 ? (curResult.result - prevResult.result) / prevResult.result : null;
                        evolPercent = evp != null ? evp.FormatPercent() : Constants.No_Value;
                    }
                    else
                    {
                        finalCurRes = Constants.No_Value;
                        finalPrevRes = Constants.No_Value;
                        evolution = Constants.No_Value;
                        evolPercent = Constants.No_Value;
                    }
                    break;
                case metricType.SizingMeasure:
                case metricType.BackgroundFact:
                    if (curResult.result != null && prevResult.result != null)
                    {
                        finalCurRes = curResult.result.Value.ToString("N0");
                        finalPrevRes = prevResult.result.Value.ToString("N0");
                        evolution = (curResult.result - prevResult.result).Value.ToString("N0");
                        evp = prevResult.result != 0 ? (curResult.result - prevResult.result) / prevResult.result : null;
                        evolPercent = evp != null ? evp.FormatPercent() : Constants.No_Value;
                    }
                    else
                    {
                        finalCurRes = Constants.No_Value;
                        finalPrevRes = Constants.No_Value;
                        evolution = Constants.No_Value;
                        evolPercent = Constants.No_Value;
                    }
                    break;
                case metricType.NotKnown:
                    finalCurRes = Constants.No_Value;
                    finalPrevRes = Constants.No_Value;
                    evolution = Constants.No_Value;
                    evolPercent = Constants.No_Value;
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

    }
}
