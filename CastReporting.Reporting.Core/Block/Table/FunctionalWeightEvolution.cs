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
    /// <summary>
    /// TechnicalSizingEvolution Class
    /// </summary>
    [Block("FUNCTIONAL_WEIGHT_EVOLUTION")]
    public class FunctionalWeightEvolution : TableBlock
    {
         #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            if (reportData?.CurrentSnapshot == null) return resultTable;

            #region Declarations

            double? automatedFPoints = MeasureUtility.GetAutomatedIFPUGFunction(reportData.CurrentSnapshot);
            double? decisionPoints = MeasureUtility.GetDecisionPointsNumber(reportData.CurrentSnapshot);
            double? backFiredFPoints = MeasureUtility.GetBackfiredIFPUGFunction(reportData.CurrentSnapshot);

            double? automatedFPointsPrev = MeasureUtility.GetAutomatedIFPUGFunction(reportData.PreviousSnapshot);
            double? decisionPointsPrev = MeasureUtility.GetDecisionPointsNumber(reportData.PreviousSnapshot);
            double? backFiredFPointsPrev =MeasureUtility.GetBackfiredIFPUGFunction(reportData.PreviousSnapshot);

            double? automatedFPointsEvol = MathUtility.GetEvolution(automatedFPoints, automatedFPointsPrev);
            double? decisionPointsEvol = MathUtility.GetEvolution(decisionPoints, decisionPointsPrev);
            double? backFiredFPointsEvol = MathUtility.GetEvolution(backFiredFPoints, backFiredFPointsPrev);

            double? automatedFPointsPercent = MathUtility.GetPercent(automatedFPointsEvol, automatedFPointsPrev);
            double? decisionPointsPercent = MathUtility.GetPercent(decisionPointsEvol, decisionPointsPrev);
            double? backFiredFPointsPercent = MathUtility.GetPercent(backFiredFPointsEvol, backFiredFPointsPrev);

            bool hasPrevious = (reportData.PreviousSnapshot != null);

            #endregion

            const string noData = Constants.No_Value;
            const string metricFormat = "N0";

            var rowData = new List<string>
            { Labels.Name, Labels.Current, Labels.Previous, Labels.Evolution, Labels.EvolutionPercent

                , Labels.AutomatedFP
                , automatedFPoints?.ToString(metricFormat) ?? noData
                , (hasPrevious && automatedFPointsPrev.HasValue)?  automatedFPointsPrev.Value.ToString(metricFormat) : noData
                , (hasPrevious && automatedFPointsEvol.HasValue)? FormatEvolution((int)automatedFPointsEvol.Value) : noData
                , (automatedFPointsPercent.HasValue)? FormatPercent(automatedFPointsPercent.Value): noData
                , Labels.DecisionP
                , decisionPoints?.ToString(metricFormat) ?? noData
                , (hasPrevious&& decisionPointsPrev.HasValue)? decisionPointsPrev.Value.ToString(metricFormat) : noData
                , (hasPrevious && decisionPointsEvol.HasValue)? FormatEvolution((int)decisionPointsEvol.Value) : noData
                , (decisionPointsPercent.HasValue)? FormatPercent(decisionPointsPercent.Value): noData
                , Labels.BackfiredFP
                , backFiredFPoints?.ToString(metricFormat) ?? noData
                , (hasPrevious && backFiredFPointsPrev.HasValue)? backFiredFPointsPrev.Value.ToString(metricFormat) : noData
                , (hasPrevious && backFiredFPointsEvol.HasValue)? FormatEvolution((int)Math.Round(backFiredFPointsEvol.Value,0)) : noData
                , (backFiredFPointsPercent.HasValue)? FormatPercent(backFiredFPointsPercent.Value): noData
            };

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 4,
                NbColumns = 5,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
