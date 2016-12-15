using System;
using System.Collections.Generic;
using System.ComponentModel;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using System.Linq;
using CastReporting.Reporting.Languages;

namespace CastReporting.Reporting.Block.Table
{
    internal class GenericTable : TableBlock
    {
        public class ObjConfig
        {
            public string Type;
            public string[] Parameters;

        }

        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int cntRow = 0;
            int cntCol = 0;
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
            bool considerModules = false;
            bool considerTechnologies = false;
            
            Dictionary<Tuple<string,string,string,string>,string> names = new Dictionary<Tuple<string, string, string, string>,string>();
            Dictionary<Tuple<string, string, string, string>, string> results = new Dictionary<Tuple<string, string, string, string>, string>();

            #region Get Configuration
            // get the configuration
            string type0 = options.GetOption("COL1");
            _posConfig[0] = new ObjConfig {Type = type0, Parameters = options.GetOption(type0).Split('|')};
            string type1 = options.GetOption("COL11");
            _posConfig[1] = new ObjConfig { Type = type1, Parameters = options.GetOption(type1).Split('|') };
            string type2 = options.GetOption("ROW1");
            _posConfig[2] = new ObjConfig { Type = type2, Parameters = options.GetOption(type2).Split('|') };
            string type3 = options.GetOption("ROW11");
            _posConfig[3] = new ObjConfig { Type = type3, Parameters = options.GetOption(type3).Split('|') };

            string[] snapshotConfiguration = options.GetOption("SNAPSHOTS").Split('|');

