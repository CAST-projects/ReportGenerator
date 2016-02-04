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
    /// <summary>
    /// TechnicalSizingEvolution Class
    /// </summary>
    [Block("TECHNICAL_SIZING_EVOLUTION")]
    class TechnicalSizingEvolution : TableBlock
    {
        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {  
           
             TableDefinition resultTable = null;
                    bool hasPrevious = reportData.PreviousSnapshot != null;
            if (null != reportData &&
                null != reportData.CurrentSnapshot)
            {
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

                const string noData = Constants.No_Data;
                const string metricFormat = "N0";

                var rowData = new List<string>() 
                    { Labels.Name, Labels.Current, Labels.Previous, Labels.Evolution, Labels.EvolutionPercent

                    , Labels.LoC
                    , codeLineNumber.HasValue?  codeLineNumber.Value.ToString(metricFormat):noData
                    , codeLineNumberPrev.HasValue? codeLineNumberPrev.Value.ToString(metricFormat) : noData
                    , hasPrevious? TableBlock.FormatEvolution((Int32)codeLineNumberEvol.Value) : noData
                    , (codeLineNumberPercent.HasValue)? TableBlock.FormatPercent(codeLineNumberPercent.Value): noData
                   
                    , "   " + Labels.Files
                    , fileNumber.HasValue? fileNumber.Value.ToString(metricFormat) :noData
                    , fileNumberPrev.HasValue? fileNumberPrev.Value.ToString(metricFormat) : noData
                    , hasPrevious? TableBlock.FormatEvolution((Int32)fileNumberEvol.Value) : noData
                    , (fileNumberPercent.HasValue)? TableBlock.FormatPercent(fileNumberPercent.Value): noData
                    
                    , "   " + Labels.Classes
                    , classNumber.HasValue?  classNumber.Value.ToString(metricFormat): noData
                    , classNumberPrev.HasValue? classNumberPrev.Value.ToString(metricFormat) : noData
                    , hasPrevious? TableBlock.FormatEvolution((Int32)classNumberEvol.Value) : noData
                    , (classNumberPercent.HasValue)? TableBlock.FormatPercent(classNumberPercent.Value): noData

                    , Labels.ArtifactsSQL
                    , sqlArtifactNumber.HasValue? sqlArtifactNumber.Value.ToString(metricFormat) : noData
                    , sqlArtifactNumberPrev.HasValue? sqlArtifactNumberPrev.Value.ToString(metricFormat) : noData
                    , hasPrevious? TableBlock.FormatEvolution((Int32)sqlArtifactNumberEvol.Value) : noData
                    , (sqlArtifactNumberPercent.HasValue)? TableBlock.FormatPercent(sqlArtifactNumberPercent.Value): noData
                    
                    , "   " + Labels.Tables
                    , tableNumber.HasValue? tableNumber.Value.ToString(metricFormat): noData
                    , tableNumberPrev.HasValue? tableNumberPrev.Value.ToString(metricFormat) : noData
                    , hasPrevious? TableBlock.FormatEvolution((Int32)tableNumberEvol.Value) : noData
                    , (tableNumberPercent.HasValue)? TableBlock.FormatPercent(tableNumberPercent.Value): noData
                    };

                resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 6,
                    NbColumns = 5,
                    Data = rowData
                };
            }
            return resultTable;
        }
        #endregion METHODS
 
    }
}
