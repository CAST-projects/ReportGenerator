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
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Table
{
    /// <summary>
    /// Rule Improvement Opportunity Class
    /// </summary>
    [Block("RULES_LIST_LARGEST_VARIATION")]
    public class RulesListLargestVariation : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;
            List<string> rowData = new List<string>();
            List<RuleViolationsVariationResultDTO> variationRules = new List<RuleViolationsVariationResultDTO>();

            rowData.AddRange(new[] {
                Labels.Weight,
                Labels.Variation,
				Labels.RuleName
			});

            List<string> bcIdsStr = options.GetOption("BCID", "60017").Split('|').ToList();
            List<int> bcIds = bcIdsStr.Select(int.Parse).ToList();

            // to get decrease by default
            bool increase = options.GetOption("VARIATION", "DECREASE").ToLower().Equals("increase");
            // to get number by default
            bool percent = options.GetOption("DATA", "NUMBER").ToLower().Equals("percent");

            int nbLimitTop = options.GetIntOption("COUNT", 50);

            if (reportData?.CurrentSnapshot != null) {
                List<RuleViolationResultDTO> currentCriticalRulesViolation = 
                    RulesViolationUtility.GetNbViolationByRule(reportData.CurrentSnapshot, reportData.RuleExplorer, bcIds, -1);
                
                List<RuleViolationResultDTO> previousCriticalRulesViolation = (reportData.PreviousSnapshot != null) ? 
                    RulesViolationUtility.GetNbViolationByRule(reportData.PreviousSnapshot, reportData.RuleExplorer, bcIds, -1)
                    : null;

                if (currentCriticalRulesViolation != null && previousCriticalRulesViolation != null)
                {

                    if (increase)
                    {
                        foreach (RuleViolationResultDTO item in currentCriticalRulesViolation)
                        {
                            RuleViolationResultDTO previousitem = previousCriticalRulesViolation.FirstOrDefault(_ => _.Rule.Key == item.Rule.Key);
                            double? variation;

                            if (previousitem != null)
                            {
                                if (item.TotalChecks != null && previousitem.TotalChecks != null)
                                {
                                    variation = percent
                                        ? item.TotalFailed / (double)item.TotalChecks - previousitem.TotalFailed / (double)previousitem.TotalChecks
                                        : item.TotalFailed - previousitem.TotalFailed;
                                    if (variation <= 0) continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (item.TotalChecks != null)
                                {
                                    variation = percent
                                        ? item.TotalFailed / (double)item.TotalChecks
                                        : item.TotalFailed;
                                    if (variation <= 0) continue;
                                }
                                else
                                {
                                    continue;
                                }
                                    
                            }

                            variationRules.Add(new RuleViolationsVariationResultDTO
                            {
                                Rule = new RuleDetailsDTO {Name = item.Rule.Name, Key = item.Rule.Key, CompoundedWeight = item.Rule.CompoundedWeight},
                                CurrentNbViolations = item.TotalFailed ?? -1,
                                PreviousNbViolations = previousitem?.TotalFailed ?? -1,
                                Variation = variation ?? double.NaN
                            });

                        }
                    }
                    else
                    {
                        foreach (RuleViolationResultDTO previousitem in previousCriticalRulesViolation)
                        {
                            RuleViolationResultDTO item = currentCriticalRulesViolation.FirstOrDefault(_ => _.Rule.Key == previousitem.Rule.Key);
                            double? variation;

                            if (item != null)
                            {
                                if (previousitem.TotalChecks != null && item.TotalChecks!= null)
                                {
                                    variation = percent
                                        ? previousitem.TotalFailed / (double)previousitem.TotalChecks - item.TotalFailed / (double)item.TotalChecks
                                        : previousitem.TotalFailed - item.TotalFailed;
                                    if (variation <= 0) continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (previousitem.TotalChecks != null)
                                {
                                    variation = percent
                                        ? previousitem.TotalFailed / (double) previousitem.TotalChecks
                                        : previousitem.TotalFailed;
                                    if (variation <= 0) continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            variationRules.Add(new RuleViolationsVariationResultDTO
                            {
                                Rule = new RuleDetailsDTO {Name = previousitem.Rule.Name, Key = previousitem.Rule.Key, CompoundedWeight = previousitem.Rule.CompoundedWeight},
                                CurrentNbViolations = item?.TotalFailed ?? -1,
                                PreviousNbViolations = previousitem.TotalFailed ?? -1,
                                Variation = variation ?? double.NaN
                            });

                        }
                    }

                    List<RuleViolationsVariationResultDTO> selected_elements = nbLimitTop != -1 ? variationRules.OrderByDescending(_ => _.Variation).Take(nbLimitTop).ToList() : variationRules.OrderByDescending(_ => _.Variation).ToList();
                    if (selected_elements.Count <= 0)
                    {
                        rowData.AddRange(new[]
                        {
                            Labels.NoItem,
                            string.Empty,
                            string.Empty
                        });
                        rowCount = 1;
                    }
                    else
                    {
                        foreach (RuleViolationsVariationResultDTO varRule in selected_elements)
                        {
                            rowData.AddRange(new[]
                                {
                                    varRule.Rule.CompoundedWeight.ToString(),
                                    percent ? FormatPercent(varRule.Variation) : varRule.Variation.ToString(),
                                    varRule.Rule.Name
                                }
                            );
                            rowCount++;
                        }
                    }
                }
                else
                {
                    rowData.AddRange(new[]
                    {
                        Labels.NoItem,
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
                NbColumns = 3,
                Data = rowData
            };
            return resultTable;
        }
    }
}
