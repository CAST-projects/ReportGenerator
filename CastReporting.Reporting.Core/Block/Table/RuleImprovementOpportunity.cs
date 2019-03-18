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
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    /// <summary>
    /// Rule Improvement Opportunity Class
    /// </summary>
    [Block("RULE_IMPROVEMENT_OPPORTUNITY")]
    public class RuleImprovementOpportunity : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;
            int nbLimitTop;
            int order;
            List<string> rowData = new List<string>();
            List<RuleVariationResultDTO> variationRules = new List<RuleVariationResultDTO>();

            rowData.AddRange(new[] {
				Labels.RuleName,
				Labels.ViolationsCurrent,
				Labels.ViolationsPrevious,
				Labels.Evolution,
				Labels.Grade,
				Labels.GradeEvolution
			});

            int? metricId = (options != null && options.ContainsKey("PAR")) ? int.Parse(options["PAR"]) : (int?)null;

            if (options == null || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }
            if (options == null || !options.ContainsKey("C") || !int.TryParse(options["C"], out order)) {
                order = 0;
            }

            if (reportData?.CurrentSnapshot != null && metricId.HasValue) {
                var currentCriticalRulesViolation = RulesViolationUtility.GetAllRuleViolations(reportData.CurrentSnapshot,
                                                                                            Constants.RulesViolation.All,
                                                                                            (Constants.BusinessCriteria)metricId,
                                                                                            true);


                var previousCriticalRulesViolation = (reportData.PreviousSnapshot != null) ? RulesViolationUtility.GetAllRuleViolations(reportData.PreviousSnapshot,
                                                                                                                                     Constants.RulesViolation.All,
                                                                                                                                    (Constants.BusinessCriteria)metricId,
                                                                                                                                     false)
                                                                                           : null;


                if (currentCriticalRulesViolation != null) {
                    foreach (var item in currentCriticalRulesViolation) {
                        //Get previous value
                        var previousitem = previousCriticalRulesViolation?.FirstOrDefault(_ => _.Rule.Key == item.Rule.Key);
                        double? previousVal = previousitem?.TotalFailed;
                        double? previousGrade = previousitem?.Grade;

                        //Compute the varioation
                        double? variationVal = (item.TotalFailed.HasValue && previousVal.HasValue) ? (item.TotalFailed.Value - previousVal.Value) : (double?)null;
                        double? variationGrade = (item.Grade.HasValue && previousGrade.HasValue) ? (item.Grade.Value - previousGrade.Value) : (double?)null;

                        variationRules.Add(new RuleVariationResultDTO {
                                                Rule = new RuleDetailsDTO { Name = item.Rule.Name, Key = item.Rule.Key },
                                                CurrentNbViolations = item.TotalFailed ?? -1,
                                                PreviousNbViolations = previousitem?.TotalFailed ?? -1,
                                                Evolution = (variationVal.HasValue && previousVal > 0) ? variationVal / previousVal : double.NaN,
                                                Grade = item.Grade ?? double.NaN,
                                                GradeEvolution = (variationGrade.HasValue && previousGrade > 0) ? variationGrade / previousGrade : double.NaN
                                            });
                    }

                    IEnumerable<RuleVariationResultDTO> selected_elements;
                    switch (order)
                    {
                        case 0:
                            selected_elements = variationRules.Where(_ => _.Grade != null).OrderByDescending(_ => _.Rule.CompoundedWeight*(4 - _.Grade.Value)).Take(nbLimitTop);
                            break;
                        case 1:
                            selected_elements = variationRules.Where(_ => _.GradeEvolution >= 0).OrderByDescending(_ => _.GradeEvolution).Take(nbLimitTop);
                            break;
                        case 2:
                            selected_elements = variationRules.Where(_ => _.GradeEvolution < 0).OrderBy(_ => _.GradeEvolution).Take(nbLimitTop);
                            break;
                        default:
                            selected_elements = variationRules.Where(_=> _.Grade != null).OrderByDescending(_ => _.Rule.CompoundedWeight*(4 - _.Grade.Value)).Take(nbLimitTop);
                            break;
                    }
                    
                    foreach (var varRule in selected_elements)
                    {
                        rowData.AddRange(new[] 
                                { 
                                      varRule.Rule.Name
                                    , (varRule.CurrentNbViolations.HasValue && varRule.CurrentNbViolations.Value != -1)? varRule.CurrentNbViolations.Value.ToString("N0"): Constants.No_Value
                                    , (varRule.PreviousNbViolations.HasValue && varRule.PreviousNbViolations.Value != -1)? varRule.PreviousNbViolations.Value.ToString("N0"): Constants.No_Value
                                    , (varRule.Evolution.HasValue && !double.IsNaN(varRule.Evolution.Value)) ? FormatPercent(varRule.Evolution.Value) : Constants.No_Value
                                    , (varRule.Grade.HasValue && !double.IsNaN(varRule.Grade.Value)) ? varRule.Grade.Value.ToString("N2"):Constants.No_Value
                                    , (varRule.GradeEvolution.HasValue && !double.IsNaN(varRule.GradeEvolution.Value)) ? FormatPercent(varRule.GradeEvolution.Value) : Constants.No_Value
                               }
                            );
						rowCount++;
                    }
                } else {
					rowData.AddRange(new[] {
						Labels.NoItem,
						string.Empty,
						string.Empty,
						string.Empty,
						string.Empty
					});
                    rowCount = 1;
                }
            }

            var resultTable = new TableDefinition {
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
