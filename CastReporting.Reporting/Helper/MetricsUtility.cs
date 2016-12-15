using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using System;
using System.Linq;
using CastReporting.BLL.Computing;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.Reporting
{
    public static class MetricsUtility
    {
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

        public static EvolutionResult GetMetricNameAndResult(ReportData reportData, Snapshot snapshot, string metricId)
        {
            metricType type = metricType.NotKnown;
            Result bfResult = null;
            string result;

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
                    result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Grade).FirstOrDefault()?.ToString("N2") ?? Constants.No_Value;
                    break;
                case metricType.TechnicalCriteria:
                    result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Grade).FirstOrDefault()?.ToString("N2") ?? Constants.No_Value;
                    break;
                case metricType.QualityRule:
                    result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Grade).FirstOrDefault()?.ToString("N2") ?? Constants.No_Value;
                    break;
                case metricType.SizingMeasure:
                    result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.DetailResult.Value).FirstOrDefault()?.ToString("N0") ?? Constants.No_Value;
                    break;
                case metricType.BackgroundFact:
                    result = bfResult?.ApplicationResults[0].DetailResult.Value?.ToString("N0") ?? Constants.No_Value;
                    break;
                case metricType.NotKnown:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EvolutionResult evolres = new EvolutionResult {name = name,type = type, curResult = result};
            return evolres;
        }

        public static EvolutionResult GetMetricEvolution(ReportData reportData, Snapshot curSnapshot, Snapshot prevSnapshot, string metricId, bool evol)
        {
            EvolutionResult curResult = null;
            EvolutionResult prevResult = null;
            if (curSnapshot != null) curResult = GetMetricNameAndResult(reportData, curSnapshot, metricId);
            if (prevSnapshot != null) prevResult = GetMetricNameAndResult(reportData, curSnapshot, metricId);
            if (evol && (curResult?.curResult == null || prevResult?.curResult == null))
            {
                return new EvolutionResult
                {
                    name = curResult?.name ?? prevResult?.name ?? Constants.No_Value,
                    type = curResult?.type ?? prevResult?.type ?? metricType.NotKnown,
                    curResult = curResult?.curResult ?? Constants.No_Value,
                    prevResult = prevResult?.curResult ?? Constants.No_Value,
                    evolution = Constants.No_Value,
                    evolutionPercent = Constants.No_Value
                };
            }

            if (!evol || curResult.curResult == null || prevResult.curResult == null) return null;

            string evolution;
            string evolPercent;
            switch (curResult.type)
            {
                case metricType.BusinessCriteria:
                case metricType.TechnicalCriteria:
                case metricType.QualityRule:
                    double? curValueD = double.Parse(curResult.curResult);
                    double? prevValueD = double.Parse(prevResult.curResult);
                    evolution = (curValueD - prevValueD).Value.ToString("N2");
                    evolPercent = Math.Abs((double)prevValueD) > 0.0 ?  ((curValueD - prevValueD) /prevValueD).Value.ToString("N2") : Constants.No_Value;
                    break;
                case metricType.SizingMeasure:
                case metricType.BackgroundFact:
                    int? curValueI = int.Parse(curResult.curResult);
                    int? prevValueI = int.Parse(prevResult.curResult);
                    evolution = (curValueI - prevValueI).Value.ToString("N0") ?? Constants.No_Value;
                    evolPercent = prevValueI != 0 ? ((curValueI - prevValueI) / prevValueI).Value.ToString("N2") : Constants.No_Value;
                    break;
                case metricType.NotKnown:
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
                curResult = curResult.curResult,
                prevResult = prevResult.curResult,
                evolution = evolution,
                evolutionPercent = evolPercent
            };
        }

    }
}
