using System;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using CastReporting.BLL.Computing.DTO;
using System.Linq;
using Cast.Util.Log;

namespace CastReporting.Reporting.Helper
{
    public static class PortfolioGenericContent
    {
        public class ObjConfig
        {
            public string Type { get; set; }
            public string[] Parameters { get; set; }
        }

        public static string GetTypeName(string type)
        {
            switch (type)
            {
                case "METRICS":
                    return Labels.Metrics;
                case "APPLICATIONS":
                    return Labels.Applications;
                case "VIOLATIONS":
                    return Labels.Violations;
                case "CRITICAL_VIOLATIONS":
                    return Labels.ViolationsCritical;
                case "TECHNOLOGIES":
                    return Labels.Technologies;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string GetItemName(string type, string item, ReportData reportData)
        {
            switch (type)
            {
                case "METRICS":
                    string name = string.Empty;
                    foreach (Application app in reportData.Applications)
                    {
                        name = MetricsUtility.GetMetricName(reportData, app.Snapshots.FirstOrDefault(), item);
                        if (name != Constants.No_Value) break;
                    }
                    return name ?? Constants.No_Value;
                case "APPLICATIONS":
                    return item;
                case "VIOLATIONS":
                    switch (item)
                    {
                        case "ADDED":
                            return Labels.AddedViolations;
                        case "REMOVED":
                            return Labels.RemovedViolations;
                        case "TOTAL":
                            return Labels.TotalViolations;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case "CRITICAL_VIOLATIONS":
                    switch (item)
                    {
                        case "ADDED":
                            return Labels.AddedCriticalViolations;
                        case "REMOVED":
                            return Labels.RemovedCriticalViolations;
                        case "TOTAL":
                            return Labels.TotalCriticalViolations;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case "TECHNOLOGIES":
                    return item;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /*
         * the bool format parameter is true for table component, and false for graph component
         */
        public static TableDefinition Content(ReportData reportData, Dictionary<string, string> options, bool format)
        {
            var rowData = new List<string>();
            ObjConfig[] _posConfig = new ObjConfig[4];
            Dictionary<Application, Snapshot> lastApplicationSnapshots = new Dictionary<Application, Snapshot>();
            List<Application> applications = new List<Application>();
            int positionApplications = -1;
            Dictionary<string, string> metricsAggregated = new Dictionary<string, string>();
            int positionMetrics = -1;
            List<string> violations = new List<string>();
            int positionViolations = -1;
            List<string> criticalViolations = new List<string>();
            int positionCriticalViolations = -1;
            List<string> technologies = new List<string>();
            int positionTechnologies = -1;

            List<string> metricsToRemove = new List<string>();
            Dictionary<Tuple<string, string, string, string>, string> results = new Dictionary<Tuple<string, string, string, string>, string>();

            #region Get Configuration

            // get the configuration
            string type0 = options.GetOption("COL1");
            _posConfig[0] = type0 != null ? new ObjConfig { Type = type0, Parameters = options.GetOption(type0) != null ? options.GetOption(type0).Trim().Split('|') : new string[] { } } : null;
            string type1 = options.GetOption("COL11");
            _posConfig[1] = type1 != null ? new ObjConfig { Type = type1, Parameters = options.GetOption(type1) != null ? options.GetOption(type1).Split('|') : new string[] { } } : null;
            string type2 = options.GetOption("ROW1");
            _posConfig[2] = type2 != null ? new ObjConfig { Type = type2, Parameters = options.GetOption(type2) != null ? options.GetOption(type2).Split('|') : new string[] { } } : null;
            string type3 = options.GetOption("ROW11");
            _posConfig[3] = type3 != null ? new ObjConfig { Type = type3, Parameters = options.GetOption(type3) != null ? options.GetOption(type3).Split('|') : new string[] { } } : null;

            string[] metricsAggregators = options.GetOption("AGGREGATORS")?.Split('|');

            // build list of last snapshot by application
            BuildApplicationSnapshots(lastApplicationSnapshots, reportData);

            // get the data and calculate results : snapshots, metrics, applications, technologies, violations, critical_violations
            for (int i = 0; i < _posConfig.Length; i++)
            {
                if (_posConfig[i] == null) continue;
                switch (_posConfig[i].Type)
                {
                    case "METRICS":
                        positionMetrics = i;
                        if (_posConfig[i].Parameters.Length == 0)
                        {
                            metricsAggregated.Add("60011", "AVERAGE");
                            metricsAggregated.Add("60012", "AVERAGE");
                            metricsAggregated.Add("60013", "AVERAGE");
                            metricsAggregated.Add("60014", "AVERAGE");
                            metricsAggregated.Add("60016", "AVERAGE");
                        }
                        else
                        {
                            BuildAggregatedMetricsList(reportData, metricsAggregated, _posConfig[i].Parameters.ToList(), metricsAggregators);
                        }
                        _posConfig[i].Parameters = metricsAggregated.Keys.ToArray();
                        break;
                    case "APPLICATIONS":
                        positionApplications = i;
                        if (_posConfig[i].Parameters.Length == 0 || _posConfig[positionApplications].Parameters.Contains("ALL"))
                        {
                            _posConfig[i] = new ObjConfig { Type = "APPLICATIONS", Parameters = new[] { "ALL" } };
                        }
                        else
                        {
                            applications.AddRange(_posConfig[i].Parameters.Contains("EACH") ?
                                reportData.Applications
                                // ReSharper disable once AccessToModifiedClosure
                                : reportData.Applications.Where(_ => _posConfig[i].Parameters.Contains(_.Name)));
                            _posConfig[i].Parameters = applications.Select(_ => _.Name).ToArray();
                        }
                        break;
                    case "VIOLATIONS":
                        positionViolations = i;
                        if (_posConfig[i].Parameters.Length == 0 || _posConfig[i].Parameters.Contains("ALL"))
                        {
                            violations.AddRange(new[] { "TOTAL", "ADDED", "REMOVED" });
                            _posConfig[i].Parameters = violations.ToArray();
                        }
                        else
                        {
                            violations.AddRange(_posConfig[i].Parameters);
                        }
                        break;
                    case "CRITICAL_VIOLATIONS":
                        positionCriticalViolations = i;
                        if (_posConfig[i].Parameters.Length == 0 || _posConfig[i].Parameters.Contains("ALL"))
                        {
                            criticalViolations.AddRange(new[] { "TOTAL", "ADDED", "REMOVED" });
                            _posConfig[i].Parameters = criticalViolations.ToArray();
                        }
                        else
                        {
                            criticalViolations.AddRange(_posConfig[i].Parameters);
                        }
                        break;
                    case "TECHNOLOGIES":
                        positionTechnologies = i;
                        if (_posConfig[i].Parameters.Contains("EACH") || _posConfig[i].Parameters.Length == 0)
                        {
                            foreach (var snapshot in lastApplicationSnapshots.Values)
                            {
                                foreach (var technology in snapshot.Technologies)
                                {
                                    if (!technologies.Contains(technology)) technologies.Add(technology);
                                }
                            }
                        }
                        else
                        {
                            technologies.AddRange(_posConfig[i].Parameters);
                        }
                        _posConfig[i].Parameters = technologies.ToArray();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // implicit metrics
            if (metricsAggregated.Count == 0) // use metricsAggregated to get the aggregator associated to a metric
            {
                string[] metricConfiguration = options.GetOption("METRICS")?.Split('|');
                if (metricConfiguration != null)
                {
                    BuildAggregatedMetricsList(reportData, metricsAggregated, metricConfiguration.ToList(), metricsAggregators);
                    if (metricsAggregated.Count > 1)
                    {
                        var metric = metricsAggregated.Keys.FirstOrDefault();
                        var agg = metricsAggregated[metric] ?? string.Empty;
                        metricsAggregated.Clear();
                        metricsAggregated.Add(metric, agg);
                    }
                }
                else
                {
                    metricsAggregated.Add("60017", "AVERAGE");
                }
            }

            #endregion

            #region Get Results

            #region Portfolio

            // case portfolio : no applications (APPLICATIONS=ALL), no technologies
            if (applications.Count == 0 && technologies.Count == 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                if (positionApplications != -1)
                {
                    _posConfig[positionApplications] = new ObjConfig { Type = "APPLICATIONS", Parameters = new[] { lastApplicationSnapshots.Count + " " + Labels.Applications } };
                    _posResults[positionApplications] = lastApplicationSnapshots.Count + " " + Labels.Applications;
                }

                // case grade (quality indicator) or value (sizing measure or background fact)
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        // Need to know the AGGREGATOR to replace the result of a metric_num_value by the aggregation of metric_num_value on the portfolio
                        // metricsAggregated contains metrics and aggregator
                        // Need to define methods to get the aggregation of metrics for the different kind of metrics => to do when building the metric lists
                        string _aggregator = metricsAggregated[_metricId];
                        SimpleResult res = MetricsUtility.GetAggregatedMetric(reportData, lastApplicationSnapshots, _metricId, string.Empty, _aggregator, format);
                        if (res.name == Constants.No_Value) continue;
                        if (positionMetrics != -1) _posResults[positionMetrics] = res.name;
                        try
                        {
                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.resultStr);
                        }
                        catch (ArgumentException e)
                        {
                            // When this exception occurs, this is because a metric with same name already exists.
                            LogHelper.Instance.LogDebug(e.Message);
                            metricsToRemove.Add(_metricId);
                        }
                    }
                }

                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        // _metricId should be a quality indicator, if not, return null
                        string name = string.Empty;
                        foreach (Application app in lastApplicationSnapshots.Keys)
                        {
                            name = MetricsUtility.GetMetricName(reportData, lastApplicationSnapshots[app], _metricId);
                            if (name != Constants.No_Value) break;
                        }
                        if (positionMetrics != -1) _posResults[positionMetrics] = name;

                        ViolStatMetricIdDTO stat = RulesViolationUtility.GetAggregatedViolStat(lastApplicationSnapshots, int.Parse(_metricId));
                        foreach (string _violation in violations)
                        {
                            string value;
                            switch (_violation)
                            {
                                case "TOTAL":
                                    _posResults[positionViolations] = Labels.TotalViolations;
                                    value = format ? stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalViolations?.ToString() ?? Constants.No_Value;
                                    break;
                                case "ADDED":
                                    _posResults[positionViolations] = Labels.AddedViolations;
                                    value = format ? stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedViolations?.ToString() ?? Constants.No_Value;
                                    break;
                                case "REMOVED":
                                    _posResults[positionViolations] = Labels.RemovedViolations;
                                    value = format ? stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedViolations?.ToString() ?? Constants.No_Value;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            try
                            {
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                            }
                            catch (ArgumentException e)
                            {
                                // When this exception occurs, this is because a metric with same name already exists.
                                LogHelper.Instance.LogDebug(e.Message);
                                metricsToRemove.Add(_metricId);
                            }
                        }
                    }
                }

                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        // _metricId should be a quality indicator, if not, return null
                        string name = string.Empty;
                        foreach (Application app in lastApplicationSnapshots.Keys)
                        {
                            name = MetricsUtility.GetMetricName(reportData, lastApplicationSnapshots[app], _metricId);
                            if (name != Constants.No_Value) break;
                        }
                        if (positionMetrics != -1) _posResults[positionMetrics] = name;

                        ViolStatMetricIdDTO stat = RulesViolationUtility.GetAggregatedViolStat(lastApplicationSnapshots, int.Parse(_metricId));
                        foreach (string _violation in criticalViolations)
                        {
                            string value;
                            switch (_violation)
                            {
                                case "TOTAL":
                                    _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                    value = format ? stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalCriticalViolations?.ToString() ?? Constants.No_Value;
                                    break;
                                case "ADDED":
                                    _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                    value = format ? stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedCriticalViolations?.ToString() ?? Constants.No_Value;
                                    break;
                                case "REMOVED":
                                    _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                    value = format ? stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedCriticalViolations?.ToString() ?? Constants.No_Value;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            try
                            {
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                            }
                            catch (ArgumentException e)
                            {
                                // When this exception occurs, this is because a metric with same name already exists.
                                LogHelper.Instance.LogDebug(e.Message);
                                metricsToRemove.Add(_metricId);
                            }
                        }
                    }
                }

                // functionnaly impossible case
                if (violations.Count != 0 && criticalViolations.Count != 0)
                {
                    return null;
                }

            }

            #endregion

            #region Applications

            if (applications.Count != 0 && technologies.Count == 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                // case grade
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        foreach (Application application in applications) // applications, no need to aggregate here, resuls by application
                        {
                            _posResults[positionApplications] = application.Name;
                            SimpleResult res = MetricsUtility.GetMetricNameAndResult(reportData, lastApplicationSnapshots[application], _metricId, null, string.Empty, format);
                            if (res.name == Constants.No_Value) continue;
                            if (positionMetrics != -1) _posResults[positionMetrics] = res.name;
                            try
                            {
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.resultStr);

                            }
                            catch (ArgumentException e)
                            {
                                // When this exception occurs, this is because a metric with same name already exists.
                                LogHelper.Instance.LogDebug(e.Message);
                                metricsToRemove.Add(_metricId);
                            }
                        }
                    }
                }

                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        string name = string.Empty;
                        foreach (Application _application in applications)
                        {
                            if (string.IsNullOrEmpty(name) || name != Constants.No_Value)
                            {
                                name = MetricsUtility.GetMetricName(reportData, lastApplicationSnapshots[_application], _metricId);
                                if (positionMetrics != -1) _posResults[positionMetrics] = name;
                            }

                            _posResults[positionApplications] = _application.Name;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(lastApplicationSnapshots[_application], int.Parse(_metricId));
                            foreach (string _violation in violations)
                            {
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        _posResults[positionViolations] = Labels.TotalViolations;
                                        value = format ? stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        _posResults[positionViolations] = Labels.AddedViolations;
                                        value = format ? stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "REMOVED":
                                        _posResults[positionViolations] = Labels.RemovedViolations;
                                        value = format ? stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                try
                                {
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                }
                                catch (ArgumentException e)
                                {
                                    // When this exception occurs, this is because a metric with same name already exists.
                                    LogHelper.Instance.LogDebug(e.Message);
                                    metricsToRemove.Add(_metricId);
                                }
                            }

                        }
                    }

                }

                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        string name = string.Empty;

