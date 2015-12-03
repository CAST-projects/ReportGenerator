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

    


    [Block("CAST_HIGH_DISTRIBUTION")]
    public class CastHighDistribution : TableBlock
    {
        private const string _MetricFormat = "N0";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition back = new TableDefinition();
            int parId = -1;
            Constants.QualityDistribution distributionId;
            List<string> rowData = new List<string>();
            
            double? previousHigVal = null;
            double? previousVhiVal = null;
            double? previousHttVal = null;

            if (null != options && options.ContainsKey("PAR") && Int32.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;

            if (null != reportData)
            {

                #region Selected Snapshot

                double? selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                Constants.CyclomaticComplexity.ComplexityArtifacts_Low.GetHashCode());
                double? selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                Constants.CyclomaticComplexity.ComplexityArtifacts_Moderate.GetHashCode());
                double? selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                Constants.CyclomaticComplexity.ComplexityArtifacts_High.GetHashCode());
                double? selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                Constants.CyclomaticComplexity.ComplexityArtifacts_VeryHigh.GetHashCode());



                double? selectedTotal = (selectedLowVal.HasValue && selectedAveVal.HasValue && selectedHigVal.HasValue && selectedVhiVal.HasValue) ? selectedLowVal.Value + selectedAveVal.Value + selectedHigVal.Value + selectedVhiVal.Value : (double?)null;
                double? selectedHttVal = (selectedHigVal.HasValue && selectedVhiVal.HasValue) ? selectedHigVal.Value + selectedVhiVal.Value : (double?)null;
 
                #endregion Selected Snapshot
            
                #region Previous Snapshot

                if (reportData.PreviousSnapshot != null)
                {

                    previousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                  distributionId.GetHashCode(),
                                                                                  Constants.CyclomaticComplexity.ComplexityArtifacts_High.GetHashCode());
                    previousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                  distributionId.GetHashCode(),
                                                                                  Constants.CyclomaticComplexity.ComplexityArtifacts_VeryHigh.GetHashCode());

                    previousHttVal = previousHigVal.HasValue && previousVhiVal.HasValue ? previousHigVal.Value + previousVhiVal.Value : (double?)null;

                }
             
               #endregion Previous Snapshot

               #region Data
               Int32? variation = (selectedHttVal.HasValue && previousHttVal.HasValue) ? (Int32)(selectedHttVal - previousHttVal) : (Int32?)null;
                 
               string distributionName = CastComplexityUtility.GetCostComplexityName(reportData.CurrentSnapshot, distributionId.GetHashCode());

               rowData.AddRange(new string[] { distributionName, "Current total", "Previous total", "Evol.", "% on total elements" });
               rowData.AddRange(new string[]
                        { "High and Very High Complexity"
                        , selectedHttVal.HasValue? selectedHttVal.Value.ToString(_MetricFormat) : Constants.No_Value
                        , previousHttVal.HasValue ? previousHttVal.Value.ToString(_MetricFormat) : Constants.No_Value
                        , variation.HasValue? TableBlock.FormatEvolution((Int32)variation.Value): Constants.No_Value
                        , (selectedHttVal.HasValue && previousHttVal.HasValue && previousHttVal.Value>0)? TableBlock.FormatPercent(selectedHttVal.Value / selectedTotal.Value, false): Constants.No_Value
                        });

              
                #endregion Data

                back = new TableDefinition
                {
                    Data = rowData,
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbColumns = 5,
                    NbRows = 2
                };
            }
        
            return back;
        }


       
    }
}
