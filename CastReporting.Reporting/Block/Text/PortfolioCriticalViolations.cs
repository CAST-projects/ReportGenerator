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
using Cast.Util.Log;
using System.Globalization;
using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_CRITICAL_VIOLATIONS")]
    class PortfolioCriticalViolations : TextBlock
    {
        #region METHODS
        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            int metricId;
            #region Item BCID
            if (options == null ||
                !options.ContainsKey("BCID") ||
                !int.TryParse(options["BCID"], out metricId))
            {
                metricId = 0;
            }
            #endregion Item BCID

            if (null != reportData && null != reportData.Applications && null != reportData.snapshots)
            {
                double? CV = 0;

                Application[] AllApps = reportData.Applications;
                foreach (Application _app in AllApps)
                {
                    try
                    {
                        Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot == null) continue;
                        ViolationSummaryDTO result = RulesViolationUtility.GetBCEvolutionSummary(_snapshot,metricId).FirstOrDefault();
                        int? _snapCv = result != null ? result.Total : 0;
                        CV = _snapCv != null ? CV + _snapCv : CV;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                    }
                }

                //return string.Format("{0:n0}", intFinalValue) + "%";
                //return Convert.ToInt32(rulesViol).ToString(); 
                return string.Format("{0:n0}", Convert.ToInt32(CV));
            }
            return CastReporting.Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}
