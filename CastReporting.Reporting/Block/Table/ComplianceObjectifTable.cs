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
using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System;
using CastReporting.Reporting.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("COMPLIANCE_TO_OBJ_TABLE")]
    public class ComplianceObjectifTable : TableBlock
    {

        private const string _MetricFormat = "N0";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop = 0;
            List<string> rowData = new List<string>();           

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);           
                     
            if (options == null || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            if (null != reportData && null != reportData.CurrentSnapshot)
            {
                //Compute nb objectives
                Int32? nbObjectives = RulesViolationUtility.GetNbRuleWithViolations(reportData.CurrentSnapshot, Constants.RulesViolation.CriticalRulesViolation, 0, false);

                //Compute nb acchiveemnt for the whole applcation
                Int32? nbRuleWithViolations = RulesViolationUtility.GetNbRuleWithViolations(reportData.CurrentSnapshot, Constants.RulesViolation.CriticalRulesViolation, 0, true);
                Int32? nbAchievement = (nbObjectives.HasValue && nbRuleWithViolations.HasValue) ? (nbObjectives.Value - nbRuleWithViolations.Value) : (Int32?)null;

                Double? achievementRatio = (nbAchievement.HasValue && nbObjectives.HasValue && nbObjectives.Value != 0) ? (Double)nbAchievement.Value / nbObjectives.Value : (Double?)null;

                //Compute nb acchiveemnt add in the last delivery
                Int32? nbAddedCriticalViolations = MeasureUtility.GetAddedCriticalViolations(reportData.CurrentSnapshot);
                if (!nbAddedCriticalViolations.HasValue) nbAddedCriticalViolations = 0;
                Int32? nbAchievementAdded = (nbObjectives.HasValue && nbAddedCriticalViolations.HasValue) ? nbObjectives.Value - nbAddedCriticalViolations.Value : (Int32?)null;

                Double? achievementAddedRatio = (nbAchievementAdded.HasValue && nbObjectives.HasValue && nbObjectives.Value != 0) ? (Double)nbAchievementAdded.Value / nbObjectives.Value : (Double?)null;

                //BuildContent header
                rowData.AddRange(displayShortHeader ? new[] { " ", Labels.Obj, Labels.Achiev, Labels.AchievRatio }
                                                    : new[] { " ", Labels.Objectives, Labels.Achievement, Labels.AchievementRatio });


                //BuildContent "Entire Application" row
                rowData.AddRange(new string[] {
                    Labels.DeliveryWhole,
                    (nbObjectives.HasValue)?nbObjectives.Value.ToString(_MetricFormat):String.Empty,
                    (nbAchievement.HasValue)?nbAchievement.Value.ToString(_MetricFormat):String.Empty,
                    TableBlock.FormatPercent(MathUtility.GetRound(achievementRatio), false)
                });


                //BuildContent "Last Delivery" row          
                rowData.AddRange(new string[] {
                     Labels.DeliveryLast,
                     (nbObjectives.HasValue)?nbObjectives.Value.ToString(_MetricFormat):String.Empty,
                     (nbAchievementAdded.HasValue)?nbAchievementAdded.Value.ToString(_MetricFormat):String.Empty,
                     TableBlock.FormatPercent(MathUtility.GetRound(achievementAddedRatio), false)
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
