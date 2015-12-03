/*
 *   Copyright (c) 2015 CAST
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

            if (null != reportData &&
                null != reportData.CurrentSnapshot &&
                null != reportData.CurrentSnapshot.BusinessCriteriaResults)
            {
                bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;


                #region currSnapshot
                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot);
                #endregion  currSnapshot

                #region prevSnapshot
                string prevSnapshotLabel = hasPreviousSnapshot ? SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot) : CastReporting.Domain.Constants.No_Value;
                BusinessCriteriaDTO prevSnapshotBisCriDTO = hasPreviousSnapshot ?BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot) : null;
                 #endregion  prevSnapshot


                #region variation
                BusinessCriteriaDTO BusinessCriteriaGradesVartiation = hasPreviousSnapshot ? ((currSnapshotBisCriDTO - prevSnapshotBisCriDTO) / prevSnapshotBisCriDTO) : null ;
                #endregion  variation
             
                
                
               
                List<string> rowData = new List<string>();
                rowData.AddRange(displayShortHeader
                                    ? new[] { "", "TQI", "Robu.", "Efcy.", "Secu.", "Trans.", "Chang." }
                                    : new[] { "", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });
                rowData.AddRange(
                    new[]
                    {
                        currSnapshotLabel,
                        currSnapshotBisCriDTO.TQI.HasValue?currSnapshotBisCriDTO.TQI.Value.ToString(metricFormat):Constants.No_Value,
                        currSnapshotBisCriDTO.Robustness.HasValue?currSnapshotBisCriDTO.Robustness.Value.ToString(metricFormat):Constants.No_Value,
                        currSnapshotBisCriDTO.Performance.HasValue?currSnapshotBisCriDTO.Performance.Value.ToString(metricFormat):Constants.No_Value,
                        currSnapshotBisCriDTO.Security.HasValue?currSnapshotBisCriDTO.Security.Value.ToString(metricFormat):Constants.No_Value,
                        currSnapshotBisCriDTO.Transferability.HasValue?currSnapshotBisCriDTO.Transferability.Value.ToString(metricFormat):Constants.No_Value,
                        currSnapshotBisCriDTO.Changeability.HasValue?currSnapshotBisCriDTO.Changeability.Value.ToString(metricFormat):Constants.No_Value
                    });
                if (hasPreviousSnapshot)
                {
                    rowData.AddRange(
                        new[]
                    {
                        prevSnapshotLabel,
                        prevSnapshotBisCriDTO.TQI.HasValue?prevSnapshotBisCriDTO.TQI.Value.ToString(metricFormat):Constants.No_Value,
                        prevSnapshotBisCriDTO.Robustness.HasValue?prevSnapshotBisCriDTO.Robustness.Value.ToString(metricFormat):Constants.No_Value,
                        prevSnapshotBisCriDTO.Performance.HasValue?prevSnapshotBisCriDTO.Performance.Value.ToString(metricFormat):Constants.No_Value,
                        prevSnapshotBisCriDTO.Security.HasValue?prevSnapshotBisCriDTO.Security.Value.ToString(metricFormat):Constants.No_Value,
                        prevSnapshotBisCriDTO.Transferability.HasValue?prevSnapshotBisCriDTO.Transferability.Value.ToString(metricFormat):Constants.No_Value,
                        prevSnapshotBisCriDTO.Changeability.HasValue?prevSnapshotBisCriDTO.Changeability.Value.ToString(metricFormat):Constants.No_Value,
                        
                        "Variation",
                        BusinessCriteriaGradesVartiation.TQI.HasValue?TableBlock.FormatPercent(BusinessCriteriaGradesVartiation.TQI.Value):Constants.No_Value,
                        BusinessCriteriaGradesVartiation.Robustness.HasValue?TableBlock.FormatPercent(BusinessCriteriaGradesVartiation.Robustness.Value):Constants.No_Value,
                        BusinessCriteriaGradesVartiation.Performance.HasValue?TableBlock.FormatPercent(BusinessCriteriaGradesVartiation.Performance.Value):Constants.No_Value,
                        BusinessCriteriaGradesVartiation.Security.HasValue? TableBlock.FormatPercent(BusinessCriteriaGradesVartiation.Security.Value):Constants.No_Value,
                        BusinessCriteriaGradesVartiation.Transferability.HasValue? TableBlock.FormatPercent(BusinessCriteriaGradesVartiation.Transferability.Value):Constants.No_Value,
                        BusinessCriteriaGradesVartiation.Changeability.HasValue? TableBlock.FormatPercent(BusinessCriteriaGradesVartiation.Changeability.Value):Constants.No_Value
                    });
                }
                resultTable = new TableDefinition
                {
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
