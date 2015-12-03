
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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.BLL.Computing;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CRITERIA_GRADE")]
    class CriteriaAndGrade : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            List<string> rowData = new List<string>();
            int count = reportData.Parameter.NbResultDefault, nbRows = 0, nb = 0;
            
            IList<string> strMetricId = ((options != null && options.ContainsKey("PAR")) ? options["PAR"] : string.Empty).Split(',');
                        
            if (null != options && options.ContainsKey("COUNT") && Int32.TryParse(options["COUNT"], out nb) && 0 < nb)
            {
                count = nb;
            }

            if (null != reportData && null != reportData.CurrentSnapshot)
            {
                //Result of current snapshot
                var technicalCriteriasResults = BusinessCriteriaUtility.GetTechnicalCriteriasByBusinessCriterias(reportData.CurrentSnapshot, strMetricId, count);
                technicalCriteriasResults = technicalCriteriasResults.Take(count).ToList();
                nbRows = technicalCriteriasResults.Count();

                //Result of previous snapshot
                IEnumerable<TechnicalCriteriaResultDTO> prevTechnicalCriteriasResults = new List<TechnicalCriteriaResultDTO>();
                if (reportData.PreviousSnapshot != null)
                    prevTechnicalCriteriasResults = BusinessCriteriaUtility.GetTechnicalCriteriasByBusinessCriterias(reportData.PreviousSnapshot, strMetricId, count);                

                rowData.AddRange(new[] { "Technical Criteria Name", "Grade", "Evolution" });

               
                foreach (var grade in technicalCriteriasResults)
                {
                    rowData.Add(grade.Name);
                    rowData.Add((grade.Grade.HasValue) ? grade.Grade.Value.ToString("N2") : Constants.No_Value);

                    
                    var prevGrade = (from pgrade in prevTechnicalCriteriasResults
                                     where pgrade.Key == grade.Key
                                     select pgrade).FirstOrDefault();

                    string evol = Constants.No_Value;
                    if (prevGrade != null)
                    {
                        double? variation = MathUtility.GetVariationPercent(grade.Grade, prevGrade.Grade);
                        evol = (variation.HasValue) ? TableBlock.FormatPercent(variation) : Constants.No_Value;
                    }                       

                    rowData.Add(evol);
                }
            }

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = 3,
                Data = rowData
            };
            return resultTable;
        }
    }
}
