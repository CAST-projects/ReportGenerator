/*
 *   Copyright (c) 2019 CAST
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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("HEALTH_FACTOR")]
    public class HealthFactor : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS

            const string metricFormat = "N2";

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

			int param;

			bool showEvol = false;
			if (options != null && options.ContainsKey("SHOW_EVOL") && int.TryParse(options["SHOW_EVOL"], out param)) {
				showEvol = (param != 0);
			}

			bool showEvolPercent = true;
			if (options != null && options.ContainsKey("SHOW_EVOL_PERCENT") && int.TryParse(options["SHOW_EVOL_PERCENT"], out param)) {
				showEvolPercent = (param != 0);
			}

            if (reportData?.CurrentSnapshot?.BusinessCriteriaResults == null) return null;
            bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;

            #region currSnapshot
            string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
            BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot, false);
            #endregion  currSnapshot

            #region prevSnapshot
            string prevSnapshotLabel = hasPreviousSnapshot ? SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot) : Constants.No_Value;
            BusinessCriteriaDTO prevSnapshotBisCriDTO = hasPreviousSnapshot ? BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot, false) : null;
            #endregion  prevSnapshot

            List<string> rowData = new List<string>();
            rowData.AddRange(displayShortHeader
                ? new[] { " ", Labels.TQI, Labels.Robu, Labels.Efcy, Labels.Secu, Labels.Trans, Labels.Chang }
                : new[] { " ", Labels.TQI, Labels.Robustness, Labels.Efficiency, Labels.Security, Labels.Transferability, Labels.Changeability });
            rowData.AddRange(
                new[] {
                    currSnapshotLabel,
                    currSnapshotBisCriDTO.TQI?.ToString(metricFormat) ?? Constants.No_Value,
                    currSnapshotBisCriDTO.Robustness?.ToString(metricFormat) ?? Constants.No_Value,
                    currSnapshotBisCriDTO.Performance?.ToString(metricFormat) ?? Constants.No_Value,
                    currSnapshotBisCriDTO.Security?.ToString(metricFormat) ?? Constants.No_Value,
                    currSnapshotBisCriDTO.Transferability?.ToString(metricFormat) ?? Constants.No_Value,
                    currSnapshotBisCriDTO.Changeability?.ToString(metricFormat) ?? Constants.No_Value
                });

            if (hasPreviousSnapshot) {
                #region variation
                BusinessCriteriaDTO _businessCriteriaGradesEvol = (currSnapshotBisCriDTO - prevSnapshotBisCriDTO);
                BusinessCriteriaDTO _businessCriteriaGradesEvolPercent = (_businessCriteriaGradesEvol / prevSnapshotBisCriDTO);
                #endregion  variation

                rowData.AddRange(
                    new[] {
                        prevSnapshotLabel,
                        prevSnapshotBisCriDTO.TQI?.ToString(metricFormat) ?? Constants.No_Value,
                        prevSnapshotBisCriDTO.Robustness?.ToString(metricFormat) ?? Constants.No_Value,
                        prevSnapshotBisCriDTO.Performance?.ToString(metricFormat) ?? Constants.No_Value,
                        prevSnapshotBisCriDTO.Security?.ToString(metricFormat) ?? Constants.No_Value,
                        prevSnapshotBisCriDTO.Transferability?.ToString(metricFormat) ?? Constants.No_Value,
                        prevSnapshotBisCriDTO.Changeability?.ToString(metricFormat) ?? Constants.No_Value,
                    });
                
                if (showEvol) {
                    rowData.AddRange(
                        new[] {
                            Labels.Evol,
                            _businessCriteriaGradesEvol.TQI.HasValue ? FormatEvolution(_businessCriteriaGradesEvol.TQI.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvol.Robustness.HasValue ? FormatEvolution(_businessCriteriaGradesEvol.Robustness.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvol.Performance.HasValue ? FormatEvolution(_businessCriteriaGradesEvol.Performance.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvol.Security.HasValue ? FormatEvolution(_businessCriteriaGradesEvol.Security.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvol.Transferability.HasValue ? FormatEvolution(_businessCriteriaGradesEvol.Transferability.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvol.Changeability.HasValue ? FormatEvolution(_businessCriteriaGradesEvol.Changeability.Value) : Constants.No_Value
                        });
                }

                if (showEvolPercent) {
                    rowData.AddRange(
                        new[] {
                            Labels.EvolPercent,
                            _businessCriteriaGradesEvolPercent.TQI.HasValue ? FormatPercent(_businessCriteriaGradesEvolPercent.TQI.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvolPercent.Robustness.HasValue ? FormatPercent(_businessCriteriaGradesEvolPercent.Robustness.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvolPercent.Performance.HasValue ? FormatPercent(_businessCriteriaGradesEvolPercent.Performance.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvolPercent.Security.HasValue ? FormatPercent(_businessCriteriaGradesEvolPercent.Security.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvolPercent.Transferability.HasValue ? FormatPercent(_businessCriteriaGradesEvolPercent.Transferability.Value) : Constants.No_Value,
                            _businessCriteriaGradesEvolPercent.Changeability.HasValue ? FormatPercent(_businessCriteriaGradesEvolPercent.Changeability.Value) : Constants.No_Value
                        });
                }
            }

            var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = hasPreviousSnapshot ? 4 : 2,
                NbColumns = 7,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
