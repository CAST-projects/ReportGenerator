
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
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System.Globalization;

namespace CastReporting.Reporting.Block.Graph
    {
        [Block("BUBBLE")]
        class Bubble : GraphBlock
        {
            #region METHODS
            protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
            {
             

                #region Required Options
                string moduleIdstr = (options != null && options.ContainsKey("M") ? options["M"] : string.Empty);
                int moduleId;
                if (string.IsNullOrWhiteSpace(moduleIdstr) || !Int32.TryParse(moduleIdstr, out moduleId))
                {
                    moduleId = -1;
                }
               
                #endregion Required Options

                List<String> rowData = new List<String>();
                rowData.AddRange(new string[] { Labels.TechnicalDebt + " (" + reportData.CurrencySymbol + ")", Labels.TQI, Labels.Size });

                if (reportData != null &&  reportData.CurrentSnapshot != null)
                {

                    double? TQIValue = 0;
                    double? TechDebtValue = 0;
                    double? COLValue = 0;

                    if (moduleId > 0)
                    {

                        TQIValue = BusinessCriteriaUtility.GetBusinessCriteriaModuleGrade(reportData.CurrentSnapshot, moduleId, Constants.BusinessCriteria.TechnicalQualityIndex, true);
                        TechDebtValue = MeasureUtility.GetModuleMeasureGrade(reportData.CurrentSnapshot, moduleId, Constants.SizingInformations.TechnicalDebt);
                        COLValue = MeasureUtility.GetModuleMeasureGrade(reportData.CurrentSnapshot, moduleId, Constants.SizingInformations.CodeLineNumber);
                    }
                    else
                    {
                        TQIValue = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(reportData.CurrentSnapshot, Constants.BusinessCriteria.TechnicalQualityIndex, true);
                        TechDebtValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.TechnicalDebt);
                        COLValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.CodeLineNumber);
                    }

                    rowData.Add(TQIValue.GetValueOrDefault().ToString());
                    rowData.Add(TechDebtValue.GetValueOrDefault().ToString());
                    rowData.Add(COLValue.GetValueOrDefault().ToString());
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



