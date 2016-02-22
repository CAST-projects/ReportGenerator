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


namespace CastReporting.Reporting.Block.Table
{
    [Block("HEALTH_FACTOR")]
    class HealthFactor : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS
            TableDefinition resultTable = null;
         
           string metricFormat = "N2";

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

			int param = 0;

			bool showEvol = false;
			if (options != null && options.ContainsKey("SHOW_EVOL") && int.TryParse(options["SHOW_EVOL"], out param)) {
				showEvol = (param != 0);
			}

			bool showEvolPercent = true;
			if (options != null && options.ContainsKey("SHOW_EVOL_PERCENT") && int.TryParse(options["SHOW_EVOL_PERCENT"], out param)) {
				showEvolPercent = (param != 0);
			}

            if (null != reportData &&
                null != reportData.CurrentSnapshot &&
			             null != reportData.CurrentSnapshot.BusinessCriteriaResults) {
                bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;

                #region currSnapshot
                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot, false);
                #endregion  currSnapshot

                #region prevSnapshot
                string prevSnapshotLabel = hasPreviousSnapshot ? SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot) : CastReporting.Domain.Constants.No_Value;
                BusinessCriteriaDTO prevSnapshotBisCriDTO = hasPreviousSnapshot ? BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot, false) : null;
                 #endregion  prevSnapshot

                List<string> rowData = new List<string>();
                rowData.AddRange(displayShortHeader
                                    ? new[] { " ", Labels.TQI, Labels.Robu, Labels.Efcy, Labels.Secu, Labels.Trans, Labels.Chang }
                                    : new[] { " ", Labels.TQI, Labels.Robustness, Labels.Efficiency, Labels.Security, Labels.Transferability, Labels.Changeability });
                rowData.AddRange(
					new[] {
                        currSnapshotLabel,
                        currSnapshotBisCriDTO.TQI.HasValue ? currSnapshotBisCriDTO.TQI.Value.ToString(metricFormat) : Constants.No_Value,
                        currSnapshotBisCriDTO.Robustness.HasValue ? currSnapshotBisCriDTO.Robustness.Value.ToString(metricFormat) : Constants.No_Value,
                        currSnapshotBisCriDTO.Performance.HasValue ? currSnapshotBisCriDTO.Performance.Value.ToString(metricFormat) : Constants.No_Value,
                        currSnapshotBisCriDTO.Security.HasValue ? currSnapshotBisCriDTO.Security.Value.ToString(metricFormat) : Constants.No_Value,
                        currSnapshotBisCriDTO.Transferability.HasValue ? currSnapshotBisCriDTO.Transferability.Value.ToString(metricFormat) : Constants.No_Value,
                        currSnapshotBisCriDTO.Changeability.HasValue ? currSnapshotBisCriDTO.Changeability.Value.ToString(metricFormat) : Constants.No_Value
                    });

                if (hasPreviousSnapshot) {
	                #region variation
					BusinessCriteriaDTO BusinessCriteriaGradesEvol = hasPreviousSnapshot ? (currSnapshotBisCriDTO - prevSnapshotBisCriDTO) : null;
	                BusinessCriteriaDTO BusinessCriteriaGradesEvolPercent = hasPreviousSnapshot ? (BusinessCriteriaGradesEvol / prevSnapshotBisCriDTO) : null;
                    #endregion  variation

                    rowData.AddRange(
                        new[] {
                            prevSnapshotLabel,
                            prevSnapshotBisCriDTO.TQI.HasValue?prevSnapshotBisCriDTO.TQI.Value.ToString(metricFormat):Constants.No_Value,
                            prevSnapshotBisCriDTO.Robustness.HasValue?prevSnapshotBisCriDTO.Robustness.Value.ToString(metricFormat):Constants.No_Value,
                            prevSnapshotBisCriDTO.Performance.HasValue?prevSnapshotBisCriDTO.Performance.Value.ToString(metricFormat):Constants.No_Value,
                            prevSnapshotBisCriDTO.Security.HasValue?prevSnapshotBisCriDTO.Security.Value.ToString(metricFormat):Constants.No_Value,
                            prevSnapshotBisCriDTO.Transferability.HasValue?prevSnapshotBisCriDTO.Transferability.Value.ToString(metricFormat):Constants.No_Value,
                            prevSnapshotBisCriDTO.Changeability.HasValue?prevSnapshotBisCriDTO.Changeability.Value.ToString(metricFormat):Constants.No_Value,
                        });
                
					if (showEvol) {
                        rowData.AddRange(
                            new[] {
                                Labels.Evol,
                                BusinessCriteriaGradesEvol.TQI.HasValue ? FormatEvolution(BusinessCriteriaGradesEvol.TQI.Value) : Constants.No_Value,
                                BusinessCriteriaGradesEvol.Robustness.HasValue ? FormatEvolution(BusinessCriteriaGradesEvol.Robustness.Value) : Constants.No_Value,
                                BusinessCriteriaGradesEvol.Performance.HasValue ? FormatEvolution(BusinessCriteriaGradesEvol.Performance.Value) : Constants.No_Value,
                                BusinessCriteriaGradesEvol.Security.HasValue ? FormatEvolution(BusinessCriteriaGradesEvol.Security.Value) : Constants.No_Value,
                                BusinessCriteriaGradesEvol.Transferability.HasValue ? FormatEvolution(BusinessCriteriaGradesEvol.Transferability.Value) : Constants.No_Value,
                                BusinessCriteriaGradesEvol.Changeability.HasValue ? FormatEvolution(BusinessCriteriaGradesEvol.Changeability.Value) : Constants.No_Value
                            });
					}

					if (showEvolPercent) {
	                    rowData.AddRange(
							new[] {
								Labels.EvolPercent,
		                        BusinessCriteriaGradesEvolPercent.TQI.HasValue ? FormatPercent(BusinessCriteriaGradesEvolPercent.TQI.Value) : Constants.No_Value,
		                        BusinessCriteriaGradesEvolPercent.Robustness.HasValue ? FormatPercent(BusinessCriteriaGradesEvolPercent.Robustness.Value) : Constants.No_Value,
		                        BusinessCriteriaGradesEvolPercent.Performance.HasValue ? FormatPercent(BusinessCriteriaGradesEvolPercent.Performance.Value) : Constants.No_Value,
		                        BusinessCriteriaGradesEvolPercent.Security.HasValue ? FormatPercent(BusinessCriteriaGradesEvolPercent.Security.Value) : Constants.No_Value,
		                        BusinessCriteriaGradesEvolPercent.Transferability.HasValue ? FormatPercent(BusinessCriteriaGradesEvolPercent.Transferability.Value) : Constants.No_Value,
		                        BusinessCriteriaGradesEvolPercent.Changeability.HasValue ? FormatPercent(BusinessCriteriaGradesEvolPercent.Changeability.Value) : Constants.No_Value
	                    	});
					}
                }

                resultTable = new TableDefinition {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = hasPreviousSnapshot ? 4 : 2,
                    NbColumns = 7,
                    Data = rowData
                };
            }
            return resultTable;
        }
        #endregion METHODS
    }
}
