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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CAST_COMPLEXITY")]
    public class CastComplexity : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

            TableDefinition back = new TableDefinition();

            bool hasPreviousSnapshot = (null != reportData.PreviousSnapshot);
            double? selectedLowVal = null;
            double? selectedAveVal = null;
            double? selectedHigVal = null;
            double? selectedVhiVal = null;
            double? selectedTotal = null;
            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;            

                
            if (null != reportData && null != reportData.CurrentSnapshot)
            {

                #region Selected Snapshot

                selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                Constants.CostComplexity.CostComplexityArtifacts_Low.GetHashCode());
                selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                               Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                               Constants.CostComplexity.CostComplexityArtifacts_Average.GetHashCode());
                selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());
                selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                 Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());

                selectedTotal = 0;
                if (selectedLowVal.HasValue) selectedTotal += selectedLowVal;
                if (selectedAveVal.HasValue) selectedTotal += selectedAveVal;
                if (selectedHigVal.HasValue) selectedTotal += selectedHigVal;
                if (selectedVhiVal.HasValue) selectedTotal += selectedVhiVal;

                #endregion Selected Snapshot

                #region Previous Snapshot
                if (hasPreviousSnapshot)
                {
                    previousLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                  Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                  Constants.CostComplexity.CostComplexityArtifacts_Low.GetHashCode());
                    previousAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                   Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                   Constants.CostComplexity.CostComplexityArtifacts_Average.GetHashCode());
                    previousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                    Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());
                    previousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                     Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());
                }

                #endregion Previous Snapshot

                #region Data
                List<string> rowData = new List<string>();
                rowData.AddRange(new string[] { Labels.Complexity, Labels.Current, Labels.Previous,  Labels.Evol, Labels.EvolPercent, Labels.TotalPercent });

                const string noData = Constants.No_Data;
           
                rowData.AddRange(new string[]
                    { Labels.ComplexityLow
                    , selectedLowVal.HasValue ? selectedLowVal.Value.ToString("N0") : noData
                    , previousLowVal.HasValue ? previousLowVal.Value.ToString("N0") : noData
                    , (selectedLowVal.HasValue && previousLowVal.HasValue) ? TableBlock.FormatEvolution((Int32)(selectedLowVal.Value - previousLowVal.Value)) : noData
                    , (selectedLowVal.HasValue && previousLowVal.HasValue && previousLowVal.Value !=0)? TableBlock.FormatPercent((selectedLowVal - previousLowVal) / previousLowVal)
                                                                                                      : noData
                    , (selectedLowVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedLowVal / selectedTotal, false): noData
                    });

                rowData.AddRange(new string[]
                    { Labels.ComplexityAverage
                    , selectedAveVal.HasValue ? selectedAveVal.Value.ToString("N0"):noData
                    , previousAveVal.HasValue ? previousAveVal.Value.ToString("N0") : noData
                    , (selectedAveVal.HasValue && previousAveVal.HasValue) ? TableBlock.FormatEvolution((Int32)(selectedAveVal.Value - previousAveVal.Value)) : noData
                    , (selectedAveVal.HasValue && previousAveVal.HasValue && previousAveVal.Value !=0)? TableBlock.FormatPercent((selectedAveVal - previousAveVal) / previousAveVal)
                                                                                                      : noData
                    , (selectedAveVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedAveVal / selectedTotal, false): noData
                    });

                rowData.AddRange(new string[]
                    { Labels.ComplexityHigh
                    , selectedHigVal.HasValue ? selectedHigVal.Value.ToString("N0"): noData
                    , previousHigVal.HasValue ? previousHigVal.Value.ToString("N0") : noData
                    , (selectedHigVal.HasValue && previousHigVal.HasValue) ? TableBlock.FormatEvolution((Int32)(selectedHigVal.Value - previousHigVal.Value)) : noData
                    , (selectedHigVal.HasValue && previousHigVal.HasValue && previousHigVal.Value !=0)? TableBlock.FormatPercent((selectedHigVal - previousHigVal) / previousHigVal)
                                                                                                      : noData
                    , (selectedHigVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedHigVal / selectedTotal, false): noData
                    });

                rowData.AddRange(new string[]
                    { Labels.ComplexityVeryHigh
                    , selectedVhiVal.HasValue ? selectedVhiVal.Value.ToString("N0"): noData
                    , previousVhiVal.HasValue ? previousVhiVal.Value.ToString("N0") : noData
                    , previousVhiVal.HasValue ? TableBlock.FormatEvolution((Int32)(selectedVhiVal.Value - previousVhiVal.Value)): noData
                    , (selectedVhiVal.HasValue && previousVhiVal.HasValue && previousVhiVal.Value !=0)? TableBlock.FormatPercent((selectedVhiVal - previousVhiVal) / previousVhiVal)
                                                                                                      : noData
                    , (selectedVhiVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?TableBlock.FormatPercent(selectedVhiVal / selectedTotal, false): noData
                    });

                #endregion Data

                back = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    Data = rowData,
                    NbColumns = 6,
                    NbRows = 5
                };
            }
                                                                
            return back;
        }
    }
}
