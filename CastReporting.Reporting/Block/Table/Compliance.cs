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
    [Block("COMPLIANCE")]
    class Compliance : TableBlock
    {   
        #region CONSTANTS
        private const string _format_metricFormat_us = "0.#0";
        private const string _format_metricFormat_fr = "0,#0";
        #endregion CONSTANTS

        #region ATTRIBUTES
        private int programmingPracticesId = Constants.BusinessCriteria.ProgrammingPractices.GetHashCode();
        private int architecturalDesignId = Constants.BusinessCriteria.ArchitecturalDesign.GetHashCode();
        private int documentationId = Constants.BusinessCriteria.Documentation.GetHashCode();
        #endregion ATTRIBUTES

         #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

          
            string metricFormat = "N2";
            TableDefinition resultTable = null;
            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

            if (null != reportData &&
                null != reportData.CurrentSnapshot &&
                null != reportData.CurrentSnapshot.BusinessCriteriaResults)
            {


                bool hasPreviousSnapshot = null != reportData.PreviousSnapshot && null != reportData.PreviousSnapshot.BusinessCriteriaResults;
                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot);


                string prevSnapshotLabel = hasPreviousSnapshot ? SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot) : CastReporting.Domain.Constants.No_Value;
                BusinessCriteriaDTO prevSnapshotBisCriDTO = hasPreviousSnapshot ? BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot) : null;


                double? currProgrammingPracticesValue = null != currSnapshotBisCriDTO.ProgrammingPractices ? currSnapshotBisCriDTO.ProgrammingPractices : 1;
                double? currArchitecturalDesignValue = null != currSnapshotBisCriDTO.ArchitecturalDesign ? currSnapshotBisCriDTO.ArchitecturalDesign : 1;
                double? currDocumentationValue = null != currSnapshotBisCriDTO.Documentation ?currSnapshotBisCriDTO.Documentation : 1;


                double? prevProgrammingPracticesValue = hasPreviousSnapshot?prevSnapshotBisCriDTO.ProgrammingPractices : 0;
                double? prevArchitecturalDesignValue = hasPreviousSnapshot ?prevSnapshotBisCriDTO.ArchitecturalDesign : 0;
                double? prevDocumentationValue = hasPreviousSnapshot?prevSnapshotBisCriDTO.Documentation : 0;

                double? varProgrammingPractices = MathUtility.GetPercent(MathUtility.GetEvolution(currProgrammingPracticesValue.Value, prevProgrammingPracticesValue.Value),
                                                                                        prevProgrammingPracticesValue.Value);

                double? varArchitecturalDesign = MathUtility.GetPercent(MathUtility.GetEvolution(currArchitecturalDesignValue.Value, prevArchitecturalDesignValue.Value),
                                                                                       prevArchitecturalDesignValue.Value);

                double? varDocumentation = MathUtility.GetPercent(MathUtility.GetEvolution(currDocumentationValue.Value, prevDocumentationValue.Value), 
                                                                                 prevDocumentationValue.Value);



                List<string> rowData = new List<string>();
                if (displayShortHeader) { rowData.AddRange(new[] { "", "Prog.", "Arch.", "Doc." }); }
                else { rowData.AddRange(new[] { "", "Programming Practices", "Architectural Design", "Documentation" }); }

                rowData.AddRange(
                    new[]
                    {
                        currSnapshotLabel,
                        currProgrammingPracticesValue.HasValue? currProgrammingPracticesValue.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value,
                        currArchitecturalDesignValue.HasValue? currArchitecturalDesignValue.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value,
                        currDocumentationValue.HasValue? currDocumentationValue.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value
                    });
                if (hasPreviousSnapshot)
                {
                    rowData.AddRange(
                        new[]
                    {
                        prevSnapshotLabel,
                        prevProgrammingPracticesValue.HasValue? prevProgrammingPracticesValue.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value,
                        prevArchitecturalDesignValue.HasValue?prevArchitecturalDesignValue.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value,
                        prevDocumentationValue.HasValue?prevDocumentationValue.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value,
                        "Variation",
                        varProgrammingPractices.HasValue? TableBlock.FormatPercent(varProgrammingPractices.Value): CastReporting.Domain.Constants.No_Value,
                        varArchitecturalDesign.HasValue?TableBlock.FormatPercent(varArchitecturalDesign.Value): CastReporting.Domain.Constants.No_Value,
                        varDocumentation.HasValue?TableBlock.FormatPercent(varDocumentation.Value): CastReporting.Domain.Constants.No_Value
                    });
                }
                resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = hasPreviousSnapshot ? 4 : 2,
                    NbColumns = 4,
                    Data = rowData
                };
            }
            return resultTable;
        }
        #endregion METHODS
    }
}
