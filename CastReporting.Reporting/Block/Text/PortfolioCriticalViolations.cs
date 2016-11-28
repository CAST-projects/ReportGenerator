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
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Helper;
using Cast.Util.Log;
using CastReporting.Reporting.Languages;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_CRITICAL_VIOLATIONS")]
    class PortfolioCriticalViolations : TextBlock
    {
        #region METHODS
        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region Item BCID
            int metricId = options.GetIntOption("BCID", (Int32)Constants.BusinessCriteria.TechnicalQualityIndex);
            #endregion Item BCID

            if (null != reportData && null != reportData.Applications && null != reportData.snapshots)
            {
                double? CV = 0;

                Application[] AllApps = reportData.Applications;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];

                    try
                    {
                        Snapshot _snapshot = App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot != null)
                        {
                            int? snapCV = RulesViolationUtility.GetBCEvolutionSummary(_snapshot,metricId).FirstOrDefault().TotalCriticalViolations;
                            if (snapCV != null)
                            {
                                CV = CV + snapCV;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                        LogHelper.Instance.LogInfo(Labels.NoSnapshot);
                    }
                }
                return CV.Value.ToString("N0");
            }
            return CastReporting.Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}
