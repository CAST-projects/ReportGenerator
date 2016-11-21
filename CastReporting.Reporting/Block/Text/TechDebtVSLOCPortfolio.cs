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
using System.Globalization;
using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Languages;
using Cast.Util.Log;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_TECHDEBT_VS_LOC")]
    class TechDebtVSLOCPortfolio : TextBlock
    {
        #region METHODS
        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (null != reportData && null != reportData.Applications)
            {
                Application[] AllApps = reportData.Applications;
                double? AllTechDebt = 0;
                double? AllLOC = 0;

                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];
                    try
                    {
                        Snapshot _snapshot = App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot != null)
                        {
                            double? result = MeasureUtility.GetTechnicalDebtMetric(_snapshot);
                            if (result != null)
                            {
                                AllTechDebt = AllTechDebt + result;
                            }

                            double? LOCSnap = MeasureUtility.GetCodeLineNumber(_snapshot);
                            if (LOCSnap != null)
                            {
                                AllLOC = AllLOC + LOCSnap;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogInfo(Labels.NoSnapshot);
                    }
                }
                 
                if (AllTechDebt > 0 && AllLOC > 0)
                {
                    double? FinalValue = AllTechDebt / AllLOC;
                    return FinalValue.Value.ToString("C0"); // currency format with no decimal
                }
                return Labels.NoData;
            }
            return CastReporting.Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}
