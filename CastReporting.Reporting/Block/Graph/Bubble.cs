
/*
 *   Copyright (c) 2016 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System.Collections.Generic;
using System.Globalization;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Graph
    {
        [Block("BUBBLE")]
        internal class Bubble : GraphBlock
        {
            #region METHODS
            protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
            {
             

                #region Required Options
                string moduleIdstr = (options != null && options.ContainsKey("M") ? options["M"] : string.Empty);
                int moduleId;
                if (string.IsNullOrWhiteSpace(moduleIdstr) || !int.TryParse(moduleIdstr, out moduleId))
                {
                    moduleId = -1;
                }
               
                #endregion Required Options

                List<string> rowData = new List<string>();
                rowData.AddRange(new[] { Labels.TechnicalDebt + " (" + reportData.CurrencySymbol + ")", Labels.TQI, Labels.Size });

                if (reportData.CurrentSnapshot != null)
                {

                    double? _tqiValue;
                    double? _techDebtValue;
                    double? _colValue;

                    if (moduleId > 0)
                    {

                        _tqiValue = BusinessCriteriaUtility.GetBusinessCriteriaModuleGrade(reportData.CurrentSnapshot, moduleId, Constants.BusinessCriteria.TechnicalQualityIndex, true);
                        _techDebtValue = MeasureUtility.GetModuleMeasureGrade(reportData.CurrentSnapshot, moduleId, Constants.SizingInformations.TechnicalDebt);
                        _colValue = MeasureUtility.GetModuleMeasureGrade(reportData.CurrentSnapshot, moduleId, Constants.SizingInformations.CodeLineNumber);
                    }
                    else
                    {
                        _tqiValue = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(reportData.CurrentSnapshot, Constants.BusinessCriteria.TechnicalQualityIndex, true);
                        _techDebtValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.TechnicalDebt);
                        _colValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.CodeLineNumber);
                    }

                    rowData.Add(_tqiValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                    rowData.Add(_techDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                    rowData.Add(_colValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                }

                TableDefinition resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 3,
                    Data = rowData
                };
                return resultTable;
            }
            #endregion METHODS
        }
    }



