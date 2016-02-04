/*
 *   Copyright (c) 2016 CAST
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
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TOP_CRITICAL_VIOLATIONS_EVOLUTION")]
    class TopCriticalViolationsEvolution : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;
            int nbLimitTop = 0;
            List<string> rowData = new List<string>();
            TableDefinition resultTable = null;
            
			rowData.AddRange(new string[] {
				Labels.RuleName,
				Labels.Current,
				Labels.Previous,
				Labels.Evolution,
				Labels.EvolutionPercent
			});
          
            Int32? metricId = (options != null && options.ContainsKey("BC-ID")) ? Convert.ToInt32(options["BC-ID"]) : (Int32?)null;
            if (metricId == null)
                metricId = (options != null && options.ContainsKey("PAR")) ? Convert.ToInt32(options["PAR"]) : (Int32?)null;
            if (options == null || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            if (reportData != null && reportData.CurrentSnapshot != null) {
            	
            	if (!metricId.HasValue)
            		metricId = 0;
            	
                var currentCriticalRulesViolation = RulesViolationUtility.GetRuleViolations(reportData.CurrentSnapshot, 
                                                                                            Constants.RulesViolation.CriticalRulesViolation,
                                                                                            (Constants.BusinessCriteria)metricId,
                                                                                            true, 
                                                                                            nbLimitTop);


                var previousCriticalRulesViolation = (reportData.PreviousSnapshot != null) ? RulesViolationUtility.GetRuleViolations(reportData.PreviousSnapshot, 
                                                                                                                                     Constants.RulesViolation.CriticalRulesViolation,
                                                                                                                                    (Constants.BusinessCriteria)metricId, 
                                                                                                                                     false, 
                                                                                                                                     nbLimitTop) 
                                                                                           : null;
              

                if (currentCriticalRulesViolation != null) {
                    rowCount += currentCriticalRulesViolation.Count;
                    foreach (var item in currentCriticalRulesViolation) {
                        //Get previous value
                        var previousitem = (previousCriticalRulesViolation != null) ? previousCriticalRulesViolation.FirstOrDefault(_ => _.Rule.Key == item.Rule.Key) : null;
                        double? previousval = (previousitem != null && previousitem.TotalFailed.HasValue) ? previousitem.TotalFailed.Value : (double?)null;

                        //Compute the varioation
                        double? variation = (item.TotalFailed.HasValue && previousval.HasValue) ? (item.TotalFailed.Value - previousval.Value) : (double?)null;
                        
                        rowData.AddRange(new string[]  { 
                                      item.Rule.Name
                                    , (item.TotalFailed.HasValue)?item.TotalFailed.Value.ToString("N0"): CastReporting.Domain.Constants.No_Value
                                    , (previousitem != null && previousitem.TotalFailed.HasValue ) ? previousitem.TotalFailed.Value.ToString("N0"): CastReporting.Domain.Constants.No_Value
                                    , (variation.HasValue)? TableBlock.FormatEvolution((Int32)variation):CastReporting.Domain.Constants.No_Value
                                    , (variation.HasValue && previousval.HasValue && previousval > 0) ? TableBlock.FormatPercent(variation/previousval) : CastReporting.Domain.Constants.No_Value
                               });
                    }
                } else {
					rowData.AddRange(new string[] {
						Labels.NoItem,
						string.Empty,
						string.Empty,
						string.Empty,
						string.Empty
					});
                    rowCount = 1;
                }
            }

			resultTable = new TableDefinition {
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

