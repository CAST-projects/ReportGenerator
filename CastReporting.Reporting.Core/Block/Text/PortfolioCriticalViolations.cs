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
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Helper;
using Cast.Util.Log;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_CRITICAL_VIOLATIONS")]
    public class PortfolioCriticalViolations : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region Item BCID
            int metricId = options.GetIntOption("BCID", (int)Constants.BusinessCriteria.TechnicalQualityIndex);
            #endregion Item BCID

            if (reportData?.Applications == null || null == reportData.Snapshots) return Constants.No_Value;
            double? _cv = 0;

            Application[] _allApps = reportData.Applications;
            foreach (Application _app in _allApps)
            {
                try
                {
                    Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                    if (_snapshot == null) continue;
                    int? _snapCv = RulesViolationUtility.GetBCEvolutionSummary(_snapshot,metricId).FirstOrDefault()?.TotalCriticalViolations;
                    if (_snapCv != null) _cv = _cv + _snapCv;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogInfo(ex.Message);
                    LogHelper.Instance.LogInfo(Labels.NoSnapshot);
                }
            }
            return _cv.Value.ToString("N0");
        }
        #endregion METHODS
    }
}
