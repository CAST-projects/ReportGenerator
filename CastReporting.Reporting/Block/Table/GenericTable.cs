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
                        if (_posConfig[i].Parameters.Length == 0)
                        {
                            snapshots.Add(reportData.CurrentSnapshot);
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
                        metrics.AddRange(_posConfig[i].Parameters);
                        positionMetrics = i;
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
                foreach (Snapshot _snapshot in snapshots)
                {
                    foreach (string _metricId in metrics)
                    {
                        string[] _posResults = { string.Empty, string.Empty, string.Empty, string.Empty };

                        // case grade (quality indicator) or value (sizing measure or background fact)
                        if (violations.Count == 0 && criticalViolations.Count == 0)
                        {
                            string value = MetricsUtility.GetMetricResult(reportData, _snapshot, _metricId);
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = _metricId;
                            results.Add(Tuple.Create(_posResults[0], _posResults[1], _posResults[2], _posResults[3]),value);
                        }

                        // case violations
                        if (violations.Count != 0 && criticalViolations.Count == 0)
                        {
                            // _metricId should be a quality indicator, if not, return null
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = _metricId;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(_snapshot, int.Parse(_metricId));
                            foreach (string _violation in violations)
                            {
                                _posResults[positionViolations] = _violation;
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        value = stat?.TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        value = stat?.AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "DELETED":
                                        value = stat?.RemovedViolations?.ToString("N0") ?? Constants.No_Value;
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
                            if (positionSnapshots != -1) _posResults[positionSnapshots] = _snapshot.Name + " - " + _snapshot.Annotation.Version;
                            _posResults[positionMetrics] = _metricId;
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(_snapshot, int.Parse(_metricId));
                            foreach (string _violation in criticalViolations)
                            {
                                _posResults[positionCriticalViolations] = _violation;
                                string value;
                                switch (_violation)
                                {
                                    case "TOTAL":
                                        value = stat?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "ADDED":
                                        value = stat?.AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                        break;
                                    case "DELETED":
                                        value = stat?.RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
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
            /*
             * Add in rowData :
             * - row1.type.name (1)
             * - foreach itemcol1 in col1
             * ---- foreach itemcol11 in col11 (if col11 not null)
             * -------- itemcol1.name - itemcol11.name (2)
             * - foreach itemrow1 in row1
             * ---- itemrow1.name (2)
             * ---- foreach itemrow11 in row11 (if row11 not null)
             * -------- add as much spaces than number of column minus one
             * -------- '    ' + itemrow11.name (2)
             * -------- then or if row11 is null same process
             * -------- foreach itemcol1 in col1
             * ------------ foreach itemcol11 in col11 (if col11 not null)
             * ----------------- results[Tuple.Create(itemcol1, itemcol11, itemrow1, itemrow11)] (3)
             * if col11 is null replace itemcol11 by "", idem for itemrow11 if row11 is null
             * 
             * 
             * (1) create a function get type name, switching in the different possibilities : SNAPSHOTS, METRICS, MODULES, TECHNOLOGIES, VIOLATIONS, CRITICAL_VIOLATIONS, to get a proper name by language
             * (2) here we have to reconstitute the name depending on the type and the value
             * (3) be careful, the item should correspond to those that have been used to save the data, depending on the type, perhaps use the name and change the _metricId by the metric name lines 173, 184, 213
             */
             
            rowData.Add(type2);
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
            cntCol = _posConfig[0].Parameters.Length * _posConfig[1]?.Parameters.Length ?? _posConfig[0].Parameters.Length;
            cntRow = 1;
            foreach (var itemrow1 in _posConfig[2].Parameters)
            {
                string itemrow1Name = GetItemName(type2, itemrow1, reportData);
                rowData.Add(itemrow1Name);
                if (_posConfig[3] != null)
                {
                    for (int s = 1; s < cntCol; s++)
                    {
                        rowData.Add(" ");
                    }
                    cntRow++;
                    foreach (var itemrow11 in _posConfig[3].Parameters)
                    {
                        string itemrow11Name = GetItemName(type3, itemrow11, reportData);
                        rowData.Add(itemrow11Name);
                        
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
                    Snapshot _snapshot = null;
                    switch (item)
                    {
                        case "CURRENT":
                            _snapshot = reportData.CurrentSnapshot;
                            break;
                        case "PREVIOUS":
                            _snapshot = reportData.PreviousSnapshot;
                            break;
                    }
                    return _snapshot?.Name + " - " + _snapshot?.Annotation.Version;
                case "METRICS":
                    break;
                case "MODULES":
                    break;
                case "TECHNOLOGIES":
                    break;
                case "VIOLATIONS":
                    switch (item)
                    {
                        case "ADDED":
                            break;
                        case "DELETED":
                            break;
                        case "TOTAL":
                            break;
                    }
                    break;
                case "CRITICAL_VIOLATIONS":
                    switch (item)
                    {
                        case "ADDED":
                            break;
                        case "DELETED":
                            break;
                        case "TOTAL":
                            break;
                    }
                    break;
            }
            return item;
        }
    }
}
