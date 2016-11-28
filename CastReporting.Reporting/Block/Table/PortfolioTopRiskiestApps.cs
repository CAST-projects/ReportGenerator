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
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_TOP_RISKIEST_APPS")]
    class PortfolioTopRiskiestApps : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;

            int metricId = options.GetIntOption("ALT", (Int32)Constants.BusinessCriteria.TechnicalQualityIndex);
            int nbLimitTop = options.GetIntOption("COUNT", reportData.Parameter.NbResultDefault);

            List<string> rowData = new List<string>();
            int nbRows = 0;

            if (reportData.Applications != null && reportData.snapshots != null)
            {
                switch (metricId)
                {
                    case 60012:
                        rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Changeability, Labels.SnapshotDate });
                        break;
                    case 60014:
                        rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Efficiency, Labels.SnapshotDate });
                        break;
                    case 60013:
                        rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Robustness, Labels.SnapshotDate });
                        break;
                    case 60016:
                        rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Security, Labels.SnapshotDate });
                        break;
                    case 60011:
                        rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.Transferability, Labels.SnapshotDate });
                        break;
                    case 60017:
                    default:
                        rowData.AddRange(new string[] { Labels.Application, Labels.ViolationsCritical, Labels.TQI, Labels.SnapshotDate });
                        break;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("AppName", typeof(string));
                dt.Columns.Add("CV", typeof(double));
                dt.Columns.Add("BizCritScore", typeof(double));
                dt.Columns.Add("LastAnalysis", typeof(string));


                Application[] AllApps = reportData.Applications;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];
                    try
                    {
                        Snapshot _snapshot = App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot != null)
                        {
                            string strAppName = App.Name;
                            double? CV = RulesViolationUtility.GetBCEvolutionSummary(_snapshot, metricId).FirstOrDefault().TotalCriticalViolations;

                            double? strCurrentBCGrade = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(_snapshot, (Constants.BusinessCriteria)metricId, false);

                            string strLastAnalysis = Convert.ToDateTime(_snapshot.Annotation.Date.DateSnapShot.Value).ToString("MMM dd yyyy");

                            dt.Rows.Add(strAppName, CV, strCurrentBCGrade, strLastAnalysis);
                            nbRows++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                        LogHelper.Instance.LogInfo(Labels.NoSnapshot);
                    }

                }
                DataView dv = dt.DefaultView;
                dv.Sort = "BizCritScore";
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
                            , string.Format("{0:0.00}", Convert.ToDouble(dtSorted.Rows[i]["BizCritScore"]))
                            , dtSorted.Rows[i]["LastAnalysis"].ToString()
                        });
                }

                resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbLimitTop + 1,
                    NbColumns = 4,
                    Data = rowData
                };
            }
            return resultTable;
        }
    }
}
