/*
 *   Copyright (c) 2014 CAST
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
    [Block("BC_BY_TECHNO")]
    class TQIbyTechno : TableBlock
    {

        /// <summary>
        /// 
        /// </summary>
        private const string _MetricFormat = "N2";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            int nbTot = 0;
            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);
            List<string> rowData = new List<string>();
            Int32? metricId = (options != null && options.ContainsKey("ID")) ? Convert.ToInt32(options["ID"]) : (Int32?)null;

            if (null != reportData &&
                null != reportData.CurrentSnapshot && metricId.HasValue)
            {
                var result = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(r => r.Reference.Key == metricId).FirstOrDefault();
                
                if (result != null)
                {
                    string value = Text(metricId.Value);
                    rowData.AddRange(new string[] { "Techno", value });
                    foreach (var res in result.TechnologyResult)
                    {
                        rowData.AddRange(new string[] { res.Technology, res.DetailResult.Grade.ToString(_MetricFormat) });
                    }
                    nbTot = result.ModulesResult.Length;
                }
            }
            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbTot + 1,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }

        static string Text(int value)
        {
            
            // Begin the switch.
            switch (value)
            {
                case (int)Domain.Constants.BusinessCriteria.TechnicalQualityIndex:
                    {

                        return "TQI";
                    }
                case (int)Domain.Constants.BusinessCriteria.ArchitecturalDesign:
                    {

                        return "Architectural Design";
                    }
                case (int)Domain.Constants.BusinessCriteria.Changeability:
                    {

                        return "Changeability";
                    }
                case (int)Domain.Constants.BusinessCriteria.Documentation:
                    {

                        return "Documentation";
                    }
                case (int)Domain.Constants.BusinessCriteria.Performance:
                    {

                        return "Efficiency";
                    }
                case (int)Domain.Constants.BusinessCriteria.ProgrammingPractices:
                    {

                        return "Programming Practices";
                    }
                case (int)Domain.Constants.BusinessCriteria.Robustness:
                    {

                        return "Robustness";
                    }
                case (int)Domain.Constants.BusinessCriteria.Security:
                    {

                        return "Security";
                    }
                case (int)Domain.Constants.BusinessCriteria.SEIMaintainability:
                    {

                        return "SEI Maintainability";
                    }
                case (int)Domain.Constants.BusinessCriteria.Transferability:
                    {

                        return "Transferability";
                    }
        
                default:
                    return "N/A" ;
            }
        }



    }
}
