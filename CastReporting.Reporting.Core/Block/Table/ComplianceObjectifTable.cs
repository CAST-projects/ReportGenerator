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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("COMPLIANCE_TO_OBJ_TABLE")]
    public class ComplianceObjectifTable : TableBlock
    {

        private const string MetricFormat = "N0";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();           

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);           
                     
            if (reportData?.CurrentSnapshot != null)
            {
                //Compute nb objectives
                int? nbObjectives = RulesViolationUtility.GetNbRuleWithViolations(reportData.CurrentSnapshot, Constants.RulesViolation.CriticalRulesViolation, 0, false);

                //Compute nb acchiveemnt for the whole applcation
                int? nbRuleWithViolations = RulesViolationUtility.GetNbRuleWithViolations(reportData.CurrentSnapshot, Constants.RulesViolation.CriticalRulesViolation, 0, true);
                int? nbAchievement = (nbObjectives.HasValue && nbRuleWithViolations.HasValue) ? (nbObjectives.Value - nbRuleWithViolations.Value) : (int?)null;

                double? achievementRatio = (nbAchievement.HasValue && nbObjectives.Value != 0) ? (double)nbAchievement.Value / nbObjectives.Value : (double?)null;

                //Compute nb acchiveemnt add in the last delivery
                int? nbAddedCriticalViolations = MeasureUtility.GetAddedCriticalViolations(reportData.CurrentSnapshot);
                if (!nbAddedCriticalViolations.HasValue) nbAddedCriticalViolations = 0;
                int? nbAchievementAdded = (nbObjectives.HasValue) ? nbObjectives.Value - nbAddedCriticalViolations.Value : (int?)null;

                double? achievementAddedRatio = (nbAchievementAdded.HasValue && nbObjectives.Value != 0) ? (double)nbAchievementAdded.Value / nbObjectives.Value : (double?)null;

                //BuildContent header
                rowData.AddRange(displayShortHeader ? new[] { " ", Labels.Obj, Labels.Achiev, Labels.AchievRatio }
                                                    : new[] { " ", Labels.Objectives, Labels.Achievement, Labels.AchievementRatio });


                //BuildContent "Entire Application" row
                rowData.AddRange(new[] {
                    Labels.DeliveryWhole,
                    nbObjectives?.ToString(MetricFormat) ?? string.Empty,
                    nbAchievement?.ToString(MetricFormat) ?? string.Empty,
                    FormatPercent(MathUtility.GetRound(achievementRatio), false)
                });


                //BuildContent "Last Delivery" row          
                rowData.AddRange(new[] {
                     Labels.DeliveryLast,
                     nbObjectives?.ToString(MetricFormat) ?? string.Empty,
                     nbAchievementAdded?.ToString(MetricFormat) ?? string.Empty,
                     FormatPercent(MathUtility.GetRound(achievementAddedRatio), false)
                });

            }

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 3,
                NbColumns = 4,
                Data = rowData
            };


            return resultTable;
        }
       
    }
}
