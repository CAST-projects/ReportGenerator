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
using System;
using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CAST_COMPLEXITY")]
    public class CastComplexity : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

            TableDefinition back = new TableDefinition();

            bool hasPreviousSnapshot = (null != reportData.PreviousSnapshot);

            if (reportData.CurrentSnapshot == null) return back;

            #region Selected Snapshot

            var selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                Constants.CostComplexity.CostComplexityArtifacts_Low.GetHashCode());
            var selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                Constants.CostComplexity.CostComplexityArtifacts_Average.GetHashCode());
            var selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());
            var selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());

            double? selectedTotal = 0;
            if (selectedLowVal.HasValue) selectedTotal += selectedLowVal;
            if (selectedAveVal.HasValue) selectedTotal += selectedAveVal;
            if (selectedHigVal.HasValue) selectedTotal += selectedHigVal;
            if (selectedVhiVal.HasValue) selectedTotal += selectedVhiVal;

            #endregion Selected Snapshot

            #region Previous Snapshot
            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;
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
            rowData.AddRange(new[] { Labels.Complexity, Labels.Current, Labels.Previous,  Labels.Evol, Labels.EvolPercent, Labels.TotalPercent });

            const string noData = Constants.No_Value;
           
            rowData.AddRange(new[]
            { Labels.ComplexityLow
                , selectedLowVal?.ToString("N0") ?? noData
                , previousLowVal?.ToString("N0") ?? noData
                , (selectedLowVal.HasValue && previousLowVal.HasValue) ? FormatEvolution((int)(selectedLowVal.Value - previousLowVal.Value)) : noData
                , (selectedLowVal.HasValue && previousLowVal.HasValue && Math.Abs(previousLowVal.Value) > 0) ? FormatPercent((selectedLowVal - previousLowVal) / previousLowVal) : noData
                , (selectedLowVal.HasValue && selectedTotal.Value>0) ? FormatPercent(selectedLowVal / selectedTotal, false) : noData
            });

            rowData.AddRange(new[]
            { Labels.ComplexityAverage
                , selectedAveVal?.ToString("N0") ?? noData
                , previousAveVal?.ToString("N0") ?? noData
                , (selectedAveVal.HasValue && previousAveVal.HasValue) ? FormatEvolution((int)(selectedAveVal.Value - previousAveVal.Value)) : noData
                , (selectedAveVal.HasValue && previousAveVal.HasValue && Math.Abs(previousAveVal.Value) > 0) ? FormatPercent((selectedAveVal - previousAveVal) / previousAveVal) : noData
                , (selectedAveVal.HasValue && selectedTotal.Value>0) ? FormatPercent(selectedAveVal / selectedTotal, false): noData
            });

            rowData.AddRange(new[]
            { Labels.ComplexityHigh
                , selectedHigVal?.ToString("N0") ?? noData
                , previousHigVal?.ToString("N0") ?? noData
                , (selectedHigVal.HasValue && previousHigVal.HasValue) ? FormatEvolution((int)(selectedHigVal.Value - previousHigVal.Value)) : noData
                , (selectedHigVal.HasValue && previousHigVal.HasValue && Math.Abs(previousHigVal.Value) > 0) ? FormatPercent((selectedHigVal - previousHigVal) / previousHigVal) : noData
                , (selectedHigVal.HasValue && selectedTotal.Value > 0) ? FormatPercent(selectedHigVal / selectedTotal, false): noData
            });

            rowData.AddRange(new[]
            { Labels.ComplexityVeryHigh
                , selectedVhiVal?.ToString("N0") ?? noData
                , previousVhiVal?.ToString("N0") ?? noData
                , previousVhiVal.HasValue && selectedVhiVal.HasValue ? FormatEvolution((int)(selectedVhiVal.Value - previousVhiVal.Value)): noData
                , (selectedVhiVal.HasValue && previousVhiVal.HasValue && Math.Abs(previousVhiVal.Value) > 0)? FormatPercent((selectedVhiVal - previousVhiVal) / previousVhiVal) : noData
                , (selectedVhiVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0) ? FormatPercent(selectedVhiVal / selectedTotal, false): noData
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

            return back;
        }
    }
}
