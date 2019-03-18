
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
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("HF_BY_MODULE")]
    public class HFbyModule : TableBlock
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            const string metricFormat = "N2";

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);
            int nbRows = 0;

            List<string> rowData = new List<string>();
            rowData.AddRange(displayShortHeader
                                ? new[] { " ", Labels.TQI, Labels.Robu, Labels.Efcy, Labels.Secu, Labels.Trans, Labels.Chang }
                                : new[] { " ", Labels.TQI, Labels.Robustness, Labels.Efficiency, Labels.Security, Labels.Transferability, Labels.Changeability });

            

            //Current snpashot
            var resultCurrentSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.CurrentSnapshot, false);

            rowData.AddRange(new[] { reportData.CurrentSnapshot.ToString(), " ", " ", " ", " ", " ", " " });
            
            foreach(var  result in resultCurrentSnapshot.OrderBy(_ => _.Name))
            {
                rowData.AddRange(new[] {
                            result.Name,
                            result.TQI?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Robustness?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Performance?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Security?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Transferability?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Changeability?.ToString(metricFormat) ?? Constants.No_Value
                        });
            }

            nbRows += resultCurrentSnapshot.Count+2;

            //previous snpashot
            var resultPreviousSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.PreviousSnapshot, false);

            if(resultPreviousSnapshot != null)
            {
                rowData.AddRange(new[] {" "," "," "," "," "," "," "});
                rowData.AddRange(new[] {  reportData.PreviousSnapshot.ToString(), " ", " ", " ", " ", " ", " " });

                foreach (var result in resultPreviousSnapshot.OrderBy(_ => _.Name))
                {
                    rowData.AddRange(new[] {
                                result.Name,
                            result.TQI?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Robustness?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Performance?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Security?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Transferability?.ToString(metricFormat) ?? Constants.No_Value,
                            result.Changeability?.ToString(metricFormat) ?? Constants.No_Value
                            });
                }

                nbRows += resultPreviousSnapshot.Count+2;

                //Variation             
                var variationList = (from current in resultCurrentSnapshot
                                    join previous in resultPreviousSnapshot
                                    on current.Name equals previous.Name
                                    select new
                                    {
                                        current.Name,
                                        Variation = (current - previous) / previous                               
                                    }).ToList();
                
                rowData.AddRange(new[] {" "," "," "," "," "," "," "});
                rowData.AddRange(new[] { Labels.Variation, " ", " ", " ", " ", " ", " " });

                foreach (var result in variationList.OrderBy(_ => _.Name))
                {
                    rowData.AddRange(new[] {
                                result.Name,
                                FormatPercent(result.Variation.TQI),
                                FormatPercent(result.Variation.Robustness),
                                FormatPercent(result.Variation.Performance),
                                FormatPercent(result.Variation.Security),
                                FormatPercent(result.Variation.Transferability),
                                FormatPercent(result.Variation.Changeability)
                            });
                }

                nbRows += variationList.Count+2;
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = 7,
                Data = rowData
            };
            
            return resultTable;
        }
       
    }
}
