using System;
using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using System.Linq;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing.DTO;

namespace CastReporting.Reporting.Block.Table
{
    [Block("GENERIC_TABLE")]
    internal class GenericTable : TableBlock
    {
        public class ObjConfig
        {
            public string Type;
            public string[] Parameters;
        }

        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            var rowData = new List<string>();
            ObjConfig[] _posConfig = new ObjConfig[4];
            List<Snapshot> snapshots = new List<Snapshot>();
            int positionSnapshots = -1;
            List<Module> modules = new List<Module>();
            int positionModules = -1;
            List<string> metrics = new List<string>();
            int positionMetrics = -1;
            List<string> technologies = new List<string>();
            int positionTechnologies = -1;
            List<string> violations = new List<string>();
            int positionViolations = -1;
            List<string> criticalViolations = new List<string>();
            int positionCriticalViolations = -1;
            
            Dictionary<Tuple<string, string, string, string>, string> results = new Dictionary<Tuple<string, string, string, string>, string>();

            #region Get Configuration
            // get the configuration
            string type0 = options.GetOption("COL1");
            _posConfig[0] = type0 != null ? new ObjConfig {Type = type0, Parameters = options.GetOption(type0).Split('|')} : null;
            string type1 = options.GetOption("COL11");
            _posConfig[1] = type1 != null ? new ObjConfig { Type = type1, Parameters = options.GetOption(type1).Split('|') } : null;
            string type2 = options.GetOption("ROW1");
            _posConfig[2] = type2 != null ? new ObjConfig { Type = type2, Parameters = options.GetOption(type2).Split('|') } : null;
            string type3 = options.GetOption("ROW11");
            _posConfig[3] = type3 != null ? new ObjConfig { Type = type3, Parameters = options.GetOption(type3).Split('|') } : null;

            string[] snapshotConfiguration = options.GetOption("SNAPSHOTS")?.Split('|');

