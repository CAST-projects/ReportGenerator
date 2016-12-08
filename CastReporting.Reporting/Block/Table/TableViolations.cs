/*
 *   Copyright (c) 2016 CAST
 *
 */

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
    [Block("TABLE_VIOLATIONS")]
    internal class TableViolations : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS

            var rowData = new List<string>();
            int cntRow = 0;

            // by default, short names
            bool displayShortHeader = options.GetBoolOption("HEADER");

            string[] idList = options.GetOption("ID")?.Split('|');

            string _snapshot = options.GetOption("SNAPSHOT", "BOTH"); // SNAPSHOT can be CURRENT, PREVIOUS or BOTH
            string _level = options.GetOption("LEVEL", "APPLICATION"); // LEVEL can be APPLICATION, MODULES or TECHNOLOGIES
            bool critical = options.GetBoolOption("CRITICAL", true); // criticity to display critical or total violations
            bool delta = options.GetBoolOption("DELTA", true); // delta to display added and removed violations


            if (reportData?.CurrentSnapshot?.BusinessCriteriaResults == null) return null;
            if (idList == null) return null;

            bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;

            #region Get Names

            int cntMetric = 0;
            Dictionary<string, string> names = new Dictionary<string, string>();
            foreach (string id in idList)
            {
                if (names.Keys.Contains(id)) continue;
                names[id] = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(id), displayShortHeader);
                cntMetric++;
            }

            rowData.Add(" ");
            rowData.AddRange(names.Values);

            #endregion

            #region CurrentSnapshot

            if (_snapshot == "CURRENT" || _snapshot == "BOTH")
            {
                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                rowData.Add(currSnapshotLabel);
                for (int i = 0; i < cntMetric; i++) rowData.Add(" ");

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (_level)
                {
                    case "APPLICATION":
                        Dictionary<string,ViolStatMetricIdDTO> curStats = new Dictionary<string,ViolStatMetricIdDTO>();
                        foreach (string id in idList)
                        {
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(reportData.CurrentSnapshot, int.Parse(id.Trim()));
                            curStats[id] = stat;
                        }
                        string[] totalRange = new string[cntMetric + 1];
                        string[] addedRange = new string[cntMetric + 1];
                        string[] removedRange = new string[cntMetric + 1];
                        if (critical)
                        {
                            totalRange[0] = Labels.ViolationsCritical;
                            if (delta)
                            {
                                addedRange[0] = Labels.ViolationsAdded;
                                removedRange[0] = Labels.ViolationsRemoved;
                            }
                            int k = 1;
                            foreach (string id in curStats.Keys)
                            {
                                totalRange[k] = curStats[id].TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    addedRange[k] = curStats[id].AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                    removedRange[k] = curStats[id].RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                        }
                        else
                        {
                            totalRange[0] = Labels.ViolationsCurrent;
                            if (delta)
                            {
                                addedRange[0] = Labels.ViolationsAdded;
                                removedRange[0] = Labels.ViolationsRemoved;
                            }
                            int k = 1;
                            foreach (string id in curStats.Keys)
                            {
                                totalRange[k] = curStats[id].TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    addedRange[k] = curStats[id].AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                    removedRange[k] = curStats[id].RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                        }
                        rowData.AddRange(totalRange);
                        cntRow++;
                        if (delta)
                        {
                            rowData.AddRange(addedRange);
                            cntRow++;
                            rowData.AddRange(removedRange);
                            cntRow++;
                        }
                        break;
                    case "MODULES":
                        // Dictionary<int, Dictionary<string,ViolStatMetricIdDTO>> curStatsModules = new Dictionary<int, Dictionary<string, ViolStatMetricIdDTO>>(); 
                        List<Module> curModules = reportData.CurrentSnapshot.Modules.Distinct()?.ToList();
                        foreach (Module module in curModules)
                        {
                            Dictionary <string,ViolStatMetricIdDTO> curStatsMod = new Dictionary<string, ViolStatMetricIdDTO>(); 
                            foreach (string id in idList)
                            {
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatModule(reportData.CurrentSnapshot, module.Id, int.Parse(id.Trim()));
                                curStatsMod[id] = stat;
                            }
                            // curStatsModules[module.Id] = curStatsMod;

                            string[] totalModuleRange = new string[cntMetric + 1];
                            string[] addedModuleRange = new string[cntMetric + 1];
                            string[] removedModuleRange = new string[cntMetric + 1];

                            totalModuleRange[0] = module.Name;
                            int k = 1;
                            foreach (string id in curStatsMod.Keys)
                            {
                                totalModuleRange[k] = critical 
                                    ? curStatsMod[id].TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value 
                                    : curStatsMod[id].TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    addedModuleRange[0] = Labels.ViolationsAdded;
                                    addedModuleRange[k] = critical
                                        ? curStatsMod[id].AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : curStatsMod[id].AddedViolations?.ToString("N0") ?? Constants.No_Value;

                                    removedModuleRange[0] = Labels.ViolationsRemoved;
                                    removedModuleRange[k] = critical
                                        ? curStatsMod[id].RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : curStatsMod[id].RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                            rowData.AddRange(totalModuleRange);
                            if (delta)
                            {
                                rowData.AddRange(addedModuleRange);
                                cntRow++;
                                rowData.AddRange(removedModuleRange);
                                cntRow++;
                                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                            }
                        }
                        break;
                    case "TECHNOLOGIES":
                        List<string> curTechnos = reportData.CurrentSnapshot.Technologies?.ToList();
                        // Dictionary<int, Dictionary<string,ViolStatMetricIdDTO>> curStatsTechnologies = new Dictionary<int, Dictionary<string, ViolStatMetricIdDTO>>(); 
                        foreach (string techno in curTechnos)
                        {
                            Dictionary<string, ViolStatMetricIdDTO> curStatsTech = new Dictionary<string, ViolStatMetricIdDTO>();
                            foreach (string id in idList)
                            {
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatTechno(reportData.CurrentSnapshot, techno, int.Parse(id.Trim()));
                                curStatsTech[id] = stat;
                            }
                            // curStatsTechnologies[module.Id] = curStatsMod;

                            string[] totalTechnoRange = new string[cntMetric + 1];
                            string[] addedTechnoRange = new string[cntMetric + 1];
                            string[] removedTechnoRange = new string[cntMetric + 1];

                            totalTechnoRange[0] = techno;
                            int k = 1;
                            foreach (string id in curStatsTech.Keys)
                            {
                                totalTechnoRange[k] = critical
                                    ? curStatsTech[id].TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                    : curStatsTech[id].TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    addedTechnoRange[0] = Labels.ViolationsAdded;
                                    addedTechnoRange[k] = critical
                                        ? curStatsTech[id].AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : curStatsTech[id].AddedViolations?.ToString("N0") ?? Constants.No_Value;

                                    removedTechnoRange[0] = Labels.ViolationsRemoved;
                                    removedTechnoRange[k] = critical
                                        ? curStatsTech[id].RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : curStatsTech[id].RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                            rowData.AddRange(totalTechnoRange);
                            if (delta)
                            {
                                rowData.AddRange(addedTechnoRange);
                                cntRow++;
                                rowData.AddRange(removedTechnoRange);
                                cntRow++;
                                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                            }
                            
                        }
                        break;
                }
            }

            #endregion

            if (_snapshot == "BOTH" && hasPreviousSnapshot && (!delta || _level == "APPLICATION"))
            {
                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                cntRow++;
            }

            if (_snapshot == "PREVIOUS" && !hasPreviousSnapshot)
            {
                rowData.Add(Constants.No_Value);
                for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                cntRow++;
            }

            #region PreviousSnapshot

            if (hasPreviousSnapshot && (_snapshot == "PREVIOUS" || _snapshot == "BOTH"))
            {
                string prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot);
                rowData.Add(prevSnapshotLabel);
                for (int i = 0; i < cntMetric; i++) rowData.Add(" ");

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (_level)
                {
                    case "APPLICATION":
                        Dictionary<string, ViolStatMetricIdDTO> prevStats = new Dictionary<string, ViolStatMetricIdDTO>();
                        foreach (string id in idList)
                        {
                            ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStat(reportData.PreviousSnapshot, int.Parse(id.Trim()));
                            prevStats[id] = stat;
                        }
                        string[] prevTotalRange = new string[cntMetric + 1];
                        string[] prevAddedRange = new string[cntMetric + 1];
                        string[] prevRemovedRange = new string[cntMetric + 1];
                        if (critical)
                        {
                            prevTotalRange[0] = Labels.ViolationsCritical;
                            if (delta)
                            {
                                prevAddedRange[0] = Labels.ViolationsAdded;
                                prevRemovedRange[0] = Labels.ViolationsRemoved;
                            }
                            int k = 1;
                            foreach (string id in prevStats.Keys)
                            {
                                prevTotalRange[k] = prevStats[id].TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    prevAddedRange[k] = prevStats[id].AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                    prevRemovedRange[k] = prevStats[id].RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                        }
                        else
                        {
                            prevTotalRange[0] = Labels.ViolationsCurrent;
                            if (delta)
                            {
                                prevAddedRange[0] = Labels.ViolationsAdded;
                                prevRemovedRange[0] = Labels.ViolationsRemoved;
                            }
                            int k = 1;
                            foreach (string id in prevStats.Keys)
                            {
                                prevTotalRange[k] = prevStats[id].TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    prevAddedRange[k] = prevStats[id].AddedViolations?.ToString("N0") ?? Constants.No_Value;
                                    prevRemovedRange[k] = prevStats[id].RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                        }
                        rowData.AddRange(prevTotalRange);
                        cntRow++;
                        if (delta)
                        {
                            rowData.AddRange(prevAddedRange);
                            cntRow++;
                            rowData.AddRange(prevRemovedRange);
                            cntRow++;
                        }
                        break;
                    case "MODULES":
                        List<Module> prevModules = reportData.PreviousSnapshot.Modules.Distinct()?.ToList();
                        // Dictionary<int, Dictionary<string, ViolStatMetricIdDTO>> prevStatsModules = new Dictionary<int, Dictionary<string, ViolStatMetricIdDTO>>();
                        foreach (Module module in prevModules)
                        {
                            Dictionary<string, ViolStatMetricIdDTO> prevStatsMod = new Dictionary<string, ViolStatMetricIdDTO>();
                            foreach (string id in idList)
                            {
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatModule(reportData.PreviousSnapshot, module.Id, int.Parse(id.Trim()));
                                prevStatsMod[id] = stat;
                            }
                            // prevStatsModules[module.Id] = prevStatsMod;

                            string[] totalModuleRange = new string[cntMetric + 1];
                            string[] addedModuleRange = new string[cntMetric + 1];
                            string[] removedModuleRange = new string[cntMetric + 1];

                            totalModuleRange[0] = module.Name;
                            int k = 1;
                            foreach (string id in prevStatsMod.Keys)
                            {
                                totalModuleRange[k] = critical
                                    ? prevStatsMod[id].TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                    : prevStatsMod[id].TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    addedModuleRange[0] = Labels.ViolationsAdded;
                                    addedModuleRange[k] = critical
                                        ? prevStatsMod[id].AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : prevStatsMod[id].AddedViolations?.ToString("N0") ?? Constants.No_Value;

                                    removedModuleRange[0] = Labels.ViolationsRemoved;
                                    removedModuleRange[k] = critical
                                        ? prevStatsMod[id].RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : prevStatsMod[id].RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                            rowData.AddRange(totalModuleRange);
                            if (delta)
                            {
                                rowData.AddRange(addedModuleRange);
                                cntRow++;
                                rowData.AddRange(removedModuleRange);
                                cntRow++;
                                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                            }
                            
                        }

                        break;
                    case "TECHNOLOGIES":
                        List<string> prevTechnos = reportData.PreviousSnapshot.Technologies?.ToList();
                        // Dictionary<int, Dictionary<string,ViolStatMetricIdDTO>> prevStatsTechnologies = new Dictionary<int, Dictionary<string, ViolStatMetricIdDTO>>(); 
                        foreach (string techno in prevTechnos)
                        {
                            Dictionary<string, ViolStatMetricIdDTO> prevStatsTech = new Dictionary<string, ViolStatMetricIdDTO>();
                            foreach (string id in idList)
                            {
                                ViolStatMetricIdDTO stat = RulesViolationUtility.GetViolStatTechno(reportData.PreviousSnapshot, techno, int.Parse(id.Trim()));
                                prevStatsTech[id] = stat;
                            }
                            // prevStatsTechnologies[module.Id] = prevStatsTech;

                            string[] totalTechnoRange = new string[cntMetric + 1];
                            string[] addedTechnoRange = new string[cntMetric + 1];
                            string[] removedTechnoRange = new string[cntMetric + 1];

                            totalTechnoRange[0] = techno;
                            int k = 1;
                            foreach (string id in prevStatsTech.Keys)
                            {
                                totalTechnoRange[k] = critical
                                    ? prevStatsTech[id].TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                    : prevStatsTech[id].TotalViolations?.ToString("N0") ?? Constants.No_Value;
                                if (delta)
                                {
                                    addedTechnoRange[0] = Labels.ViolationsAdded;
                                    addedTechnoRange[k] = critical
                                        ? prevStatsTech[id].AddedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : prevStatsTech[id].AddedViolations?.ToString("N0") ?? Constants.No_Value;

                                    removedTechnoRange[0] = Labels.ViolationsRemoved;
                                    removedTechnoRange[k] = critical
                                        ? prevStatsTech[id].RemovedCriticalViolations?.ToString("N0") ?? Constants.No_Value
                                        : prevStatsTech[id].RemovedViolations?.ToString("N0") ?? Constants.No_Value;
                                }
                                k++;
                            }
                            rowData.AddRange(totalTechnoRange);
                            if (delta)
                            {
                                rowData.AddRange(addedTechnoRange);
                                cntRow++;
                                rowData.AddRange(removedTechnoRange);
                                cntRow++;
                                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                            }
                            
                        }
                        break;
                }
            }


        #endregion


        var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = cntRow + 1,
                NbColumns = cntMetric + 1,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
