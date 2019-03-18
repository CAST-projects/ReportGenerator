
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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using System.Collections.Generic;
using System.Linq;
using CastReporting.BLL.Computing;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CRITERIA_GRADE")]
    public class CriteriaAndGrade : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            int count = -1, nbRows = 0, nbColumns = 0, nb;
            
            IList<string> strMetricId = ((options != null && options.ContainsKey("PAR")) ? options["PAR"] : string.Empty).Split(',');
                        
            if (null != options && options.ContainsKey("COUNT") && int.TryParse(options["COUNT"], out nb) && 0 < nb) {
                count = nb;
            }

            if (reportData.CurrentSnapshot != null)
            {
                //Result of current snapshot - the count is taken into account in the method to return results (if -1 return all results)
                var technicalCriteriasResults = BusinessCriteriaUtility.GetTechnicalCriteriasByBusinessCriterias(reportData.CurrentSnapshot, strMetricId, count);

                //Result of previous snapshot
                List<TechnicalCriteriaResultDTO> prevTechnicalCriteriasResults = null;
                if (reportData.PreviousSnapshot != null)
                    prevTechnicalCriteriasResults = BusinessCriteriaUtility.GetTechnicalCriteriasByBusinessCriterias(reportData.PreviousSnapshot, strMetricId, count)?.ToList();                

                if (prevTechnicalCriteriasResults != null && prevTechnicalCriteriasResults.Count != 0)
                {
                	nbColumns = 3;
                	
				    rowData.AddRange(new[] {
					    Labels.TechnicalCriterionName,
					    Labels.Grade,
					    Labels.Evolution
				    });
               
                    foreach (var grade in technicalCriteriasResults) {
                        rowData.Add(grade.Name);
                        rowData.Add(grade.Grade?.ToString("N2") ?? Constants.No_Value);

                    
                        var prevGrade = (from pgrade in prevTechnicalCriteriasResults
                                         where pgrade.Key == grade.Key
                                         select pgrade).FirstOrDefault();

                        string evol = Constants.No_Value;
                        if (prevGrade != null) {
                            double? variation = MathUtility.GetVariationPercent(grade.Grade, prevGrade.Grade);
                            evol = (variation.HasValue) ? FormatPercent(variation) : Constants.No_Value;
                        }                       

                        rowData.Add(evol);
					    nbRows++;
                    }
                }
                else
                {
                	nbColumns = 2;
                	
					rowData.AddRange(new[] {
						Labels.TechnicalCriterionName,
						Labels.Grade
					});
	               
	                foreach (var grade in technicalCriteriasResults) {
	                    rowData.Add(grade.Name);
	                    rowData.Add(grade.Grade?.ToString("N2") ?? Constants.No_Value);
						nbRows++;
	                }
                }
            }

            var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = nbColumns,
                Data = rowData
            };
            return resultTable;
        }
    }
}
