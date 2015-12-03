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

        

                const string metricFormat = "N0";

                var rowData = new List<string>() 
                    { "Name", "Current", "Previous", "Evolution", "% Evolution"

                    , "kLOCs"
                    , codeLineNumber.HasValue?  codeLineNumber.Value.ToString(metricFormat):CastReporting.Domain.Constants.No_Value
                    , codeLineNumberPrev.HasValue? codeLineNumberPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , hasPrevious? TableBlock.FormatEvolution((Int32)codeLineNumberEvol.Value) : CastReporting.Domain.Constants.No_Value
                    , (codeLineNumberPercent.HasValue)? TableBlock.FormatPercent(codeLineNumberPercent.Value): CastReporting.Domain.Constants.No_Value
                   
                    , "Files"
                    , fileNumber.HasValue? fileNumber.Value.ToString(metricFormat) :CastReporting.Domain.Constants.No_Value
                    , fileNumberPrev.HasValue? fileNumberPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , hasPrevious? TableBlock.FormatEvolution((Int32)fileNumberEvol.Value) : CastReporting.Domain.Constants.No_Value
                    , (fileNumberPercent.HasValue)? TableBlock.FormatPercent(fileNumberPercent.Value): CastReporting.Domain.Constants.No_Value
                    
                    , "Classes"
                    ,classNumber.HasValue?  classNumber.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value
                    , classNumberPrev.HasValue? classNumberPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , hasPrevious? TableBlock.FormatEvolution((Int32)classNumberEvol.Value) : CastReporting.Domain.Constants.No_Value
                    , (classNumberPercent.HasValue)? TableBlock.FormatPercent(classNumberPercent.Value): CastReporting.Domain.Constants.No_Value

                    , "SQL Art."
                    , sqlArtifactNumber.HasValue? sqlArtifactNumber.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , sqlArtifactNumberPrev.HasValue? sqlArtifactNumberPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , hasPrevious? TableBlock.FormatEvolution((Int32)sqlArtifactNumberEvol.Value) : CastReporting.Domain.Constants.No_Value
                    , (sqlArtifactNumberPercent.HasValue)? TableBlock.FormatPercent(sqlArtifactNumberPercent.Value): CastReporting.Domain.Constants.No_Value
                    
                    , "  Tables"
                    , tableNumber.HasValue? tableNumber.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value
                    , tableNumberPrev.HasValue? tableNumberPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , hasPrevious? TableBlock.FormatEvolution((Int32)tableNumberEvol.Value) : CastReporting.Domain.Constants.No_Value
                    , (tableNumberPercent.HasValue)? TableBlock.FormatPercent(tableNumberPercent.Value): CastReporting.Domain.Constants.No_Value
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
