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
    [Block("TABLE_METRIC_ID_ROW")]
    internal class TableMetricIdRow : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS

            var rowData = new List<string>();
            int cntCol;

            const string gradeFormat = "N2";
            const string valueFormat = "N0";

            // by default, short names
            bool displayShortHeader = options.GetBoolOption("HEADER", true);

            string[] qidList = options.GetOption("QID")?.Split('|');
            string[] sidList = options.GetOption("SID")?.Split('|');
            string[] bidList = options.GetOption("BID")?.Split('|');

            string _snapshot = options.GetOption("SNAPSHOT", "BOTH"); // SNAPSHOT can be CURRENT, PREVIOUS or BOTH
            string _level = options.GetOption("LEVEL", "APPLICATION"); // LEVEL can be APPLICATION, MODULES or TECHNOLOGIES
            string _variation = options.GetOption("VARIATION", "PERCENT"); // VARIATION can be VALUE, PERCENT or BOTH


            if (reportData?.CurrentSnapshot?.BusinessCriteriaResults == null) return null;

            bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;

            List<Module> curModules = new List<Module>();
            if (_level == "MODULES") curModules = reportData.CurrentSnapshot.Modules.Distinct()?.ToList();

            List<string> curTechnologies = new List<string>();
            if (_level == "TECHNOLOGIES") curTechnologies = reportData.CurrentSnapshot.Technologies.Distinct()?.ToList();
            
            #region Get Names and background facts results

            int cntMetric = 0;
            Dictionary<string, string> names = new Dictionary<string, string>();
            Dictionary<string,Result> bfResults = new Dictionary<string, Result>();
            Dictionary<string, Result> bfPrevResults = new Dictionary<string, Result>();
            if (qidList != null)
            {
                foreach (string id in qidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    names[id] = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(id), displayShortHeader);
                    cntMetric++;
                }
            }
            if (sidList != null)
            {
                foreach (string id in sidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    var name = MeasureUtility.GetSizingMeasureName(reportData.CurrentSnapshot, int.Parse(id), displayShortHeader);
                    if (name != null) names[id] = name;
                    cntMetric++;
                }
            }

            // No background facts for technologies
            if (bidList != null && _level != "TECHNOLOGIES")
            {
                foreach (string id in bidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    Result bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.CurrentSnapshot.Href, id,true, true).FirstOrDefault();
                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                    string bfName = (displayShortHeader
                        ? bfResult.ApplicationResults[0].Reference.ShortName
                        : bfResult.ApplicationResults[0].Reference.Name)
                        ?? bfResult.ApplicationResults[0].Reference.Name;
                    names[id] = bfName;
                    cntMetric++;
                    if (bfResults.Keys.Contains(id)) continue;
                    bfResults[id] = bfResult;
                }
                if (hasPreviousSnapshot)
                {
                    foreach (string id in bidList)
                    {
                        if (bfPrevResults.Keys.Contains(id)) continue;
                        Result bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.PreviousSnapshot.Href, id, true, true).FirstOrDefault();
                        if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                        bfPrevResults[id] = bfResult;
                    }
                }
            }

            int cntRow = cntMetric + 1;

            #endregion

            #region headersCol

            string currSnapshotLabel = SnapshotUtility.GetSnapshotNameVersion(reportData.CurrentSnapshot);
            string prevSnapshotLabel = SnapshotUtility.GetSnapshotNameVersion(reportData.PreviousSnapshot);

            switch (_snapshot)
            {
                case "CURRENT":
                    rowData.AddRange(new[] { " ", currSnapshotLabel });
                    cntCol = 2;
                    break;
                case "PREVIOUS":
                    rowData.AddRange(hasPreviousSnapshot ? new[] {" ", prevSnapshotLabel} : new[] {" ", Constants.No_Value});
                    cntCol = 2;
                    break;
                default:
                    //case "BOTH":
                    if (hasPreviousSnapshot)
                    {
                        switch (_variation)
                        {
                            case "VALUE":
                                rowData.AddRange(new[] { " ", currSnapshotLabel, prevSnapshotLabel, Labels.Evol });
                                cntCol = 4;
                                break;
                            case "PERCENT":
                                rowData.AddRange(new[] { " ", currSnapshotLabel, prevSnapshotLabel, Labels.EvolPercent });
                                cntCol = 4;
                                break;
                            case "BOTH":
                                rowData.AddRange(new[] { " ", currSnapshotLabel, prevSnapshotLabel, Labels.Evol, Labels.EvolPercent });
                                cntCol = 5;
                                break;
                            default:
                                rowData.AddRange(new[] { " ", currSnapshotLabel, prevSnapshotLabel, Labels.EvolPercent });
                                cntCol = 4;
                                break;
                        }

                    }
                    else
                    {
                        rowData.AddRange(new[] { " ", currSnapshotLabel, Constants.No_Value });
                        cntCol = 3;
                    }
                    break;
            }

            #endregion

            #region Quality indicators

            if (qidList != null)
            {
                foreach (string id in qidList)
                {
                    rowData.Add(names[id]);

                    #region Application - QI
                    if (_level == "APPLICATION")
                    {
                        double? _currentValue;
                        double? _previousValue;

                        switch (_snapshot)
                        {
                            case "CURRENT":
                                _currentValue = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot,int.Parse(id));
                                rowData.Add(_currentValue?.ToString(gradeFormat) ?? Constants.No_Value);
                                break;
                            case "PREVIOUS":
                                if (hasPreviousSnapshot)
                                {
                                    _previousValue = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, int.Parse(id));
                                    rowData.Add(_previousValue?.ToString(gradeFormat) ?? Constants.No_Value);
                                }
                                else
                                {
                                    rowData.Add(Constants.No_Value);
                                }
                                break;
                            default:
                                //case "BOTH"
                                _currentValue = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot,int.Parse(id));
                                if (hasPreviousSnapshot)
                                {
                                    double? _evolution;
                                    double? _evolutionPercent;
                                    _previousValue = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, int.Parse(id));
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            _evolution = _currentValue - _previousValue;
                                            _evolutionPercent = _previousValue != 0 ? (_currentValue - _previousValue)/_previousValue : 1;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _evolution?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _evolutionPercent.HasValue
                                                    ? FormatPercent(_evolutionPercent)
                                                    : Constants.No_Value
                                            });
                                            break;
                                        case "VALUE":
                                            _evolution = _currentValue - _previousValue;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _evolution?.ToString(gradeFormat) ?? Constants.No_Value
                                            });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            _evolutionPercent = _previousValue != 0 ? (_currentValue - _previousValue)/_previousValue : 1;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                _evolutionPercent.HasValue ? FormatPercent(_evolutionPercent) : Constants.No_Value
                                            });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[]
                                    {
                                        _currentValue?.ToString(gradeFormat) ?? Constants.No_Value, Constants.No_Value
                                    });

                                }
                                break;
                        }
                    }

                    #endregion

                    #region Modules - QI
                    if (_level == "MODULES")
                    {
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new [] {" "});

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " "});
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] {" ", " "});
                                }
                                break;
                        }

                        foreach (Module mod in curModules)
                        {
                            double? curModValue;
                            double? prevModValue;

                            rowData.Add(mod.Name);
                            cntRow++;
                            
                            switch (_snapshot)
                            {
                                case "CURRENT":
                                    curModValue = BusinessCriteriaUtility.GetQualityIndicatorModuleGrade(reportData.CurrentSnapshot, mod.Id, int.Parse(id));    
                                    rowData.Add(curModValue?.ToString(gradeFormat) ?? Constants.No_Value);
                                    break;
                                case "PREVIOUS":
                                    if (hasPreviousSnapshot)
                                    {
                                        prevModValue = BusinessCriteriaUtility.GetQualityIndicatorModuleGrade(reportData.PreviousSnapshot, mod.Id, int.Parse(id));
                                        rowData.Add(prevModValue?.ToString(gradeFormat) ?? Constants.No_Value);
                                    }
                                    else
                                    {
                                        rowData.Add(Constants.No_Value);
                                    }
                                    break;
                                default:
                                    // case "BOTH"
                                    curModValue = BusinessCriteriaUtility.GetQualityIndicatorModuleGrade(reportData.CurrentSnapshot, mod.Id, int.Parse(id));
                                    if (hasPreviousSnapshot)
                                    {
                                        double? modEvolution;
                                        double? modEvolutionPercent;

                                        prevModValue = BusinessCriteriaUtility.GetQualityIndicatorModuleGrade(reportData.PreviousSnapshot, mod.Id, int.Parse(id));

                                        switch (_variation)
                                        {
                                            case "BOTH":
                                                modEvolution = curModValue - prevModValue;
                                                modEvolutionPercent = prevModValue != 0 ? (curModValue - prevModValue)/prevModValue : 1;
                                                rowData.AddRange(new []
                                                {
                                                    curModValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    modEvolution?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    modEvolutionPercent.HasValue ? FormatPercent(modEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                            case "VALUE":
                                                modEvolution = curModValue - prevModValue;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    modEvolution?.ToString(gradeFormat) ?? Constants.No_Value
                                                });
                                                break;
                                            default:
                                                //case "PERCENT"
                                                modEvolutionPercent = prevModValue != 0 ? (curModValue - prevModValue) / prevModValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    modEvolutionPercent.HasValue ? FormatPercent(modEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        rowData.AddRange(new[] { curModValue?.ToString(gradeFormat) ?? Constants.No_Value, Constants.No_Value });
                                    }
                                    break;
                            }

                        }
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " ", " " });
                                }
                                break;
                        }
                        cntRow++;
                    }

                    #endregion

                    #region Techno - QI
                    // ReSharper disable once InvertIf
                    if (_level == "TECHNOLOGIES")
                    {
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " " });
                                }
                                break;
                        }

                        foreach (string techno in curTechnologies)
                        {
                            double? curTechnoValue;
                            double? prevTechnoValue;

                            rowData.Add(techno);
                            cntRow++;

                            switch (_snapshot)
                            {
                                case "CURRENT":
                                    curTechnoValue = BusinessCriteriaUtility.GetQualityIndicatorTechnologyGrade(reportData.CurrentSnapshot, techno, int.Parse(id));
                                    rowData.Add(curTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value);
                                    break;
                                case "PREVIOUS":
                                    if (hasPreviousSnapshot)
                                    {
                                        prevTechnoValue = BusinessCriteriaUtility.GetQualityIndicatorTechnologyGrade(reportData.PreviousSnapshot, techno, int.Parse(id));
                                        rowData.Add(prevTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value);
                                    }
                                    else
                                    {
                                        rowData.Add(Constants.No_Value);
                                    }
                                    break;
                                default:
                                    // case "BOTH"
                                    curTechnoValue = BusinessCriteriaUtility.GetQualityIndicatorTechnologyGrade(reportData.CurrentSnapshot, techno, int.Parse(id));
                                    if (hasPreviousSnapshot)
                                    {
                                        double? techEvolution;
                                        double? techEvolutionPercent;

                                        prevTechnoValue = BusinessCriteriaUtility.GetQualityIndicatorTechnologyGrade(reportData.PreviousSnapshot, techno, int.Parse(id));

                                        switch (_variation)
                                        {
                                            case "BOTH":
                                                techEvolution = curTechnoValue - prevTechnoValue;
                                                techEvolutionPercent = prevTechnoValue != 0 ? (curTechnoValue - prevTechnoValue) / prevTechnoValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    prevTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    techEvolution?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    techEvolutionPercent.HasValue ? FormatPercent(techEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                            case "VALUE":
                                                techEvolution = curTechnoValue - prevTechnoValue;
                                                rowData.AddRange(new[]
                                                {
                                                    curTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    prevTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    techEvolution?.ToString(gradeFormat) ?? Constants.No_Value
                                                });
                                                break;
                                            default:
                                                //case "PERCENT"
                                                techEvolutionPercent = prevTechnoValue != 0 ? (curTechnoValue - prevTechnoValue) / prevTechnoValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    prevTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value,
                                                    techEvolutionPercent.HasValue ? FormatPercent(techEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        rowData.AddRange(new[] { curTechnoValue?.ToString(gradeFormat) ?? Constants.No_Value, Constants.No_Value });
                                    }
                                    break;
                            }

                        }
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " ", " " });
                                }
                                break;
                        }
                        cntRow++;

                    }
                    #endregion
                }
            }
            #endregion

            #region Sizing Measures

            if (sidList != null)
            {
                foreach (string id in sidList)
                {
                    if (!names.ContainsKey(id)) continue;
                    rowData.Add(names[id]);

                    #region Application - SZ

                    if (_level == "APPLICATION")
                    {
                        double? _currentValue;
                        double? _previousValue;

                        switch (_snapshot)
                        {
                            case "CURRENT":
                                _currentValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, int.Parse(id));
                                rowData.Add(_currentValue?.ToString(valueFormat) ?? Constants.No_Value);
                                break;
                            case "PREVIOUS":
                                if (hasPreviousSnapshot)
                                {
                                    _previousValue = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, int.Parse(id));
                                    rowData.Add(_previousValue?.ToString(valueFormat) ?? Constants.No_Value);
                                }
                                else
                                {
                                    rowData.Add(Constants.No_Value);
                                }
                                break;
                            default:
                                //case "BOTH"
                                _currentValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, int.Parse(id));
                                if (hasPreviousSnapshot)
                                {
                                    double? _evolution;
                                    double? _evolutionPercent;
                                    _previousValue = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, int.Parse(id));
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            _evolution = _currentValue - _previousValue;
                                            _evolutionPercent = _previousValue != 0 ? (_currentValue - _previousValue) / _previousValue : 1;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolution?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolutionPercent.HasValue ? FormatPercent(_evolutionPercent) : Constants.No_Value
                                            });
                                            break;
                                        case "VALUE":
                                            _evolution = _currentValue - _previousValue;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolution?.ToString(valueFormat) ?? Constants.No_Value
                                            });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            _evolutionPercent = _previousValue != 0 ? (_currentValue - _previousValue) / _previousValue : 1;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolutionPercent.HasValue ? FormatPercent(_evolutionPercent) : Constants.No_Value
                                            });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[]
                                    {
                                        _currentValue?.ToString(valueFormat) ?? Constants.No_Value,  Constants.No_Value
                                    });

                                }
                                break;
                        }
                    }

                    #endregion

                    #region Modules - SZ
                    if (_level == "MODULES")
                    {
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " " });
                                }
                                break;
                        }

                        foreach (Module mod in curModules)
                        {
                            double? curModValue;
                            double? prevModValue;

                            rowData.Add(mod.Name);
                            cntRow++;

                            switch (_snapshot)
                            {
                                case "CURRENT":
                                    curModValue = MeasureUtility.GetSizingMeasureModule(reportData.CurrentSnapshot, mod.Id, int.Parse(id));
                                    rowData.Add(curModValue?.ToString(valueFormat) ?? Constants.No_Value);
                                    break;
                                case "PREVIOUS":
                                    if (hasPreviousSnapshot)
                                    {
                                        prevModValue = MeasureUtility.GetSizingMeasureModule(reportData.PreviousSnapshot, mod.Id, int.Parse(id));
                                        rowData.Add(prevModValue?.ToString(valueFormat) ?? Constants.No_Value);
                                    }
                                    else
                                    {
                                        rowData.Add(Constants.No_Value);
                                    }
                                    break;
                                default:
                                    // case "BOTH"
                                    curModValue = MeasureUtility.GetSizingMeasureModule(reportData.CurrentSnapshot, mod.Id, int.Parse(id));
                                    if (hasPreviousSnapshot)
                                    {
                                        double? modEvolution;
                                        double? modEvolutionPercent;

                                        prevModValue = MeasureUtility.GetSizingMeasureModule(reportData.PreviousSnapshot, mod.Id, int.Parse(id));

                                        switch (_variation)
                                        {
                                            case "BOTH":
                                                modEvolution = curModValue - prevModValue;
                                                modEvolutionPercent = prevModValue != 0 ? (curModValue - prevModValue) / prevModValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(valueFormat) ??  Constants.No_Value,
                                                    prevModValue?.ToString(valueFormat) ??  Constants.No_Value,
                                                    modEvolution?.ToString(valueFormat) ??  Constants.No_Value,
                                                    modEvolutionPercent.HasValue ? FormatPercent(modEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                            case "VALUE":
                                                modEvolution = curModValue - prevModValue;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    modEvolution?.ToString(valueFormat) ?? Constants.No_Value
                                                });
                                                break;
                                            default:
                                                //case "PERCENT"
                                                modEvolutionPercent = prevModValue != 0 ? (curModValue - prevModValue) / prevModValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    modEvolutionPercent.HasValue ? FormatPercent(modEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        rowData.AddRange(new[] { curModValue?.ToString(valueFormat) ?? Constants.No_Value, Constants.No_Value });
                                    }
                                    break;
                            }

                        }
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " ", " " });
                                }
                                break;
                        }
                        cntRow++;
                    }
                    #endregion

                    #region Technologies - SZ
                    // ReSharper disable once InvertIf
                    if (_level == "TECHNOLOGIES")
                    {
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " " });
                                }
                                break;
                        }

                        foreach (string techno in curTechnologies)
                        {
                            double? curTechnoValue;
                            double? prevTechnoValue;

                            rowData.Add(techno);
                            cntRow++;

                            switch (_snapshot)
                            {
                                case "CURRENT":
                                    curTechnoValue = MeasureUtility.GetSizingMeasureTechnologie(reportData.CurrentSnapshot, techno, int.Parse(id));
                                    rowData.Add(curTechnoValue?.ToString(valueFormat) ?? Constants.No_Value);
                                    break;
                                case "PREVIOUS":
                                    if (hasPreviousSnapshot)
                                    {
                                        prevTechnoValue = MeasureUtility.GetSizingMeasureTechnologie(reportData.PreviousSnapshot, techno, int.Parse(id));
                                        rowData.Add(prevTechnoValue?.ToString(valueFormat) ?? Constants.No_Value);
                                    }
                                    else
                                    {
                                        rowData.Add(Constants.No_Value);
                                    }
                                    break;
                                default:
                                    // case "BOTH"
                                    curTechnoValue = MeasureUtility.GetSizingMeasureTechnologie(reportData.CurrentSnapshot, techno, int.Parse(id));
                                    if (hasPreviousSnapshot)
                                    {
                                        double? techEvolution;
                                        double? techEvolutionPercent;

                                        prevTechnoValue = MeasureUtility.GetSizingMeasureTechnologie(reportData.PreviousSnapshot, techno, int.Parse(id));

                                        switch (_variation)
                                        {
                                            case "BOTH":
                                                techEvolution = curTechnoValue - prevTechnoValue;
                                                techEvolutionPercent = prevTechnoValue != 0 ? (curTechnoValue - prevTechnoValue) / prevTechnoValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curTechnoValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevTechnoValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    techEvolution?.ToString(valueFormat) ?? Constants.No_Value,
                                                    techEvolutionPercent.HasValue ? FormatPercent(techEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                            case "VALUE":
                                                techEvolution = curTechnoValue - prevTechnoValue;
                                                rowData.AddRange(new[]
                                                {
                                                    curTechnoValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevTechnoValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    techEvolution?.ToString(valueFormat) ?? Constants.No_Value
                                                });
                                                break;
                                            default:
                                                //case "PERCENT"
                                                techEvolutionPercent = prevTechnoValue != 0 ? (curTechnoValue - prevTechnoValue) / prevTechnoValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curTechnoValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevTechnoValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    techEvolutionPercent.HasValue ? FormatPercent(techEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        rowData.AddRange(new[] { curTechnoValue?.ToString(valueFormat) ?? Constants.No_Value, Constants.No_Value });
                                    }
                                    break;
                            }

                        }
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " ", " " });
                                }
                                break;
                        }
                        cntRow++;

                    }
                    #endregion
                }
            }

            #endregion

            #region Background facts

            if (bidList != null)
            {
                foreach (string id in bidList)
                {
                    if (!names.ContainsKey(id)) continue;
                    rowData.Add(names[id]);

                    #region Application - BF

                    if (_level == "APPLICATION")
                    {
                        double? _currentValue;
                        double? _previousValue;
                        Result bfResult;
                        Result bfPrevResult;

                        switch (_snapshot)
                        {
                            case "CURRENT":
                                bfResult = bfResults.Keys.Contains(id) ? bfResults[id] : null;
                                if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                _currentValue = bfResult.ApplicationResults[0].DetailResult.Value;
                                rowData.Add(_currentValue?.ToString(valueFormat) ?? Constants.No_Value);
                                break;
                            case "PREVIOUS":
                                if (hasPreviousSnapshot)
                                {
                                    bfPrevResult = bfPrevResults.Keys.Contains(id) ? bfResults[id] : null;
                                    if (bfPrevResult == null || !bfPrevResult.ApplicationResults.Any()) continue;
                                    _previousValue = bfPrevResult.ApplicationResults[0].DetailResult.Value;
                                    rowData.Add(_previousValue?.ToString(valueFormat) ?? Constants.No_Value);
                                }
                                else
                                {
                                    rowData.Add(Constants.No_Value);
                                }
                                break;
                            default:
                                //case "BOTH"
                                bfResult = bfResults.Keys.Contains(id) ? bfResults[id] : null;
                                if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                _currentValue = bfResult.ApplicationResults[0].DetailResult.Value;
                                if (hasPreviousSnapshot)
                                {
                                    double? _evolution;
                                    double? _evolutionPercent;
                                    bfPrevResult = bfPrevResults.Keys.Contains(id) ? bfResults[id] : null;
                                    if (bfPrevResult == null || !bfPrevResult.ApplicationResults.Any()) continue;
                                    _previousValue = bfPrevResult.ApplicationResults[0].DetailResult.Value;
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            _evolution = _currentValue - _previousValue;
                                            _evolutionPercent = _previousValue != 0 ? (_currentValue - _previousValue) / _previousValue : 1;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolution?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolutionPercent.HasValue ? FormatPercent(_evolutionPercent) : Constants.No_Value
                                            });
                                            break;
                                        case "VALUE":
                                            _evolution = _currentValue - _previousValue;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolution?.ToString(valueFormat) ?? Constants.No_Value
                                            });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            _evolutionPercent = _previousValue != 0 ? (_currentValue - _previousValue) / _previousValue : 1;
                                            rowData.AddRange(new[]
                                            {
                                                _currentValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _previousValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                _evolutionPercent.HasValue ? FormatPercent(_evolutionPercent) : Constants.No_Value
                                            });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[]
                                    {
                                        _currentValue?.ToString(valueFormat) ?? Constants.No_Value, Constants.No_Value
                                    });

                                }
                                break;
                        }
                    }

                    #endregion

                    #region Modules - BF
                    // ReSharper disable once InvertIf
                    if (_level == "MODULES")
                    {
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " " });
                                }
                                break;
                        }

                        foreach (Module mod in curModules)
                        {
                            double? curModValue;
                            double? prevModValue;
                            Result bfResult;
                            Result bfPrevResult;

                            rowData.Add(mod.Name);
                            cntRow++;

                            switch (_snapshot)
                            {
                                case "CURRENT":
                                    bfResult = bfResults.Keys.Contains(id) ? bfResults[id] : null;
                                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                    curModValue = bfResult.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == mod.Id)?.DetailResult.Value;
                                    rowData.Add(curModValue?.ToString(valueFormat) ?? Constants.No_Value);
                                    break;
                                case "PREVIOUS":
                                    if (hasPreviousSnapshot)
                                    {
                                        bfPrevResult = bfPrevResults.Keys.Contains(id) ? bfPrevResults[id] : null;
                                        if (bfPrevResult == null || !bfPrevResult.ApplicationResults.Any()) continue;
                                        prevModValue = bfPrevResult.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == mod.Id)?.DetailResult.Value;
                                        rowData.Add(prevModValue?.ToString(valueFormat) ?? Constants.No_Value);
                                    }
                                    else
                                    {
                                        rowData.Add(Constants.No_Value);
                                    }
                                    break;
                                default:
                                    // case "BOTH"
                                    bfResult = bfResults.Keys.Contains(id) ? bfResults[id] : null;
                                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                    curModValue = bfResult.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == mod.Id)?.DetailResult.Value;
                                    if (hasPreviousSnapshot)
                                    {
                                        double? modEvolution;
                                        double? modEvolutionPercent;

                                        bfPrevResult = bfPrevResults.Keys.Contains(id) ? bfPrevResults[id] : null;
                                        if (bfPrevResult == null || !bfPrevResult.ApplicationResults.Any()) continue;
                                        prevModValue = bfPrevResult.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == mod.Id)?.DetailResult.Value;

                                        switch (_variation)
                                        {
                                            case "BOTH":
                                                modEvolution = curModValue - prevModValue;
                                                modEvolutionPercent = prevModValue != 0 ? (curModValue - prevModValue) / prevModValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    modEvolution?.ToString(valueFormat) ?? Constants.No_Value,
                                                    modEvolutionPercent.HasValue ? FormatPercent(modEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                            case "VALUE":
                                                modEvolution = curModValue - prevModValue;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    modEvolution?.ToString(valueFormat) ?? Constants.No_Value
                                                });
                                                break;
                                            default:
                                                //case "PERCENT"
                                                modEvolutionPercent = prevModValue != 0 ? (curModValue - prevModValue) / prevModValue : 1;
                                                rowData.AddRange(new[]
                                                {
                                                    curModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    prevModValue?.ToString(valueFormat) ?? Constants.No_Value,
                                                    modEvolutionPercent.HasValue ? FormatPercent(modEvolutionPercent) : Constants.No_Value
                                                });
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        rowData.AddRange(new[] { curModValue?.ToString(valueFormat) ?? Constants.No_Value, Constants.No_Value });
                                    }
                                    break;
                            }

                        }
                        switch (_snapshot)
                        {
                            case "CURRENT":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            case "PREVIOUS":
                                rowData.AddRange(new[] { " ", " " });

                                break;
                            default:
                                // case "BOTH"
                                if (hasPreviousSnapshot)
                                {
                                    switch (_variation)
                                    {
                                        case "BOTH":
                                            rowData.AddRange(new[] { " ", " ", " ", " ", " " });
                                            break;
                                        case "VALUE":
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                        default:
                                            //case "PERCENT"
                                            rowData.AddRange(new[] { " ", " ", " ", " " });
                                            break;
                                    }
                                }
                                else
                                {
                                    rowData.AddRange(new[] { " ", " ", " " });
                                }
                                break;
                        }
                        cntRow++;
                    }
                    #endregion
                }
            }

            #endregion

            var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = cntRow,
                NbColumns = cntCol,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
