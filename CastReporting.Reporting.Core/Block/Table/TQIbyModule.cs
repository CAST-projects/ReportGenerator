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
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TQI_BY_MODULE")]
    public class TQIbyModule : TableBlock
    {
        /// <summary>
        /// 
        /// </summary>
        private const string MetricFormat = "N2";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            bool isDisplayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

            List<string> rowData = new List<string>();
            rowData.AddRange(isDisplayShortHeader
            	? new[] { " ", Labels.TQICur, Labels.TQIPrev, Labels.Var }
                : new[] { " ", Labels.TQICurrent, Labels.TQIPrevious, Labels.Variation });

            var resultCurrentSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.CurrentSnapshot, false);
            var resultPreviousSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.PreviousSnapshot, false);
            
            int count = 0;            
            if (resultCurrentSnapshot != null) {

                if (resultPreviousSnapshot == null) resultPreviousSnapshot = new List<BusinessCriteriaDTO>();

                var results =  from current in resultCurrentSnapshot
                               join prev in resultPreviousSnapshot on current.Name equals prev.Name
                               into g
                               from subset in g.DefaultIfEmpty()
                               select new
                               {
                                   current.Name,
                                   TqiCurrent = current.TQI,
                                   TqiPrevious = subset?.TQI,
                                   PercentVariation =  subset != null ? MathUtility.GetVariationPercent(current.TQI, subset.TQI):null
                               };


                foreach (var result in results.OrderBy(_ => _.Name)) {
                    rowData.AddRange(new[] {
                            result.Name,
                            result.TqiCurrent?.ToString(MetricFormat) ?? Domain.Constants.No_Value,     
                            result.TqiPrevious?.ToString(MetricFormat) ?? Domain.Constants.No_Value,     
                            result.PercentVariation.HasValue ? FormatPercent(result.PercentVariation):Domain.Constants.No_Value,
                        });
					count++;
                }
            }

            var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = count + 1,
                NbColumns = 4,
                Data = rowData
            };

            return resultTable;
        }
    }
}
