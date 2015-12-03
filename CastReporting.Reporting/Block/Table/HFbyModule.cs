
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
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.Reporting.Block.Table
{
    [Block("HF_BY_MODULE")]
    class HFbyModule : TableBlock
    {
        private const string _MetricFormat = "N2";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);
            Int32 nbRows = 0;

            List<string> rowData = new List<string>();
            rowData.AddRange(displayShortHeader
                                ? new[] { " ", "TQI", "Robu.", "Efcy.", "Secu.", "Trans.", "Chang." }
                                : new[] { " ", "TQI", "Robustness", "Efficiency", "Security", "Transferability", "Changeability" });

            

            //Current snpashot
            var resultCurrentSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.CurrentSnapshot);

            rowData.AddRange(new[] { reportData.CurrentSnapshot.ToString(), " ", " ", " ", " ", " ", " " });
            
            foreach(var  result in resultCurrentSnapshot.OrderBy(_ => _.Name))
            {
                rowData.AddRange(new[] {
                            result.Name,
                            result.TQI.Value.ToString(_MetricFormat),
                            result.Robustness.Value.ToString(_MetricFormat),
                            result.Performance.Value.ToString(_MetricFormat),
                            result.Security.Value.ToString(_MetricFormat),
                            result.Transferability.Value.ToString(_MetricFormat),
                            result.Changeability.Value.ToString(_MetricFormat)
                        });
            }

            nbRows += resultCurrentSnapshot.Count+2;

            //previous snpashot
            var resultPreviousSnapshot = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(reportData.PreviousSnapshot);

            if(resultPreviousSnapshot != null)
            {
                rowData.AddRange(new[] {" "," "," "," "," "," "," "});
                rowData.AddRange(new[] {  reportData.PreviousSnapshot.ToString(), " ", " ", " ", " ", " ", " " });

                foreach (var result in resultPreviousSnapshot.OrderBy(_ => _.Name))
                {
                    rowData.AddRange(new[] {
                                result.Name,
                                result.TQI.Value.ToString(_MetricFormat),
                                result.Robustness.Value.ToString(_MetricFormat),
                                result.Performance.Value.ToString(_MetricFormat),
                                result.Security.Value.ToString(_MetricFormat),
                                result.Transferability.Value.ToString(_MetricFormat),
                                result.Changeability.Value.ToString(_MetricFormat)
                            });
                }

                nbRows += resultPreviousSnapshot.Count+2;

                //Variation             
                var variationList = (from current in resultCurrentSnapshot
                                    join pevious in resultPreviousSnapshot
                                    on current.Name equals pevious.Name
                                    select new
                                    {
                                        current.Name,
                                        Variation = (current - pevious) / pevious                               
                                    }).ToList();
                
                rowData.AddRange(new[] {" "," "," "," "," "," "," "});
                rowData.AddRange(new[] { "Variation", " ", " ", " ", " ", " ", " " });

                foreach (var result in variationList.OrderBy(_ => _.Name))
                {
                    rowData.AddRange(new[] {
                                result.Name,
                                TableBlock.FormatPercent(result.Variation.TQI),
                                TableBlock.FormatPercent(result.Variation.Robustness),
                                TableBlock.FormatPercent(result.Variation.Performance),
                                TableBlock.FormatPercent(result.Variation.Security),
                                TableBlock.FormatPercent(result.Variation.Transferability),
                                TableBlock.FormatPercent(result.Variation.Changeability)
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
