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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Languages;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CAST_DISTRIBUTION")]
    public class CastDistribution : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition result = new TableDefinition();
            List<string> rowData = new List<string>();
            int parId;
            Constants.QualityDistribution distributionId;


            double? selectedLowVal = null;
            double? selectedAveVal = null;
            double? selectedHigVal = null;
            double? selectedVhiVal = null;
            double? selectedTotal = null;
            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;
            string distributionName = Constants.No_Value;

            if (null != options && options.ContainsKey("PAR") && Int32.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;


            if (null != reportData && null != reportData.CurrentSnapshot)            
            {              

                #region Selected Snapshot

                selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                Constants.CyclomaticComplexity.ComplexityArtifacts_Low.GetHashCode());
                selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                               distributionId.GetHashCode(),
                                                                               Constants.CyclomaticComplexity.ComplexityArtifacts_Moderate.GetHashCode());
                selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                Constants.CyclomaticComplexity.ComplexityArtifacts_High.GetHashCode());
                selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                distributionId.GetHashCode(),
                                                                                 Constants.CyclomaticComplexity.ComplexityArtifacts_VeryHigh.GetHashCode());


                if (selectedLowVal.HasValue && selectedAveVal.HasValue && selectedHigVal.HasValue && selectedVhiVal.HasValue)
                    selectedTotal = selectedLowVal + selectedAveVal + selectedHigVal + selectedVhiVal;

                #endregion Selected Snapshot

                #region Previous Snapshot


                if (reportData.PreviousSnapshot != null)
                {                    

                    previousLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                               distributionId.GetHashCode(),
                                                                               Constants.CyclomaticComplexity.ComplexityArtifacts_Low.GetHashCode());
                    previousAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                   distributionId.GetHashCode(),
                                                                                   Constants.CyclomaticComplexity.ComplexityArtifacts_Moderate.GetHashCode());
                    previousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                    distributionId.GetHashCode(),
                                                                                    Constants.CyclomaticComplexity.ComplexityArtifacts_High.GetHashCode());
                    previousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                  distributionId.GetHashCode(),
                                                                                  Constants.CyclomaticComplexity.ComplexityArtifacts_VeryHigh.GetHashCode());


                }

                #endregion Previous Snapshot

                #region Data
                distributionName = CastComplexityUtility.GetCostComplexityName(reportData.CurrentSnapshot, distributionId.GetHashCode());

                rowData.AddRange(new string[] { distributionName, Labels.Current, Labels.Previous, Labels.Evol, Labels.EvolPercent, Labels.TotalPercent });
                
                rowData.AddRange(new string[]
                    { Labels.ComplexityLow
                    , selectedLowVal.HasValue ? selectedLowVal.Value.ToString("N0"):Constants.No_Value
                    , previousLowVal.HasValue ? previousLowVal.Value.ToString("N0") : Constants.No_Value
                    , (selectedLowVal.HasValue && previousLowVal.HasValue) ? TableBlock.FormatEvolution((Int32)(selectedLowVal.Value - previousLowVal.Value)): Constants.No_Value
                    , (selectedLowVal.HasValue && previousLowVal.HasValue && previousLowVal.Value !=0)? TableBlock.FormatPercent((selectedLowVal - previousLowVal) / previousLowVal)
                                                                                                      : Constants.No_Value
                    , (selectedLowVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedLowVal / selectedTotal, false): Constants.No_Value
                    });

                rowData.AddRange(new string[]
                    { Labels.ComplexityAverage
                    , selectedAveVal.HasValue ? selectedAveVal.Value.ToString("N0"): Constants.No_Value
                    , previousAveVal.HasValue ? previousAveVal.Value.ToString("N0") : Constants.No_Value
                    , (selectedAveVal.HasValue && previousAveVal.HasValue) ? TableBlock.FormatEvolution((Int32)(selectedAveVal.Value - previousAveVal.Value)) : Constants.No_Value
                    , (selectedAveVal.HasValue && previousAveVal.HasValue && previousAveVal.Value !=0)? TableBlock.FormatPercent((selectedAveVal - previousAveVal) / previousAveVal)
                                                                                                      : Constants.No_Value
                    , (selectedAveVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedAveVal / selectedTotal, false): Constants.No_Value
                    });

                rowData.AddRange(new string[]
                    { Labels.ComplexityHigh
                    , selectedHigVal.Value.ToString("N0")
                    , previousHigVal.HasValue ? previousHigVal.Value.ToString("N0") : Constants.No_Value
                    , previousHigVal.HasValue ? TableBlock.FormatEvolution((Int32)(selectedHigVal.Value - previousHigVal.Value)): Constants.No_Value
                    , (selectedHigVal.HasValue && previousHigVal.HasValue && previousHigVal.Value !=0)? TableBlock.FormatPercent((selectedHigVal - previousHigVal) / previousHigVal)
                                                                                                      : Constants.No_Value
                    , (selectedHigVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedHigVal / selectedTotal, false): Constants.No_Value
                    });

                rowData.AddRange(new string[]
                    { Labels.ComplexityExtreme
                    , selectedVhiVal.HasValue? selectedVhiVal.Value.ToString("N0"): Constants.No_Value
                    , previousVhiVal.HasValue ? previousVhiVal.Value.ToString("N0") : Constants.No_Value
                    , (selectedVhiVal.HasValue && previousVhiVal.HasValue) ? TableBlock.FormatEvolution((Int32)(selectedVhiVal.Value - previousVhiVal.Value)): Constants.No_Value
                    , (selectedVhiVal.HasValue && previousVhiVal.HasValue && previousVhiVal.Value !=0)? TableBlock.FormatPercent((selectedVhiVal - previousVhiVal) / previousVhiVal)
                                                                                                      : Constants.No_Value
                    , (selectedVhiVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedVhiVal / selectedTotal, false): Constants.No_Value
                    });
                
                #endregion Data

                result = new TableDefinition
                {
                    Data = rowData,
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbColumns = 6,
                    NbRows = 5
                };
            }
            return result;
        }
    }
}
