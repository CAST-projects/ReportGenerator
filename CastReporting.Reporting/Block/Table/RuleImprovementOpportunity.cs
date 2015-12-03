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
    /// <summary>
    /// Rule Improvement Opportunity Class
    /// </summary>
    [Block("RULE_IMPROVEMENT_OPPORTUNITY")]
    internal class RuleImprovementOpportunity : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;
            int nbLimitTop = 0;
            int order = 0;
            List<string> rowData = new List<string>();
            TableDefinition resultTable = null;
            List<RuleVariationResultDTO> variationRules = new List<RuleVariationResultDTO>();
            //List<RuleVariationResultDTO> improvmentRules = new List<RuleVariationResultDTO>();
            //List<RuleVariationResultDTO> degradationRules = new List<RuleVariationResultDTO>();
            //List<RuleVariationResultDTO> selected_elements = new List<RuleVariationResultDTO>();
            IEnumerable<RuleVariationResultDTO> selected_elements;
            

            rowData.AddRange(new string[] { "Rule Name", "Current Violations", "Previous Violations", "Evolution", "Grade", "Grade Evolution" });

            Int32? metricId = (options != null && options.ContainsKey("PAR")) ? Convert.ToInt32(options["PAR"]) : (Int32?)null;

            if (options == null || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }
            if (options == null || !options.ContainsKey("C") || !Int32.TryParse(options["C"], out order))
            {
                order = 0;
            }

            if (reportData != null && reportData.CurrentSnapshot != null && metricId.HasValue)
            {
                var currentCriticalRulesViolation = RulesViolationUtility.GetAllRuleViolations(reportData.CurrentSnapshot,
                                                                                            Constants.RulesViolation.All,
                                                                                            (Constants.BusinessCriteria)metricId,
                                                                                            true);


                var previousCriticalRulesViolation = (reportData.PreviousSnapshot != null) ? RulesViolationUtility.GetAllRuleViolations(reportData.PreviousSnapshot,
                                                                                                                                     Constants.RulesViolation.All,
                                                                                                                                    (Constants.BusinessCriteria)metricId,
                                                                                                                                     false)
                                                                                           : null;


                if (currentCriticalRulesViolation != null)
                {
                    rowCount = currentCriticalRulesViolation.Count();
                    
                    foreach (var item in currentCriticalRulesViolation)
                    {
                        //Get previous value
                        var previousitem = (previousCriticalRulesViolation != null) ? previousCriticalRulesViolation.FirstOrDefault(_ => _.Rule.Key == item.Rule.Key) : null;
                        double? previousVal = (previousitem != null && previousitem.TotalFailed.HasValue) ? previousitem.TotalFailed.Value : (double?)null;
                        double? previousGrade = (previousitem != null && previousitem.Grade.HasValue) ? previousitem.Grade.Value : (double?)null;

                        //Compute the varioation
                        double? variationVal = (item.TotalFailed.HasValue && previousVal.HasValue) ? (item.TotalFailed.Value - previousVal.Value) : (double?)null;
                        double? variationGrade = (item.TotalFailed.HasValue && previousVal.HasValue) ? (item.Grade.Value - previousGrade.Value) : (double?)null;

                        variationRules.Add(new RuleVariationResultDTO
                                            {
                                                Rule = new RuleDetailsDTO { Name = item.Rule.Name, Key = item.Rule.Key },
                                                CurrentNbViolations = (item.TotalFailed.HasValue) ? item.TotalFailed.Value : -1,
                                                PreviousNbViolations = (previousitem != null && previousitem.TotalFailed.HasValue ) ? previousitem.TotalFailed.Value : -1,
                                                Evolution = (variationVal.HasValue && previousVal.HasValue && previousVal > 0) ? variationVal / previousVal : double.NaN,
                                                Grade = (item.Grade.HasValue) ? item.Grade.Value : double.NaN,
                                                GradeEvolution = (variationGrade.HasValue && previousGrade.HasValue && previousGrade > 0) ? variationGrade / previousGrade : double.NaN
                                            });
                    }

                    switch (order)
                    {
                        default:
                        case 0:
                            {
                                selected_elements = variationRules.OrderByDescending(_ => _.Rule.CompoundedWeight*(4-_.Grade.Value)).Take(nbLimitTop);
                                break;
                            }
                        case 1:
                            {
                                selected_elements = variationRules.Where(_ => _.GradeEvolution >= 0).OrderByDescending(_ => _.GradeEvolution).Take(nbLimitTop);
                                break;
                            }
                        case 2:
                            {
                                selected_elements = variationRules.Where(_ => _.GradeEvolution < 0).OrderBy(_ => _.GradeEvolution).Take(nbLimitTop);
                                break;
                            }
                    };

                    foreach (var varRule in selected_elements)
                    {
                        rowData.AddRange(new string[] 
                                { 
                                      varRule.Rule.Name
                                    , (varRule.CurrentNbViolations.HasValue && varRule.CurrentNbViolations.Value != -1)? varRule.CurrentNbViolations.Value.ToString("N0"): CastReporting.Domain.Constants.No_Value
                                    , (varRule.PreviousNbViolations.HasValue && varRule.PreviousNbViolations.Value != -1)? varRule.PreviousNbViolations.Value.ToString("N0"): CastReporting.Domain.Constants.No_Value
                                    , (varRule.Evolution.HasValue && !double.IsNaN(varRule.Evolution.Value)) ? TableBlock.FormatPercent(varRule.Evolution.Value) : Constants.No_Value
                                    , (varRule.Grade.HasValue && !double.IsNaN(varRule.Grade.Value)) ? varRule.Grade.Value.ToString("N2"):CastReporting.Domain.Constants.No_Value
                                    , (varRule.GradeEvolution.HasValue && !double.IsNaN(varRule.GradeEvolution.Value)) ? TableBlock.FormatPercent(varRule.GradeEvolution.Value) : Constants.No_Value
                               }
                            );
                    }

                }
                else
                {
                    rowData.AddRange(new string[] { "No enable item.", string.Empty, string.Empty, string.Empty, string.Empty });
                    rowCount = 1;
                }
            }

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowCount + 1,
                NbColumns = 6,
                Data = rowData
            };
            return resultTable;
        }

    }
}
