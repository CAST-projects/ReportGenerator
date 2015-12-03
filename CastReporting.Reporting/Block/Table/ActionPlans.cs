
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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;


namespace CastReporting.Reporting.Block.Table
{
    [Block("ACTION_PLANS")]
    class ActionPlans : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int actionPlanCount = 0;
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { "Rule", "Still violation (#)", "New violation (#)" });

            if (reportData != null
                && reportData.CurrentSnapshot != null
                && reportData.CurrentSnapshot.ActionsPlan != null
                && reportData.CurrentSnapshot.ActionsPlan.Count() > 0)
            {
                foreach (var ActionPlan in reportData.CurrentSnapshot.ActionsPlan)
                {
                    rowData.AddRange
                        (new string[]
                            { ActionPlan.RulePattern.Name
                            , ActionPlan.PendingIssues.ToString("N0")
                            , ActionPlan.AddedIssues.ToString("N0") 
                            }
                        );
                }
                actionPlanCount = reportData.CurrentSnapshot.ActionsPlan.Count();
            }
            else
            {
                rowData.AddRange(new string[] { "No enable item.", string.Empty, string.Empty });
                actionPlanCount = 1;
            }

            TableDefinition resultTable = new TableDefinition
            {
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
