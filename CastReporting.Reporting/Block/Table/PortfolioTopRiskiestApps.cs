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
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System.Data;
using Cast.Util.Log;

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_TOP_RISKIEST_APPS")]
    class PortfolioTopRiskiestApps : TableBlock
    {
        private const string _MetricFormat = "N0";
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        { 
            TableDefinition resultTable = null;
            int nbLimitTop = 0;
            string dataSource = string.Empty;

            int metricId;
            #region Item ALT
            if (options == null ||
                !options.ContainsKey("ALT") ||
                !int.TryParse(options["ALT"], out metricId))
            {
                metricId = reportData.Parameter.NbResultDefault;
            }
            #endregion Item ALT

            #region Item Count
            if (options == null ||
                !options.ContainsKey("COUNT") ||
                !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }
            #endregion Item Count

            List<string> rowData = new List<string>();
            int nbRows = 0;

            if (reportData.Applications != null && reportData.snapshots != null)
            {
                if (metricId == 60012)
                {
                    rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Changeability, Labels.SnapshotDate });
                }
                else if (metricId == 60014)
                {
                    rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Efficiency, Labels.SnapshotDate });
                }
                else if (metricId == 60013)
                {
                    rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Robustness, Labels.SnapshotDate });
                }
                else if (metricId == 60016)
                {
                    rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Security, Labels.SnapshotDate });
                }
                else if (metricId == 60017)
                {
                    rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.TQI, Labels.SnapshotDate });
                }
                else if (metricId == 60011)
                {
                    rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Transferability, Labels.SnapshotDate });
                }



                DataTable dt = new DataTable();
                dt.Columns.Add("AppName", typeof(string));
                dt.Columns.Add("CV", typeof(double));
                dt.Columns.Add("Efficiency", typeof(double));
                dt.Columns.Add("LastAnalysis", typeof(string));


                Application[] AllApps = reportData.Applications;
                foreach (Application _app in AllApps)
                {
                    try
                    {
                        Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot == null) continue;
                        string strAppName = _app.Name;

                        ViolationSummaryDTO result = RulesViolationUtility.GetBCEvolutionSummary(_snapshot, metricId).FirstOrDefault();
                        double? _cv = result != null ? result.Total : 0;

                        double? strCurrentBCGrade = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(_snapshot, (Constants.BusinessCriteria)metricId, false);

                        string strLastAnalysis = Convert.ToDateTime(_snapshot.Annotation.Date.DateSnapShot.Value).ToString("MMM dd yyyy");

                        dt.Rows.Add(strAppName, _cv, strCurrentBCGrade, strLastAnalysis);
                        nbRows++;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                    }
                }

                DataView dv = dt.DefaultView;
                dv.Sort = "Efficiency";
                DataTable dtSorted = dv.ToTable();

                if (nbRows < nbLimitTop)
                {
                    nbLimitTop = nbRows;
                }

                for (int i = 0; i < nbLimitTop; i++)
                { 
                    rowData.AddRange
        (new string[] { 
                          dtSorted.Rows[i]["AppName"].ToString()
                        , dtSorted.Rows[i]["CV"].ToString()
                        , string.Format("{0:0.00}", Convert.ToDouble(dtSorted.Rows[i]["Efficiency"]))
                        , dtSorted.Rows[i]["LastAnalysis"].ToString()
                        });
                }


            }

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbLimitTop + 1,
                NbColumns = 4,
                Data = rowData
            };

            return resultTable;
        }
    }
}
