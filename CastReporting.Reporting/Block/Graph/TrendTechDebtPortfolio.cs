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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using Cast.Util.Date;
using System.Globalization;
using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_TREND_TECH_DEBT")]
    class TrendTechDebtPortfolio : GraphBlock
    {

        #region METHODS

        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

            var rowData = new List<String>();
            rowData.AddRange(new string[] {
				" ",
				Labels.DebtRemoved + " (" + reportData.CurrencySymbol + ")",
				Labels.DebtAdded + " (" + reportData.CurrencySymbol + ")",
				Labels.Debt + " (" + reportData.CurrencySymbol + ")"
			});

            DataTable dtDates = new DataTable();
            dtDates.Columns.Add("Quarter", typeof(int));
            dtDates.Columns.Add("Year", typeof(int));
            dtDates.Columns.Add("RemovedTechnicalDebt", typeof(double));
            dtDates.Columns.Add("AddedTechnicalDebt", typeof(double));
            dtDates.Columns.Add("TotalTechnicalDebt", typeof(double)); 
            dtDates.AcceptChanges();

            #region Fetch SnapshotsPF

            if (reportData != null && reportData.Applications != null && reportData.snapshots != null)
            {
                DateTime DateNow = DateTime.Now;
                //DateTime DateNow = Convert.ToDateTime("03 01 2014");
                Application[] AllApps = reportData.Applications;
                Snapshot[] AllSnapshots = reportData.snapshots;
                int generateQuater = 6;
                int currentYear = DateNow.Year;
                int currentQuater = DateUtil.GetQuarter(DateNow);
                for (int i = generateQuater; i > 0; i--)
                {
                    DataRow dr = dtDates.NewRow();
                    dr["Quarter"] = currentQuater;
                    dr["Year"] = currentYear;
                    dtDates.Rows.InsertAt(dr, 0);
                    currentQuater = (currentQuater == 1) ? 4 : currentQuater - 1;
                    currentYear = (currentQuater == 4) ? currentYear - 1 : currentYear;
                }

                double? RemovedTechnicalDebt = 0;
                double? AddedTechnicalDebt = 0;
                double? TotalTechnicalDebt = 0;

                for (int i = 0; i < dtDates.Rows.Count; i++)
                {
                    RemovedTechnicalDebt = 0;
                    AddedTechnicalDebt = 0;
                    TotalTechnicalDebt = 0;

                    if (AllSnapshots.Count() > 0)
                    {
                        foreach (Snapshot snapshot in AllSnapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                        {
                            DateTime SnapshotDate = Convert.ToDateTime(snapshot.Annotation.Date.DateSnapShot.Value);

                            int intQuarter = Convert.ToInt32(dtDates.Rows[i]["Quarter"]);
                            int intYear = Convert.ToInt32(dtDates.Rows[i]["Year"]);

                            int intSnapshotQuarter = DateUtil.GetQuarter(SnapshotDate);
                            int intSnapshotYear = SnapshotDate.Year;

                            if (intQuarter == intSnapshotQuarter && intYear == intSnapshotYear)
                            {
                                RemovedTechnicalDebt = RemovedTechnicalDebt + MeasureUtility.GetRemovedTechDebtMetric(snapshot);
                                AddedTechnicalDebt = AddedTechnicalDebt + MeasureUtility.GetAddedTechDebtMetric(snapshot);
                                TotalTechnicalDebt = TotalTechnicalDebt + MeasureUtility.GetTechnicalDebtMetric(snapshot);
                            }

                        }
                    }

                    dtDates.Rows[i]["RemovedTechnicalDebt"] = RemovedTechnicalDebt != null ? RemovedTechnicalDebt * -1 : 0.0;
                    dtDates.Rows[i]["AddedTechnicalDebt"] = AddedTechnicalDebt != null ? AddedTechnicalDebt : 0.0;
                    dtDates.Rows[i]["TotalTechnicalDebt"] = TotalTechnicalDebt != null ? TotalTechnicalDebt : 0.0;

                }

                for (int i = 0; i < dtDates.Rows.Count; i++)
                {
                    string strQuarter = dtDates.Rows[i]["Year"].ToString() + " Q" + dtDates.Rows[i]["Quarter"].ToString();
                    rowData.AddRange(new string[] {
                                                    strQuarter,
                                                    dtDates.Rows[i]["RemovedTechnicalDebt"].ToString(),
                                                    dtDates.Rows[i]["AddedTechnicalDebt"].ToString(),
                                                    dtDates.Rows[i]["TotalTechnicalDebt"].ToString(),
                                                });
                }

            }
            #endregion Fetch SnapshotsPF






            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = dtDates.Rows.Count + 1,
                NbColumns = 4,
                Data = rowData,
                GraphOptions = null
            };
            return resultTable;
        }

        #endregion METHODS
 
    }
}
