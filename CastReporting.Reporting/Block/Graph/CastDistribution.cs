
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
using CastReporting.Reporting.Languages;
using CastReporting.Domain;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("CAST_DISTRIBUTION")]
    public class CastDistribution : GraphBlock
    {

     

        #region METHODS
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
            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;

            bool hasPreviousSnapshot = (null != reportData.PreviousSnapshot);
            string previousName = String.Empty, selectedName = String.Empty;
           

            if (null != options && options.ContainsKey("PAR") && Int32.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;

           
           
            if (null != reportData && null != reportData.CurrentSnapshot)
            {
                selectedName = reportData.CurrentSnapshot.Annotation.Version;

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

               

                #endregion Selected Snapshot

                #region Previous Snapshot


                if (hasPreviousSnapshot)
                {
                    previousName = reportData.PreviousSnapshot.Annotation.Version;

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
                rowData.Add(" ");
                rowData.Add(selectedName);
                if (hasPreviousSnapshot) rowData.Add(previousName); 

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }

                rowData.Add(Labels.CplxLow);
                rowData.Add(selectedLowVal.GetValueOrDefault().ToString());
                if (hasPreviousSnapshot) rowData.Add(previousLowVal.GetValueOrDefault().ToString()); 

                rowData.Add(Labels.CplxAverage);
                rowData.Add(selectedAveVal.GetValueOrDefault().ToString());
                if (hasPreviousSnapshot) rowData.Add(previousAveVal.GetValueOrDefault().ToString());               

                rowData.Add(Labels.CplxHigh);
                rowData.Add(selectedHigVal.GetValueOrDefault().ToString() );
                if (hasPreviousSnapshot) rowData.Add(previousHigVal.GetValueOrDefault().ToString());                      

                rowData.Add(Labels.CplxVeryHigh);
                rowData.Add(selectedVhiVal.GetValueOrDefault().ToString() );
                if (hasPreviousSnapshot) rowData.Add(previousVhiVal.GetValueOrDefault().ToString());    

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }
                #endregion Data


            }
            result = new TableDefinition
            {
                Data = rowData,
                HasRowHeaders = false,
                HasColumnHeaders = false,
                NbColumns = hasPreviousSnapshot ? 3 : 2,
                NbRows = 7
            };

            return result;
        }
        #endregion METHODS

    }
}

