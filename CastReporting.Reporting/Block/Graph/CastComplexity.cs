
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
using CastReporting.Domain;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("CAST_COMPLEXITY")]
    public class CastComplexity : GraphBlock
    {
        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
        
            List<string> rowData = new List<string>();
            bool hasPreviousSnapshot = (null != reportData.PreviousSnapshot);
            string previousName = String.Empty, selectedName = String.Empty;

            double? selectedLowVal = null;
            double? selectedAveVal = null;
            double? selectedHigVal = null;
            double? selectedVhiVal = null;
            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;

            if (reportData != null && reportData.CurrentSnapshot != null)
            {
 
                #region Selected Snapshot

                selectedName = reportData.CurrentSnapshot.Annotation.Version;
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

                #endregion Selected Snapshot

                #region Previous Snapshot

                previousName = hasPreviousSnapshot ? reportData.PreviousSnapshot.Annotation.Version : "No previous snapshot selected";

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
                rowData.Add(" ");
                rowData.Add(selectedName);
                if (hasPreviousSnapshot) { rowData.Add(previousName); }

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }

                rowData.Add("Low");
                rowData.Add(selectedLowVal.GetValueOrDefault().ToString());
                if (hasPreviousSnapshot) { rowData.Add(previousLowVal.GetValueOrDefault().ToString()); }

                rowData.Add("Average");
                rowData.Add(selectedAveVal.GetValueOrDefault().ToString());
                if (hasPreviousSnapshot) { rowData.Add(previousAveVal.GetValueOrDefault().ToString()); }

                rowData.Add("High");
                rowData.Add(selectedHigVal.GetValueOrDefault().ToString());
                if (hasPreviousSnapshot) { rowData.Add(previousHigVal.GetValueOrDefault().ToString()); }

                rowData.Add("Very High");
                rowData.Add(selectedVhiVal.GetValueOrDefault().ToString());
                if (hasPreviousSnapshot) { rowData.Add(previousVhiVal.GetValueOrDefault().ToString()); }

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }
                #endregion Data


            }
            TableDefinition back = new TableDefinition
                {
                    Data = rowData,
                    HasRowHeaders = false,
                    HasColumnHeaders = false,
                    NbColumns = hasPreviousSnapshot ? 3 : 2,
                    NbRows = 7
                };
        
            return back;
        }
        #endregion METHODS
    }
        
}

