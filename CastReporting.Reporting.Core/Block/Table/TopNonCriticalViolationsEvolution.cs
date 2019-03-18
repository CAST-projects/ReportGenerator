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
    /// TopNonCriticalViolationsEvolution Class
    /// </summary>
    [Block("TOP_NON_CRITICAL_VIOLATIONS_EVOLUTION")]
    public class TopNonCriticalViolationsEvolution : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;
            int nbLimitTop;
            List<string> rowData = new List<string>();
            List<RuleViolationsVariationResultDTO> variationRules = new List<RuleViolationsVariationResultDTO>();

            rowData.AddRange(new[] {
				Labels.RuleName,
				Labels.Current,
				Labels.Previous,
				Labels.Evolution,
				Labels.EvolutionPercent
			});

            int? metricId = ((options != null && options.ContainsKey("BC-ID")) ? int.Parse(options["BC-ID"]) : (int?)null) ?? ((options != null && options.ContainsKey("PAR")) ? int.Parse((options["PAR"])) : (int?)null);
            if (options == null || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            if (reportData?.CurrentSnapshot != null)
            {
                if (!metricId.HasValue) metricId = 0;

                var currentNonCriticalRulesViolation = RulesViolationUtility.GetAllRuleViolations(reportData.CurrentSnapshot,
                                                                                            Constants.RulesViolation.NonCriticalRulesViolation,
                                                                                            (Constants.BusinessCriteria)metricId,
                                                                                            true);


                var previousNonCriticalRulesViolation = (reportData.PreviousSnapshot != null) ? RulesViolationUtility.GetAllRuleViolations(reportData.PreviousSnapshot, Constants.RulesViolation.NonCriticalRulesViolation,
                                                                                                                                    (Constants.BusinessCriteria)metricId,
                                                                                                                                     false)
                                                                                           : null;


                if (currentNonCriticalRulesViolation != null) 
                {
                    rowCount += currentNonCriticalRulesViolation.Count;
                    foreach (var item in currentNonCriticalRulesViolation) 
                    {
                        //Get previous value
                        var previousitem = previousNonCriticalRulesViolation?.FirstOrDefault(_ => _.Rule.Key == item.Rule.Key);
                        double? previousval = previousitem?.TotalFailed;

                        //Compute the varioation
                        double? variation = (item.TotalFailed.HasValue && previousval.HasValue) ? (item.TotalFailed.Value - previousval.Value) : (double?)null;

                        variationRules.Add(new RuleViolationsVariationResultDTO
                        {
                            Rule = new RuleDetailsDTO { Name = item.Rule.Name, Key = item.Rule.Key },
                            CurrentNbViolations = item.TotalFailed ?? -1,
                            PreviousNbViolations = previousitem?.TotalFailed ?? -1,
                            Variation = variation ?? double.NaN,
                            Ratio = (variation.HasValue && previousval > 0) ? variation / previousval : double.NaN
                        });
                    }
                    var selectedRules = variationRules.OrderByDescending(_ => _.Ratio).Take(nbLimitTop);
                    foreach (var varRule in selectedRules)
                    {
                        rowData.AddRange(new[] 
                                    { 
                                          varRule.Rule.Name
                                        , (varRule.CurrentNbViolations.HasValue && varRule.CurrentNbViolations.Value != -1)? varRule.CurrentNbViolations.Value.ToString("N0"): Constants.No_Value
                                        , (varRule.PreviousNbViolations.HasValue && varRule.PreviousNbViolations.Value != -1)? varRule.PreviousNbViolations.Value.ToString("N0"): Constants.No_Value
                                        , (varRule.Variation.HasValue && !double.IsNaN(varRule.Variation.Value))? FormatEvolution((int)varRule.Variation.Value):Constants.No_Value
                                        ,  (varRule.Ratio.HasValue && !double.IsNaN(varRule.Ratio.Value)) ? FormatPercent(varRule.Ratio.Value) : Constants.No_Value
                                   }
                            );
                    }
                } 
                else 
                {
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
			    NbColumns = 5,
			    Data = rowData
			};
            return resultTable;
        }
    }
}
