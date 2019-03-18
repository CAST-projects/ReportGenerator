
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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using System.Collections.Generic;
using System.Linq;


namespace CastReporting.Reporting.Block.Table
{
    [Block("ACTION_PLANS")]
    public class ActionPlans : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();

			rowData.AddRange(new[] {
				Labels.Rule,
				Labels.ViolationsStill,
				Labels.ViolationsNew
			});

			int actionPlanCount = reportData?.CurrentSnapshot?.ActionsPlan?.Count() ?? 0;
            if (actionPlanCount > 0)
            {
                if (reportData?.CurrentSnapshot?.ActionsPlan != null)
                    foreach (var _actionPlan in reportData.CurrentSnapshot?.ActionsPlan) {
                        rowData.AddRange
                        (new[] { _actionPlan.RulePattern.Name
                            , _actionPlan.PendingIssues.ToString("N0")
                            , _actionPlan.AddedIssues.ToString("N0") 
                        });
                    }
            } else {
				rowData.AddRange(new[] {
					Labels.NoItem,
					string.Empty,
					string.Empty
				});
                actionPlanCount = 1;
            }

            TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = actionPlanCount + 1,
                NbColumns = 3,
                Data = rowData
            };
            return resultTable;
        }
    }
}