            // get the data and calculate results : snapshots, metrics, modules, technologies, violations, critical_violations
            for (int i = 0; i < _posConfig.Length; i++)
            {
                if (_posConfig[i] == null) continue;
                switch (_posConfig[i].Type)
                {
                    case "SNAPSHOTS":
                        if (_posConfig[i].Parameters.Length == 0 || (_posConfig[i].Parameters.Length == 1 && _posConfig[i].Parameters[0]=="ALL"))
                        {
                            if (reportData.CurrentSnapshot != null)
                            {
                                snapshotConfiguration = new[] {"CURRENT"};
                                snapshots.Add(reportData.CurrentSnapshot);
                                if (reportData.PreviousSnapshot != null)
                                {
                                    snapshotConfiguration = new[] { "CURRENT", "PREVIOUS", "EVOL", "EVOL_PERCENT"};
                                    snapshots.Add(reportData.PreviousSnapshot);
                                }
                            }
                        }
                        else
                        {
                            if (_posConfig[i].Parameters.Contains("CURRENT") && reportData.CurrentSnapshot != null) snapshots.Add(reportData.CurrentSnapshot);
                            if (_posConfig[i].Parameters.Contains("PREVIOUS") && reportData.PreviousSnapshot != null) snapshots.Add(reportData.PreviousSnapshot);
                            positionSnapshots = i;
                        }
                        break;
                    case "METRICS":
                        if (_posConfig[i].Parameters.Length == 0) return null;
                        positionMetrics = i;
                        metrics.AddRange(_posConfig[i].Parameters);
                        if (metrics.Contains("HEALTH_FACTOR"))
                        {
                            metrics.Remove("HEALTH_FACTOR");
                            metrics.AddRange(new []{"60011","60012","60013","60014","60016"});
                        }
                        if (metrics.Contains("BUSINESS_CRITERIA"))
                        {
                            metrics.Remove("BUSINESS_CRITERIA");
                            metrics.AddRange(reportData.CurrentSnapshot.BusinessCriteriaResults.Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("TECHNICAL_CRITERIA"))
                        {
                            metrics.Remove("TECHNICAL_CRITERIA");
                            metrics.AddRange(reportData.CurrentSnapshot.TechnicalCriteriaResults.Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("TECHNICAL_SIZING"))
                        {
                            metrics.Remove("TECHNICAL_SIZING");
                            metrics.AddRange(reportData.CurrentSnapshot.SizingMeasuresResults.Where(_ => _.Type == "technical-size-measures").Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("FUNCTIONAL_WEIGHT"))
                        {
                            metrics.Remove("FUNCTIONAL_WEIGHT");
                            metrics.AddRange(reportData.CurrentSnapshot.SizingMeasuresResults.Where(_ => _.Type == "functional-weight-measures").Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("TECHNICAL_DEBT"))
                        {
                            metrics.Remove("TECHNICAL_DEBT");
                            metrics.AddRange(reportData.CurrentSnapshot.SizingMeasuresResults.Where(_ => _.Type == "technical-debt-statistics").Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("VIOLATIONS_STATS"))
                        {
                            metrics.Remove("VIOLATIONS_STATS");
                            metrics.AddRange(reportData.CurrentSnapshot.SizingMeasuresResults.Where(_ => _.Type == "violation-statistics").Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("CRITICAL_VIOLATIONS_STATS"))
                        {
                            metrics.Remove("CRITICAL_VIOLATIONS_STATS");
                            metrics.AddRange(reportData.CurrentSnapshot.SizingMeasuresResults.Where(_ => _.Type == "critical-violation-statistics").Select(_ => _.Reference.Key.ToString()));
                        }
                        if (metrics.Contains("RUN_TIME_STATS"))
                        {
                            metrics.Remove("RUN_TIME_STATS");
                            metrics.AddRange(reportData.CurrentSnapshot.SizingMeasuresResults.Where(_ => _.Type == "run-time-statistics").Select(_ => _.Reference.Key.ToString()));
                        }
                        break;
                    case "MODULES":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            positionModules = i;
                            if (_posConfig[i].Parameters[0] == "ALL")
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null) modules.AddRange(reportData.CurrentSnapshot.Modules);
                                if (snapshotConfiguration.Contains("PREVIOUS") && reportData.PreviousSnapshot != null)
                                {
                                    foreach (Module module in reportData.PreviousSnapshot.Modules)
                                    {
                                        string name = modules.FirstOrDefault(_ => _.Id == module.Id).Name;
                                        if (name == null) modules.Add(module);
                                    }
                                }
                                string[] paramModules = new string[modules.Count];
                                for (int j = 0; j < modules.Count; j++)
                                {
                                    paramModules[j] = modules[j].Name;
                                }
                                _posConfig[i].Parameters = paramModules;
                            }
                            else
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null)
                                {
                                    modules.AddRange(reportData.CurrentSnapshot.Modules.Where(_ => _posConfig[i].Parameters.Contains(_.Name)));
                                }
                                string[] paramModules = new string[modules.Count];
                                for (int j = 0; j < modules.Count; j++)
                                {
                                    paramModules[j] = modules[j].Name;
                                }
                                _posConfig[i].Parameters = paramModules;
                            }
                        }
                        break;
                    case "TECHNOLOGIES":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            positionTechnologies = i;
                            if (_posConfig[i].Parameters[0] == "ALL")
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null) technologies.AddRange(reportData.CurrentSnapshot.Technologies);
                                if (snapshotConfiguration.Contains("PREVIOUS") && reportData.PreviousSnapshot != null)
                                {
                                    foreach (string technology in reportData.PreviousSnapshot.Technologies)
                                    {
                                        if (!technologies.Contains(technology)) technologies.Add(technology);
                                    }
                                }
                                string[] paramTechnos = new string[technologies.Count];
                                for (int j = 0; j < technologies.Count; j++)
                                {
                                    paramTechnos[j] = technologies[j];
                                }
                                _posConfig[i].Parameters = paramTechnos;
                            }
                            else
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null)
                                {
                                    technologies.AddRange(reportData.CurrentSnapshot.Technologies.Where(_ => _posConfig[i].Parameters.Contains(_)));
                                }
                            }
                        }
                        break;
                    case "VIOLATIONS":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            positionViolations = i;
                            if (_posConfig[i].Parameters.Length == 1 && _posConfig[i].Parameters[0] == "ALL")
                            {
                                violations.AddRange(new[] { "TOTAL", "ADDED", "REMOVED" });
                            }
                            else
                            {
                                violations.AddRange(_posConfig[i].Parameters);
                            }
                            
                        }
                        else
                        {
                            positionViolations = i;
                            violations.AddRange(new[] { "TOTAL", "ADDED", "REMOVED" });
                        }
                        break;
                    case "CRITICAL_VIOLATIONS":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            positionCriticalViolations = i;
                            if (_posConfig[i].Parameters.Length == 1 && _posConfig[i].Parameters[0] == "ALL")
                            {
                                criticalViolations.AddRange(new[] { "TOTAL", "ADDED", "REMOVED" });
                            }
                            else
                            {
                                criticalViolations.AddRange(_posConfig[i].Parameters);
                            }
                        }
                        else
                        {
                            positionViolations = i;
                            criticalViolations.AddRange(new[] { "TOTAL", "ADDED", "REMOVED" });
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // case implicit snapshots
            if (snapshots.Count == 0 && snapshotConfiguration.Contains("CURRENT") && snapshotConfiguration.Contains("PREVIOUS") && reportData.CurrentSnapshot != null) snapshots.Add(reportData.CurrentSnapshot);
            if (snapshots.Count == 0 && snapshotConfiguration.Contains("CURRENT") && !snapshotConfiguration.Contains("PREVIOUS") && reportData.CurrentSnapshot != null) snapshots.Add(reportData.CurrentSnapshot);
            if (snapshots.Count == 0 && !snapshotConfiguration.Contains("CURRENT") && snapshotConfiguration.Contains("PREVIOUS") && reportData.PreviousSnapshot != null) snapshots.Add(reportData.PreviousSnapshot);
            if (snapshots.Count == 0 && reportData.CurrentSnapshot != null) snapshots.Add(reportData.CurrentSnapshot);
            #endregion

            #region Get Results

            #region Application
            // case application : no modules, no technologies
            // get data for all snapshots in snapshot list
            // if snapshotConfiguration contains EVOL and snapshots list contains 2 snapshots get the difference between the snapshots 
            // if snapshotConfiguration contains EVOL_PERCENT and snapshots list contains 2 snapshots get the differential ratio between the snapshots 
            if (modules.Count == 0 && technologies.Count == 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };

                    // case grade (quality indicator) or value (sizing measure or background fact)
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metrics)
                    {
                        EvolutionResult res = MetricsUtility.GetMetricEvolution(reportData, reportData.CurrentSnapshot, reportData.PreviousSnapshot, _metricId, true, null, string.Empty);
                        _posResults[positionMetrics] = res.name;
                        foreach (string param in snapshotConfiguration)
                        {
                            switch (param)
                            {
                                case "CURRENT":
                                    if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.CurrentSnapshot.Name + " - " + reportData.CurrentSnapshot.Annotation.Version;
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.curResult);
                                    break;
                                case "PREVIOUS":
                                    if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.PreviousSnapshot.Name + " - " + reportData.PreviousSnapshot.Annotation.Version;
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.prevResult);
                                    break;
                                case "EVOL":
                                    if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.Evolution;
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolution);
                                    break;
                                case "EVOL_PERCENT":
                                    if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.EvolutionPercent;
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolutionPercent);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }

                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(_snapshot, int.Parse(_metricId));
                            foreach (string _violation in violations)
                            {
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        _posResults[positionViolations] = Labels.TotalViolations;
                                        value = stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        _posResults[positionViolations] = Labels.AddedViolations;
                                        value = stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "REMOVED":
                                        _posResults[positionViolations] = Labels.RemovedViolations;
                                        value = stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                            }
                        }
                    }
                }

                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(_snapshot, int.Parse(_metricId));
                            foreach (string _violation in criticalViolations)
                            {
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                        value = stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                        value = stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "REMOVED":
                                        _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                        value = stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
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

            #region Modules
            // case modules : no technologies
            if (modules.Count != 0 && technologies.Count == 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                // case grade
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metrics)
                    {
                        foreach (Module module in modules)
                        {
                            _posResults[positionModules] = module.Name;
                            EvolutionResult res = MetricsUtility.GetMetricEvolution(reportData, reportData.CurrentSnapshot, reportData.PreviousSnapshot, _metricId, true, module, string.Empty);
                            _posResults[positionMetrics] = res.name;
                            foreach (string param in snapshotConfiguration)
                            {
                                switch (param)
                                {
                                    case "CURRENT":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.CurrentSnapshot.Name + " - " + reportData.CurrentSnapshot.Annotation.Version;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.curResult);
                                        break;
                                    case "PREVIOUS":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.PreviousSnapshot.Name + " - " + reportData.PreviousSnapshot.Annotation.Version;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.prevResult);
                                        break;
                                    case "EVOL":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.Evolution;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolution);
                                        break;
                                    case "EVOL_PERCENT":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.EvolutionPercent;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolutionPercent);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                }

                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            foreach (Module _module in modules)
                            {
                                _posResults[positionModules] = _module.Name;
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatModule(_snapshot, _module.Id, int.Parse(_metricId));
                                foreach (string _violation in violations)
                                {
                                    string value;
                                    switch (_violation)
                                    {
                                        case "TOTAL":
                                            _posResults[positionViolations] = Labels.TotalViolations;
                                            value = stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "ADDED":
                                            _posResults[positionViolations] = Labels.AddedViolations;
                                            value = stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "REMOVED":
                                            _posResults[positionViolations] = Labels.RemovedViolations;
                                            value = stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                }

                            }
                        }
                    }

                }

                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            foreach (Module _module in modules)
                            {
                                _posResults[positionModules] = _module.Name;
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatModule(_snapshot, _module.Id, int.Parse(_metricId));
                                foreach (string _violation in criticalViolations)
                                {
                                    string value;
                                    switch (_violation)
                                    {
                                        case "TOTAL":
                                            _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                            value = stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "ADDED":
                                            _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                            value = stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "REMOVED":
                                            _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                            value = stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
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
            // case technologies : no modules
            if (modules.Count == 0 && technologies.Count != 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                // case grade
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metrics)
                    {
                        foreach (string techno in technologies)
                        {
                            _posResults[positionTechnologies] = techno;
                            EvolutionResult res = MetricsUtility.GetMetricEvolution(reportData, reportData.CurrentSnapshot, reportData.PreviousSnapshot, _metricId, true, null, techno);
                            _posResults[positionMetrics] = res.name;
                            foreach (string param in snapshotConfiguration)
                            {
                                switch (param)
                                {
                                    case "CURRENT":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.CurrentSnapshot.Name + " - " + reportData.CurrentSnapshot.Annotation.Version;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.curResult);
                                        break;
                                    case "PREVIOUS":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.PreviousSnapshot.Name + " - " + reportData.PreviousSnapshot.Annotation.Version;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.prevResult);
                                        break;
                                    case "EVOL":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.Evolution;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolution);
                                        break;
                                    case "EVOL_PERCENT":
                                        if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.EvolutionPercent;
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolutionPercent);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                }

                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            foreach (string techno in technologies)
                            {
                                _posResults[positionTechnologies] = techno;
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatTechno(_snapshot, techno, int.Parse(_metricId));
                                foreach (string _violation in violations)
                                {
                                    string value;
                                    switch (_violation)
                                    {
                                        case "TOTAL":
                                            _posResults[positionViolations] = Labels.TotalViolations;
                                            value = stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "ADDED":
                                            _posResults[positionViolations] = Labels.AddedViolations;
                                            value = stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "REMOVED":
                                            _posResults[positionViolations] = Labels.RemovedViolations;
                                            value = stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                                }

                            }
                        }
                    }

                }

                // case critical violations
                if (violations.Count == 0 && criticalViolations.Count != 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            foreach (string techno in technologies)
                            {
                                _posResults[positionTechnologies] = techno;
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatTechno(_snapshot, techno, int.Parse(_metricId));
                                foreach (string _violation in criticalViolations)
                                {
                                    string value;
                                    switch (_violation)
                                    {
                                        case "TOTAL":
                                            _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                            value = stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "ADDED":
                                            _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                            value = stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        case "REMOVED":
                                            _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                            value = stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
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

            #region Modules and Technologies

            // case modules and technologies
            if (modules.Count != 0 && technologies.Count != 0)
            {
                string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };
                // case grade
                if (violations.Count == 0 && criticalViolations.Count == 0)
                {
                    foreach (string _metricId in metrics)
                    {
                        foreach (Module module in modules)
                        {
                            _posResults[positionModules] = module.Name;
                            foreach (string techno in technologies)
                            {
                                _posResults[positionTechnologies] = techno;
                                EvolutionResult res = MetricsUtility.GetMetricEvolution(reportData, reportData.CurrentSnapshot, reportData.PreviousSnapshot, _metricId, true, module, techno);
                                _posResults[positionMetrics] = res.name;
                                foreach (string param in snapshotConfiguration)
                                {
                                    switch (param)
                                    {
                                        case "CURRENT":
                                            if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.CurrentSnapshot.Name + " - " + reportData.CurrentSnapshot.Annotation.Version;
                                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.curResult);
                                            break;
                                        case "PREVIOUS":
                                            if (positionSnapshots != -1) _posResults[positionSnapshots] = reportData.PreviousSnapshot.Name + " - " + reportData.PreviousSnapshot.Annotation.Version;
                                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.prevResult);
                                            break;
                                        case "EVOL":
                                            if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.Evolution;
                                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolution);
                                            break;
                                        case "EVOL_PERCENT":
                                            if (positionSnapshots != -1) _posResults[positionSnapshots] = Labels.EvolutionPercent;
                                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), res.evolutionPercent);
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                            }
                        }
                    }
                }

                // case violations
                if (violations.Count != 0 && criticalViolations.Count == 0)
                {
                    // _metricId should be a quality indicator, if not, return null
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            foreach (Module _module in modules)
                            {
                                _posResults[positionModules] = _module.Name;
                                foreach (string techno in technologies)
                                {
                                    _posResults[positionTechnologies] = techno;
                                    ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatModuleTechno(_snapshot, _module.Id, techno, int.Parse(_metricId));
                                    foreach (string _violation in violations)
                                    {
                                        string value;
                                        switch (_violation)
                                        {
                                            case "TOTAL":
                                                _posResults[positionViolations] = Labels.TotalViolations;
                                                value = stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                                break;
                                            case "ADDED":
                                                _posResults[positionViolations] = Labels.AddedViolations;
                                                value = stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                                break;
                                            case "REMOVED":
                                                _posResults[positionViolations] = Labels.RemovedViolations;
                                                value = stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
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
                    foreach (string _metricId in metrics)
                    {
                        foreach (Snapshot _snapshot in snapshots)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = MetricsUtility.GetMetricName(reportData, _snapshot, _metricId);
                            foreach (Module _module in modules)
                            {
                                _posResults[positionModules] = _module.Name;
                                foreach (string techno in technologies)
                                {
                                    _posResults[positionTechnologies] = techno;
                                    ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatModuleTechno(_snapshot, _module.Id, techno, int.Parse(_metricId));
                                    foreach (string _violation in criticalViolations)
                                    {
                                        string value;
                                        switch (_violation)
                                        {
                                            case "TOTAL":
                                                _posResults[positionCriticalViolations] = Labels.TotalCriticalViolations;
                                                value = stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                                break;
                                            case "ADDED":
                                                _posResults[positionCriticalViolations] = Labels.AddedCriticalViolations;
                                                value = stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                                break;
                                            case "REMOVED":
                                                _posResults[positionCriticalViolations] = Labels.RemovedCriticalViolations;
                                                value = stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
                                        results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
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

            #endregion

            #region Get Display Data
            // define the table content in configuration order
            /*
             * Add in rowData :
             * - row1.type.name (1)
             * - foreach itemcol1 in col1
             * ---- foreach itemcol11 in col11 (if col11 not null)
             * -------- itemcol1.name - itemcol11.name (2)
             * - foreach itemrow1 in row1
             * ---- itemrow1.name (2)
             * ---- if row11 not null
             * -------- add as much spaces than number of column minus one
             * -------- foreach itemrow11 in row11 (if row11 not null)
             * ------------ '    ' + itemrow11.name (2)
             * ------------ then or if row11 is null same process
             * ------------ foreach itemcol1 in col1
             * ---------------- foreach itemcol11 in col11 (if col11 not null)
             * -------------------- results[Tuple.Create(itemcol1, itemcol11, itemrow1, itemrow11)] (3)
             * if col11 is null replace itemcol11 by "", idem for itemrow11 if row11 is null
             * 
             * (1) create a function get type name, switching in the different possibilities : SNAPSHOTS, METRICS, MODULES, TECHNOLOGIES, VIOLATIONS, CRITICAL_VIOLATIONS, to get a proper name by language
             * (2) here we have to reconstitute the name depending on the type and the value
             * (3) be careful, the item should correspond to those that have been used to save the data, depending on the type, perhaps use the name and change the _metricId by the metric name lines 173, 184, 213
             */

            rowData.Add(GetTypeName(type2));
            foreach (var itemcol1 in _posConfig[0].Parameters)
            {
                string _col1Name = GetItemName(type0, itemcol1,reportData);
                if (_posConfig[1] != null)
                {
                    rowData.AddRange(_posConfig[1].Parameters.Select(itemcol11 => GetItemName(type1, itemcol11,reportData)).Select(col11Name => _col1Name + " - " + col11Name));
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
                        rowData.Add("    " + itemrow11Name);
                        
                        foreach (var itemcol1 in _posConfig[0].Parameters)
                        {
                            string itemcol1Name = GetItemName(type0, itemcol1, reportData);
                            if (_posConfig[1] != null)
                            {
                                foreach (var itemcol11 in _posConfig[1].Parameters)
                                {
                                    string itemcol11Name = GetItemName(type1, itemcol11, reportData);
                                    rowData.Add(results[Tuple.Create(itemcol1Name, itemcol11Name, itemrow1Name, itemrow11Name)]);
                                }
                                
                            }
                            else
                            {
                                rowData.Add(results[Tuple.Create(itemcol1Name, string.Empty, itemrow1Name, itemrow11Name)]);
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
                                rowData.Add(results[Tuple.Create(itemcol1Name, itemcol11Name, itemrow1Name, string.Empty)]);
                            }

                        }
                        else
                        {
                            rowData.Add(results[Tuple.Create(itemcol1Name, string.Empty, itemrow1Name, string.Empty)]);
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


        protected string GetItemName(string type, string item, ReportData reportData)
        {
            // snapshots, metrics, modules, technologies, violations, critical_violations
            switch (type)
            {
                case "SNAPSHOTS":
                    switch (item)
                    {
                        case "CURRENT":
                            return reportData.CurrentSnapshot?.Name + " - " + reportData.CurrentSnapshot?.Annotation.Version;
                        case "PREVIOUS":
                            return reportData.PreviousSnapshot?.Name + " - " + reportData.PreviousSnapshot?.Annotation.Version;
                        case "EVOL":
                            return Labels.Evolution;
                        case "EVOL_PERCENT":
                            return Labels.EvolutionPercent;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                case "METRICS":
                    return MetricsUtility.GetMetricName(reportData, reportData.CurrentSnapshot, item) ?? Constants.No_Value;
                case "MODULES":
                    return item;
                case "TECHNOLOGIES":
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected string GetTypeName(string type)
        {
            // snapshots, metrics, modules, technologies, violations, critical_violations
            switch (type)
            {
                case "SNAPSHOTS":
                    return Labels.Snapshots;
                case "METRICS":
                    return Labels.Metrics;
                case "MODULES":
                    return Labels.Modules;
                case "TECHNOLOGIES":
                    return Labels.Technologies;
                case "VIOLATIONS":
                    return Labels.Violations;
                case "CRITICAL_VIOLATIONS":
                    return Labels.ViolationsCritical;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
