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
                double? resultAllTechDebt = 0;
                double? AFPAll = 0; 

                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];

                    int nbSnapshotsEachApp = App.Snapshots.Count();
                    if (nbSnapshotsEachApp > 0)
                    {
                        foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                        {
                            double? result = MeasureUtility.GetTechnicalDebtMetric(snapshot);
                            resultAllTechDebt = resultAllTechDebt + result;
                            break;
                        }
                    }
                }

                for (int j = 0; j < AllApps.Count(); j++)
                {
                    int nbResult = reportData.Parameter.NbResultDefault;
                    Application App = AllApps[j];

                    int nbSnapshotsEachApp = App.Snapshots.Count();
                    if (nbSnapshotsEachApp > 0)
                    {
                        foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                        {
                            var technologyInfos = MeasureUtility.GetTechnoLoc(snapshot, nbResult);
                            double? LOCSnap = 0;

                            foreach (var elt in technologyInfos)
                            {
                                LOCSnap = LOCSnap + elt.Value;
                            }


                            AFPAll = AFPAll + LOCSnap;
                            break;
                        }
                    }
                }
                //handle 0 functions case
                if (resultAllTechDebt > 0 && AFPAll > 0)
                {

                    double? FinalValue = (resultAllTechDebt / AllApps.Count()) / (AFPAll / AllApps.Count());
                    int intFinalValue = Convert.ToInt32(FinalValue);
                    return string.Format("{0:n0}", intFinalValue) + "%";
                }
                else
                {
                    return "NA";
                }
                //return (result.HasValue ? String.Format("{0:N0} {1}", result.Value, reportData.CurrencySymbol) : CastReporting.Domain.Constants.No_Value);
            }
            return CastReporting.Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}
