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
                    rowData.AddRange(new string[] { "Applications", Labels.ViolationsCritical, Labels.Changeability, "Last Analysis" });
                }
                else if (metricId == 60014)
                {
                    rowData.AddRange(new string[] { "Applications", Labels.ViolationsCritical, "Performance", "Last Analysis" });
                }
                else if (metricId == 60013)
                {
                    rowData.AddRange(new string[] { "Applications", Labels.ViolationsCritical, Labels.Robustness, "Last Analysis" });
                }
                else if (metricId == 60016)
                {
                    rowData.AddRange(new string[] { "Applications", Labels.ViolationsCritical, Labels.Security, "Last Analysis" });
                }
                else if (metricId == 60017)
                {
                    rowData.AddRange(new string[] { "Applications", Labels.ViolationsCritical, Labels.TQI, "Last Analysis" });
                }
                else if (metricId == 60011)
                {
                    rowData.AddRange(new string[] { "Applications", Labels.ViolationsCritical, Labels.Transferability, "Last Analysis" });
                }



                DataTable dt = new DataTable();
                dt.Columns.Add("AppName", typeof(string));
                dt.Columns.Add("CV", typeof(double));
                dt.Columns.Add("Efficiency", typeof(double));
                dt.Columns.Add("LastAnalysis", typeof(string));


                Application[] AllApps = reportData.Applications;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];

                    int nbSnapshotsEachApp = App.Snapshots.Count();
                    if (nbSnapshotsEachApp > 0)
                    {
                        foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                        {
                            Snapshot[] BuiltSnapshots = reportData.snapshots;

                            foreach (Snapshot BuiltSnapshot in BuiltSnapshots)
                            {
                                if (snapshot == BuiltSnapshot)
                                {
                                    string strAppName = App.Name;
                                    double? CV = 0;
                                    var results = RulesViolationUtility.GetStatViolation(BuiltSnapshot);
                                    foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
                                   {
                                       CV = CV + ((resultModule != null && resultModule[(Constants.BusinessCriteria)metricId].Total.HasValue) ?
                         resultModule[(Constants.BusinessCriteria)metricId].Total.Value : 0);

                                    }

                                    string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(BuiltSnapshot);
                                    BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(BuiltSnapshot, false);

                                    double? strCurrentEfficiency = 0;
                                     

                                    if (metricId == 60012)
                                    {
                                        strCurrentEfficiency = currSnapshotBisCriDTO.Changeability.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Changeability.Value) : 0;
                                    }
                                    else if (metricId == 60014)
                                    {
                                        strCurrentEfficiency = currSnapshotBisCriDTO.Performance.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Performance.Value) : 0;
                                    }
                                    else if (metricId == 60013)
                                    {
                                        strCurrentEfficiency = currSnapshotBisCriDTO.Robustness.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Robustness.Value) : 0;
                                    }
                                    else if (metricId == 60016)
                                    {
                                        strCurrentEfficiency = currSnapshotBisCriDTO.Security.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Security.Value) : 0;
                                    }
                                    else if (metricId == 60017)
                                    {
                                        strCurrentEfficiency = currSnapshotBisCriDTO.TQI.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.TQI.Value) : 0;
                                    }
                                    else if (metricId == 60011)
                                    {
                                        strCurrentEfficiency = currSnapshotBisCriDTO.Transferability.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Transferability.Value) : 0;
                                    }

                                    string strLastAnalysis = Convert.ToDateTime(BuiltSnapshot.Annotation.Date.DateSnapShot.Value).ToString("MMM dd yyyy");

                                    dt.Rows.Add(strAppName, CV, strCurrentEfficiency, strLastAnalysis);


                        //            rowData.AddRange
                        //(new string[] { strAppName.ToString()
                        //    , CV.ToString()
                        //    , strCurrentEfficiency.GetValueOrDefault().ToString()
                        //    , strLastAnalysis.ToString()
                        //    });

                                    nbRows++;

                                    break;
                                }
                            }
                            break;
                        }
                    }
                    continue;
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
        (new string[] { dtSorted.Rows[i]["AppName"].ToString()
                        , dtSorted.Rows[i]["CV"].ToString()
                        , dtSorted.Rows[i]["Efficiency"].ToString()
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
