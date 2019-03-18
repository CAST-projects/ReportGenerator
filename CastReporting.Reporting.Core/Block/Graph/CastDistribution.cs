
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
using System.Globalization;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("CAST_DISTRIBUTION")]
    public class CastDistribution : GraphBlock
    {

     

        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            int parId;
            Constants.QualityDistribution distributionId;

            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;

            bool hasPreviousSnapshot = (null != reportData.PreviousSnapshot);
            string previousName = string.Empty;


            if (null != options && options.ContainsKey("PAR") && int.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;

           
           
            if (reportData.CurrentSnapshot != null)
            {
                var selectedName = reportData.CurrentSnapshot.Annotation.Version;

                #region Selected Snapshot

                var selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    distributionId.GetHashCode(), "low");
                var selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    distributionId.GetHashCode(),"average");
                var selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    distributionId.GetHashCode(),"high");
                var selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    distributionId.GetHashCode(),"very_high");

               

                #endregion Selected Snapshot

                #region Previous Snapshot


                if (hasPreviousSnapshot)
                {
                    previousName = reportData.PreviousSnapshot.Annotation.Version;

                    previousLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                               distributionId.GetHashCode(),
                                                                               "low");
                    previousAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                   distributionId.GetHashCode(),
                                                                                  "average");
                    previousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                    distributionId.GetHashCode(),
                                                                                    "high");
                    previousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                    distributionId.GetHashCode(),
                                                                                    "very_high");

                                     
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
                rowData.Add(selectedLowVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                if (hasPreviousSnapshot) rowData.Add(previousLowVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)); 

                rowData.Add(Labels.CplxAverage);
                rowData.Add(selectedAveVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                if (hasPreviousSnapshot) rowData.Add(previousAveVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));               

                rowData.Add(Labels.CplxHigh);
                rowData.Add(selectedHigVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture) );
                if (hasPreviousSnapshot) rowData.Add(previousHigVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));                      

                rowData.Add(Labels.CplxVeryHigh);
                rowData.Add(selectedVhiVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture) );
                if (hasPreviousSnapshot) rowData.Add(previousVhiVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));    

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }
                #endregion Data


            }
            var result = new TableDefinition
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

