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
    [Block("TECHNICAL_SIZING_EVOLUTION")]
    public class TechnicalSizingEvolution : TableBlock
    {
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            bool hasPrevious = reportData.PreviousSnapshot != null;
            if (reportData?.CurrentSnapshot == null) return null;

            #region CastComputing

            double? codeLineNumber = MeasureUtility.GetCodeLineNumber(reportData.CurrentSnapshot);
            double? fileNumber = MeasureUtility.GetFileNumber(reportData.CurrentSnapshot);
            double? classNumber = MeasureUtility.GetClassNumber(reportData.CurrentSnapshot);
            double? sqlArtifactNumber = MeasureUtility.GetSqlArtifactNumber(reportData.CurrentSnapshot);
            double? tableNumber = MeasureUtility.GetTableNumber(reportData.CurrentSnapshot);

            double? codeLineNumberPrev = MeasureUtility.GetCodeLineNumber(reportData.PreviousSnapshot);
            double? fileNumberPrev = MeasureUtility.GetFileNumber(reportData.PreviousSnapshot);
            double? classNumberPrev = MeasureUtility.GetClassNumber(reportData.PreviousSnapshot);
            double? sqlArtifactNumberPrev = MeasureUtility.GetSqlArtifactNumber(reportData.PreviousSnapshot);
            double? tableNumberPrev = MeasureUtility.GetTableNumber(reportData.PreviousSnapshot);



            double? codeLineNumberEvol = MathUtility.GetEvolution(codeLineNumber, codeLineNumberPrev);
            double? fileNumberEvol = MathUtility.GetEvolution(fileNumber, fileNumberPrev);
            double? classNumberEvol = MathUtility.GetEvolution(classNumber, classNumberPrev);
            double? sqlArtifactNumberEvol = MathUtility.GetEvolution(sqlArtifactNumber, sqlArtifactNumberPrev);
            double? tableNumberEvol = MathUtility.GetEvolution(tableNumber, tableNumberPrev);


            double? codeLineNumberPercent = MathUtility.GetPercent(codeLineNumberEvol, codeLineNumberPrev);
            double? fileNumberPercent = MathUtility.GetPercent(fileNumberEvol, fileNumberPrev);
            double? classNumberPercent = MathUtility.GetPercent(classNumberEvol, classNumberPrev);
            double? sqlArtifactNumberPercent = MathUtility.GetPercent(sqlArtifactNumberEvol, sqlArtifactNumberPrev);
            double? tableNumberPercent = MathUtility.GetPercent(tableNumberEvol, tableNumberPrev);

            #endregion CastComputing

            const string noData = Constants.No_Value;
            const string metricFormat = "N0";

            var rowData = new List<string>() 
            { Labels.Name, Labels.Current, Labels.Previous, Labels.Evolution, Labels.EvolutionPercent

                , Labels.LoC
                , codeLineNumber?.ToString(metricFormat) ?? noData
                , codeLineNumberPrev?.ToString(metricFormat) ?? noData
                , hasPrevious? FormatEvolution((int)codeLineNumberEvol.Value) : noData
                , (codeLineNumberPercent.HasValue)? FormatPercent(codeLineNumberPercent.Value): noData
                   
                , "   " + Labels.Files
                , fileNumber?.ToString(metricFormat) ?? noData
                , fileNumberPrev?.ToString(metricFormat) ?? noData
                , hasPrevious? FormatEvolution((int)fileNumberEvol.Value) : noData
                , (fileNumberPercent.HasValue)? FormatPercent(fileNumberPercent.Value): noData
                    
                , "   " + Labels.Classes
                , classNumber?.ToString(metricFormat) ?? noData
                , classNumberPrev?.ToString(metricFormat) ?? noData
                , hasPrevious? FormatEvolution((int)classNumberEvol.Value) : noData
                , (classNumberPercent.HasValue)? FormatPercent(classNumberPercent.Value): noData

                , Labels.ArtifactsSQL
                , sqlArtifactNumber?.ToString(metricFormat) ?? noData
                , sqlArtifactNumberPrev?.ToString(metricFormat) ?? noData
                , hasPrevious? FormatEvolution((int)sqlArtifactNumberEvol.Value) : noData
                , (sqlArtifactNumberPercent.HasValue)? FormatPercent(sqlArtifactNumberPercent.Value): noData
                    
                , "   " + Labels.Tables
                , tableNumber?.ToString(metricFormat) ?? noData
                , tableNumberPrev?.ToString(metricFormat) ?? noData
                , hasPrevious? FormatEvolution((int)tableNumberEvol.Value) : noData
                , (tableNumberPercent.HasValue)? FormatPercent(tableNumberPercent.Value): noData
            };

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 6,
                NbColumns = 5,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
 
    }
}