            // get the data and calculate results : snapshots, metrics, modules, technologies, violations, critical_violations
            for (int i = 0; i < _posConfig.Length; i++)
            {
                switch (_posConfig[i].Type)
                {
                    case "SNAPSHOTS":
                        if (_posConfig[i].Parameters.Length == 0)
                        {
                            snapshots.Add(reportData.CurrentSnapshot);
                        }
                        else
                        {
                            if (_posConfig[i].Parameters.Contains("CURRENT")) snapshots.Add(reportData.CurrentSnapshot);
                            if (_posConfig[i].Parameters.Contains("PREVIOUS")) snapshots.Add(reportData.PreviousSnapshot);
                            positionSnapshots = i;
                        }
                        break;
                    case "METRICS":
                        if (_posConfig[i].Parameters.Length == 0) return null;
                        metrics.AddRange(_posConfig[i].Parameters);
                        positionMetrics = i;
                        break;
                    case "MODULES":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            considerModules = true;
                            positionModules = i;
                            if (_posConfig[i].Parameters[0] == "ALL")
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null) modules.AddRange(reportData.CurrentSnapshot.Modules);
                                if (snapshotConfiguration.Contains("PREVIOUS") && reportData.PreviousSnapshot != null)
                                {
                                    foreach (Module module in reportData.PreviousSnapshot.Modules)
                                    {
                                        if (!modules.Contains(module)) modules.Add(module);
                                    }
                                }
                            }
                            else
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null) modules.AddRange(reportData.CurrentSnapshot.Modules.Where(_ => _posConfig[i].Parameters.Contains(_.Id.ToString())));
                                if (snapshotConfiguration.Contains("PREVIOUS") && reportData.PreviousSnapshot != null)
                                {
                                    foreach (Module module in reportData.PreviousSnapshot.Modules)
                                    {
                                        if (!modules.Contains(module)) modules.Add(module);
                                    }
                                }
                            }
                        }
                        break;
                    case "TECHNOLOGIES":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            considerTechnologies = true;
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
                            }
                            else
                            {
                                if (snapshotConfiguration.Contains("CURRENT") && reportData.CurrentSnapshot != null) technologies.AddRange(reportData.CurrentSnapshot.Technologies.Where(_ => _posConfig[i].Parameters.Contains(_)));
                                if (snapshotConfiguration.Contains("PREVIOUS") && reportData.PreviousSnapshot != null)
                                {
                                    foreach (string technology in reportData.PreviousSnapshot.Technologies)
                                    {
                                        if (!technologies.Contains(technology)) technologies.Add(technology);
                                    }
                                }
                            }
                        }
                        break;
                    case "VIOLATIONS":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            positionViolations = i;
                            violations.AddRange(_posConfig[i].Parameters);
                        }
                        break;
                    case "CRITICAL_VIOLATIONS":
                        if (_posConfig[i].Parameters.Length != 0)
                        {
                            positionCriticalViolations = i;
                            criticalViolations.AddRange(_posConfig[i].Parameters);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            #region Get Results

            #region Application
            // case application : no modules, no technologies
            // get data for all snapshots in snapshot list
            // if snapshotConfiguration contains EVOL and snapshots list contains 2 snapshots get the difference between the snapshots 
            // if snapshotConfiguration contains EVOL_PERCENT and snapshots list contains 2 snapshots get the differential ratio between the snapshots 
            if (modules.Count == 0 && technologies.Count == 0)
            {
                string[] _posResults = {string.Empty, string.Empty, string.Empty, string.Empty };
                foreach (Snapshot _snapshot in snapshots)
                {
                    foreach (string _metricId in metrics)
                    {
                        // case grade (quality indicator) or value (sizing measure or background fact)
                        if (violations.Count == 0 && criticalViolations.Count == 0)
                        {
                            string value = MetricsUtility.GetMetricResult(reportData, _snapshot, _metricId);
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name;
                            _posResults[positionMetrics] = _metricId;
                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]),value);
                        }

                        // case violations
                        if (violations.Count != 0 && criticalViolations.Count == 0)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name;
                            _posResults[positionMetrics] = _metricId;
                            string value;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(_snapshot, int.Parse(_metricId));
                            if (stat == null) value = Constants.No_Value;
                            foreach (string _violation in violations)
                            {
                                _posResults[positionViolations] = _violation;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        value = stat.TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        value = stat.AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "DELETED":
                                        value = stat.RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                            }
                        }

                        // case critical violations
                        if (violations.Count == 0 && criticalViolations.Count != 0)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name;
                            _posResults[positionMetrics] = _metricId;
                            string value;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(_snapshot, int.Parse(_metricId));
                            if (stat == null) value = Constants.No_Value;
                            foreach (string _violation in criticalViolations)
                            {
                                _posResults[positionCriticalViolations] = _violation;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        value = stat.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        value = stat.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "DELETED":
                                        value = stat.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]), value);
                            }
                        }

                        // functionnaly impossible case
                        if (violations.Count != 0 && criticalViolations.Count != 0)
                        {
                            return null;
                        }
                    }
                }
            }
            #endregion

            #region Modules
            // case modules : no technologies
            if (modules.Count != 0 && technologies.Count == 0)
            {
                string[] _posResults = new string[4];
                foreach (Snapshot _snapshot in snapshots)
                {
                    foreach (string _metricId in metrics)
                    {
                        // case grade
                        if (violations.Count == 0 && criticalViolations.Count == 0)
                        {

                        }

                        // case violations
                        if (violations.Count != 0 && criticalViolations.Count == 0)
                        {
                            // _metricId should be a quality indicator, if not, return null

                        }

                        // case critical violations
                        if (violations.Count == 0 && criticalViolations.Count != 0)
                        {
                            // _metricId should be a quality indicator, if not, return null
                        }

                        // functionnaly impossible case
                        if (violations.Count != 0 && criticalViolations.Count != 0)
                        {
                            return null;
                        }
                    }
                }
            }
            #endregion

            #region Technologies
            // case technologies : no modules
            if (modules.Count == 0 && technologies.Count != 0)
            {
                string[] _posResults = new string[4];
                foreach (Snapshot _snapshot in snapshots)
                {
                    foreach (string _metricId in metrics)
                    {
                        // case grade
                        if (violations.Count == 0 && criticalViolations.Count == 0)
                        {

                        }

                        // case violations
                        if (violations.Count != 0 && criticalViolations.Count == 0)
                        {
                            // _metricId should be a quality indicator, if not, return null

                        }

                        // case critical violations
                        if (violations.Count == 0 && criticalViolations.Count != 0)
                        {
                            // _metricId should be a quality indicator, if not, return null
                        }

                        // functionnaly impossible case
                        if (violations.Count != 0 && criticalViolations.Count != 0)
                        {
                            return null;
                        }
                    }
                }
            }
            #endregion

            #region Modules and Technologies

            // case modules and technologies
            if (modules.Count != 0 && technologies.Count != 0)
            {
                string[] _posResults = new string[4];
                foreach (Snapshot _snapshot in snapshots)
                {
                    foreach (string _metricId in metrics)
                    {
                        // case grade
                        if (violations.Count == 0 && criticalViolations.Count == 0)
                        {

                        }

                        // case violations
                        if (violations.Count != 0 && criticalViolations.Count == 0)
                        {
                            // _metricId should be a quality indicator, if not, return null

                        }

                        // case critical violations
                        if (violations.Count == 0 && criticalViolations.Count != 0)
                        {
                            // _metricId should be a quality indicator, if not, return null
                        }

                        // functionnaly impossible case
                        if (violations.Count != 0 && criticalViolations.Count != 0)
                        {
                            return null;
                        }
                    }
                }
            }
            #endregion            

            #endregion

            #region Get Display Data
            // define the table content in configuration order




            #endregion

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = cntRow + 1,
                NbColumns = cntCol + 1,
                Data = rowData
            };
            return resultTable;

        }
    }
}
