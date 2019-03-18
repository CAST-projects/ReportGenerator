﻿/*
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
    /// TopNonCriticalViolations Class
    /// </summary>
    [Block("TOP_NON_CRITICAL_VIOLATIONS")]
    public class TopNonCriticalViolations : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbRows = 0;
            int nbLimitTop;
            List<string> rowData = new List<string>();
            
			rowData.AddRange(new[] {
				Labels.RuleName,
				Labels.ViolationsCount
			});
                                   
            int? metricId = ((options != null && options.ContainsKey("BC-ID")) ? int.Parse(options["BC-ID"]) : (int?)null) ?? ((options != null && options.ContainsKey("PAR")) ? int.Parse(options["PAR"]) : (int?)null);
            if (options == null || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

			if (reportData?.CurrentSnapshot != null) {

            	if (!metricId.HasValue)
            		metricId = 0;

                var _nonCriticalRulesViolation = RulesViolationUtility.GetRuleViolations(reportData.CurrentSnapshot, 
                                                                                        Constants.RulesViolation.NonCriticalRulesViolation,
                                                                                       (Constants.BusinessCriteria)metricId,
                                                                                       true, 
                                                                                       nbLimitTop);


                if (_nonCriticalRulesViolation != null && _nonCriticalRulesViolation.Any()) {                    
                    foreach (var elt in _nonCriticalRulesViolation) {
                        rowData.AddRange(new[] { elt.Rule.Name, elt.TotalFailed?.ToString("N0") });
                    }
                } else {
					rowData.AddRange(new[] {
						Labels.NoItem,
						string.Empty
					});
                }
			    if (_nonCriticalRulesViolation != null) nbRows = _nonCriticalRulesViolation.Count;
			}

            TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }
    }
}