                        foreach (Application _application in applications)
                        {
                            if (string.IsNullOrEmpty(name) || name != Constants.No_Value)
                            {
                                name = MetricsUtility.GetMetricName(reportData, lastApplicationSnapshots[_application], _metricId);
                                if (positionMetrics != -1) _posResults[positionMetrics] = name;
                            }

                            _posResults[positionApplications] = _application.Name;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(lastApplicationSnapshots[_application], int.Parse(_metricId));
                            foreach (string _violation in criticalViolations)
                            {
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                        value = format ? stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalCriticalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                        value = format ? stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedCriticalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "REMOVED":
                                        _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                        value = format ? stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedCriticalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                try
                                {
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                }
                                catch (ArgumentException e)
                                {
                                    // When this exception occurs, this is because a metric with same name already exists.
                                    LogHelper.Instance.LogDebug(e.Message);
                                    metricsToRemove.Add(_metricId);
                                }
                            }
                        }
                    }
                }

                // functionnaly impossible case
                if (violations.Count != 0 && criticalViolations.Count != 0)
                {
                    return null;
                }

            }

            #endregion

            #region Technologies

            if (technologies.Count != 0 && applications.Count == 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                // case metrics
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        foreach (string techno in technologies)
                        {
                            _posResults[positionTechnologies] = techno;
                            string _aggregator = metricsAggregated[_metricId];
                            SimpleResult res = MetricsUtility.GetAggregatedMetric(reportData, lastApplicationSnapshots, _metricId, techno, _aggregator, format);
                            if (res == null) continue;
                            if (res.name == Constants.No_Value) continue;
                            if (positionMetrics != -1) _posResults[positionMetrics] = res.name;
                            try
                            {
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.resultStr);
                            }
                            catch (ArgumentException e)
                            {
                                // When this exception occurs, this is because a metric with same name already exists.
                                LogHelper.Instance.LogDebug(e.Message);
                                metricsToRemove.Add(_metricId);
                            }
                        }
                    }
                }
                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        string name = GetItemName("METRICS", _metricId, reportData);
                        if (positionMetrics != -1) _posResults[positionMetrics] = name;
                        foreach (string _techno in technologies)
                        {
                            _posResults[positionTechnologies] = _techno;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetAggregatedViolStatTechno(lastApplicationSnapshots, _techno, int.Parse(_metricId));
                            foreach (string _violation in violations)
                            {
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        if (positionViolations != -1) _posResults[positionViolations] = Labels.TotalViolations;
                                        value = format ? stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        if (positionViolations != -1) _posResults[positionViolations] = Labels.AddedViolations;
                                        value = format ? stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "REMOVED":
                                        if (positionViolations != -1) _posResults[positionViolations] = Labels.RemovedViolations;
                                        value = format ? stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                try
                                {
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                }
                                catch (ArgumentException e)
                                {
                                    // When this exception occurs, this is because a metric with same name already exists.
                                    LogHelper.Instance.LogDebug(e.Message);
                                    metricsToRemove.Add(_metricId);
                                }
                            }
                        }
                    }
                }
                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        string name = GetItemName("METRICS", _metricId, reportData);
                        if (positionMetrics != -1) _posResults[positionMetrics] = name;
                        foreach (string _techno in technologies)
                        {
                            _posResults[positionTechnologies] = _techno;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetAggregatedViolStatTechno(lastApplicationSnapshots, _techno, int.Parse(_metricId));
                            foreach (string _criticalViolation in criticalViolations)
                            {
                                string value;
                                switch (_criticalViolation)
                                {
                                    case "TOTAL":
                                        if (positionCriticalViolations != -1) _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                        value = format ? stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalCriticalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        if (positionCriticalViolations != -1) _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                        value = format ? stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedCriticalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    case "REMOVED":
                                        if (positionCriticalViolations != -1) _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                        value = format ? stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedCriticalViolations?.ToString() ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                try
                                {
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                }
                                catch (ArgumentException e)
                                {
                                    // When this exception occurs, this is because a metric with same name already exists.
                                    LogHelper.Instance.LogDebug(e.Message);
                                    metricsToRemove.Add(_metricId);
                                }
                            }
                        }
                    }
                }
                // functionnaly impossible case
                if (violations.Count != 0 && criticalViolations.Count != 0)
                {
                    return null;
                }
            }

            #endregion

            #region Applications et Technologies

            if (technologies.Count != 0 && applications.Count != 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                // case metrics
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        foreach (Application _app in applications)
                        {
                            if (positionApplications != -1) _posResults[positionApplications] = _app.Name;
                            foreach (string techno in technologies)
                            {
                                if (positionTechnologies != -1) _posResults[positionTechnologies] = techno;
                                SimpleResult res = MetricsUtility.GetMetricNameAndResult(reportData, lastApplicationSnapshots[_app], _metricId, null, techno, format);
                                if (res == null) continue;
                                if (res.name == Constants.No_Value) continue;
                                if (positionMetrics != -1) _posResults[positionMetrics] = res.name;
                                try
                                {
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.resultStr);
                                }
                                catch (ArgumentException e)
                                {
                                    // When this exception occurs, this is because a metric with same name already exists.
                                    LogHelper.Instance.LogDebug(e.Message);
                                    metricsToRemove.Add(_metricId);
                                }
                            }
                        }
                    }
                }
                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        string name = GetItemName("METRICS", _metricId, reportData);
                        if (positionMetrics != -1) _posResults[positionMetrics] = name;

                        foreach (Application _app in applications)
                        {
                            if (positionApplications != -1) _posResults[positionApplications] = _app.Name;
                            foreach (string _techno in technologies)
                            {
                                if (positionTechnologies != -1) _posResults[positionTechnologies] = _techno;
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatTechno(lastApplicationSnapshots[_app], _techno, int.Parse(_metricId));
                                foreach (string _violation in violations)
                                {
                                    string value;
                                    switch (_violation)
                                    {
                                        case "TOTAL":
                                            if (positionViolations != -1) _posResults[positionViolations] = Labels.TotalViolations;
                                            value = format ? stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalViolations?.ToString() ?? Constants.No_Value;
                                            break;
                                        case "ADDED":
                                            if (positionViolations != -1) _posResults[positionViolations] = Labels.AddedViolations;
                                            value = format ? stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedViolations?.ToString() ?? Constants.No_Value;
                                            break;
                                        case "REMOVED":
                                            if (positionViolations != -1) _posResults[positionViolations] = Labels.RemovedViolations;
                                            value = format ? stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedViolations?.ToString() ?? Constants.No_Value;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    try
                                    {
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        // When this exception occurs, this is because a metric with same name already exists.
                                        LogHelper.Instance.LogDebug(e.Message);
                                        metricsToRemove.Add(_metricId);
                                    }
                                }
                            }
                        }
                    }
                }
                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metricsAggregated.Keys)
                    {
                        string name = GetItemName("METRICS", _metricId, reportData);
                        if (positionMetrics != -1) _posResults[positionMetrics] = name;
                        foreach (Application _app in applications)
                        {
                            if (positionApplications != -1) _posResults[positionApplications] = _app.Name;
                            foreach (string _techno in technologies)
                            {
                                if (positionTechnologies != -1) _posResults[positionTechnologies] = _techno;
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatTechno(lastApplicationSnapshots[_app], _techno, int.Parse(_metricId));
                                foreach (string _criticalViolation in criticalViolations)
                                {
                                    string value;
                                    switch (_criticalViolation)
                                    {
                                        case "TOTAL":
                                            if (positionCriticalViolations != -1) _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                            value = format ? stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.TotalCriticalViolations?.ToString() ?? Constants.No_Value;
                                            break;
                                        case "ADDED":
                                            if (positionCriticalViolations != -1) _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                            value = format ? stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.AddedCriticalViolations?.ToString() ?? Constants.No_Value;
                                            break;
                                        case "REMOVED":
                                            if (positionCriticalViolations != -1) _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                            value = format ? stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value : stat?.RemovedCriticalViolations?.ToString() ?? Constants.No_Value;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    try
                                    {
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        // When this exception occurs, this is because a metric with same name already exists.
                                        LogHelper.Instance.LogDebug(e.Message);
                                        metricsToRemove.Add(_metricId);
                                    }
                                }
                            }
                        }
                    }
                }
                // functionnaly impossible case
                if (violations.Count != 0 && criticalViolations.Count != 0)
                {
                    return null;
                }
            }

            #endregion

            foreach (ObjConfig _t in _posConfig)
            {
                if (_t == null) continue;
                if (_t.Type != "METRICS") continue;
                foreach (string _metric in metricsToRemove)
                {
                    metricsAggregated.Remove(_metric);
                }
                _t.Parameters = metricsAggregated.Keys.ToArray();
            }

            #endregion

            #region Get Display Data

            rowData.Add(GetTypeName(type2));
            foreach (var itemcol1 in _posConfig[0].Parameters)
            {
                string _col1Name = GetItemName(type0, itemcol1, reportData);
                if (_posConfig[1] != null)
                {
                    // ReSharper disable once AccessToModifiedClosure => false positive
                    rowData.AddRange(_posConfig[1].Parameters.Select(itemcol11 => GetItemName(type1, itemcol11, reportData)).Select(col11Name => _col1Name + " - " + col11Name));
                }
                else
                {
                    rowData.Add(_col1Name);
                }
            }
            int cntCol = _posConfig[0].Parameters.Length * _posConfig[1]?.Parameters.Length ?? _posConfig[0].Parameters.Length;
            int cntRow = 1;
            foreach (var itemrow1 in _posConfig[2].Parameters)
            {
                string itemrow1Name = GetItemName(type2, itemrow1, reportData);
                if (itemrow1Name == Constants.No_Value) continue;
                rowData.Add(itemrow1Name);
                if (_posConfig[3] != null)
                {
                    for (int s = 0; s < cntCol; s++)
                    {
                        rowData.Add(" ");
                    }
                    cntRow++;
                    foreach (var itemrow11 in _posConfig[3].Parameters)
                    {
                        string itemrow11Name = GetItemName(type3, itemrow11, reportData);
                        if (itemrow11Name == Constants.No_Value) continue;
                        rowData.Add("    " + itemrow11Name);

                        foreach (var itemcol1 in _posConfig[0].Parameters)
                        {
                            string itemcol1Name = GetItemName(type0, itemcol1, reportData);
                            if (_posConfig[1] != null)
                            {
                                foreach (var itemcol11 in _posConfig[1].Parameters)
                                {
                                    string itemcol11Name = GetItemName(type1, itemcol11, reportData);
                                    string data;
                                    try
                                    {
                                        data = results[Tuple.Create(itemcol1Name, itemcol11Name, itemrow1Name, itemrow11Name)];
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        data = Constants.No_Value;
                                    }
                                    rowData.Add(data);
                                }

                            }
                            else
                            {
                                string data;
                                try
                                {
                                    data = results[Tuple.Create(itemcol1Name, string.Empty, itemrow1Name, itemrow11Name)];
                                }
                                catch (KeyNotFoundException)
                                {
                                    data = Constants.No_Value;
                                }
                                rowData.Add(data);
                            }
                        }
                        cntRow++;
                    }
                }
                else
                {
                    foreach (var itemcol1 in _posConfig[0].Parameters)
                    {
                        string itemcol1Name = GetItemName(type0, itemcol1, reportData);
                        if (_posConfig[1] != null)
                        {
                            foreach (var itemcol11 in _posConfig[1].Parameters)
                            {
                                string itemcol11Name = GetItemName(type1, itemcol11, reportData);
                                string data;
                                try
                                {
                                    data = results[Tuple.Create(itemcol1Name, itemcol11Name, itemrow1Name, string.Empty)];
                                }
                                catch (KeyNotFoundException)
                                {
                                    data = Constants.No_Value;
                                }
                                rowData.Add(data);
                            }

                        }
                        else
                        {
                            string data;
                            try
                            {
                                data = results[Tuple.Create(itemcol1Name, string.Empty, itemrow1Name, string.Empty)];
                            }
                            catch (KeyNotFoundException)
                            {
                                data = Constants.No_Value;
                            }
                            rowData.Add(data);
                        }
                    }
                    cntRow++;
                }
            }

            #endregion

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = cntRow,
                NbColumns = cntCol + 1,
                Data = rowData
            };
            return resultTable;

        }

        public static void BuildAggregatedMetricsList(ReportData reportData, Dictionary<string, string> metricsAggregated, List<string> metrics, string[] aggregators)
        {

            if (metrics.Contains("HEALTH_FACTOR"))
            {
                string[] metricList = { "60011", "60012", "60013", "60014", "60016" };
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("HEALTH_FACTOR")] : "AVERAGE";
                }
                catch (KeyNotFoundException)
                {
                    agg = "AVERAGE";
                }

                foreach (string _m in metricList)
                {
                    metricsAggregated.Add(_m, agg);
                }
            }
            if (metrics.Contains("BUSINESS_CRITERIA"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.BusinessCriteriaResults.Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("BUSINESS_CRITERIA")] : "AVERAGE";
                }
                catch (KeyNotFoundException)
                {
                    agg = "AVERAGE";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("TECHNICAL_CRITERIA"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.TechnicalCriteriaResults.Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("TECHNICAL_CRITERIA")] : "AVERAGE";
                }
                catch (KeyNotFoundException)
                {
                    agg = "AVERAGE";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("CRITICAL_QUALITY_RULES"))
            {
                string[] metricList = null;
                var bizRes = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == 60017);
                if (bizRes != null)
                    metricList = bizRes.CriticalRulesViolation.Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("CRITICAL_QUALITY_RULES")] : "AVERAGE";
                }
                catch (KeyNotFoundException)
                {
                    agg = "AVERAGE";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("QUALITY_RULES"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.QualityRulesResults.Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("QUALITY_RULES")] : "AVERAGE";
                }
                catch (KeyNotFoundException)
                {
                    agg = "AVERAGE";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("TECHNICAL_SIZING"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.SizingMeasuresResults.Where(_ => _.Type == "technical-size-measures").Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("TECHNICAL_SIZING")] : "SUM";
                }
                catch (KeyNotFoundException)
                {
                    agg = "SUM";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("FUNCTIONAL_WEIGHT"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.SizingMeasuresResults.Where(_ => _.Type == "functional-weight-measures").Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("FUNCTIONAL_WEIGHT")] : "SUM";
                }
                catch (KeyNotFoundException)
                {
                    agg = "SUM";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("TECHNICAL_DEBT"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.SizingMeasuresResults.Where(_ => _.Type == "technical-debt-statistics").Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("TECHNICAL_DEBT")] : "SUM";
                }
                catch (KeyNotFoundException)
                {
                    agg = "SUM";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("VIOLATION"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.SizingMeasuresResults.Where(_ => _.Type == "violation-statistics").Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("VIOLATION")] : "SUM";
                }
                catch (KeyNotFoundException)
                {
                    agg = "SUM";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("CRITICAL_VIOLATION"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.SizingMeasuresResults.Where(_ => _.Type == "critical-violation-statistics").Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("CRITICAL_VIOLATION")] : "SUM";
                }
                catch (KeyNotFoundException)
                {
                    agg = "SUM";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }
            if (metrics.Contains("RUN_TIME"))
            {
                string[] metricList = reportData.Applications.FirstOrDefault()?.Snapshots.FirstOrDefault()?.SizingMeasuresResults.Where(_ => _.Type == "run-time-statistics").Select(_ => _.Reference.Key.ToString()).ToArray();
                string agg;
                try
                {
                    agg = aggregators != null ? aggregators[metrics.IndexOf("RUN_TIME")] : "SUM";
                }
                catch (KeyNotFoundException)
                {
                    agg = "SUM";
                }
                if (metricList != null)
                {
                    foreach (string _m in metricList)
                    {
                        metricsAggregated.Add(_m, agg);
                    }
                }
            }

            if (metricsAggregated.Count == 0 && metrics.Count > 0)
            {
                // case when configuration contains only id and no groups
                if (aggregators != null)
                {
                    foreach (string _metric in metrics)
                    {
                        try
                        {
                            metricsAggregated.Add(_metric, aggregators[metrics.IndexOf(_metric)]);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // case when only one aggregator is defined for all ids
                            metricsAggregated.Add(_metric, aggregators.Length > 0 ? aggregators.FirstOrDefault() : string.Empty);
                        }
                    }
                }
                else
                {
                    // case when violations and critical violations on a metric id
                    foreach (string _metric in metrics)
                    {
                        metricsAggregated.Add(_metric, string.Empty);
                    }
                }
            }

        }


        public static void BuildApplicationSnapshots(Dictionary<Application, Snapshot> list, ReportData reportData)
        {
            foreach (Application _application in reportData.Applications)
            {
                list.Add(_application, _application.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot).LastOrDefault());
            }
        }

    }
}
