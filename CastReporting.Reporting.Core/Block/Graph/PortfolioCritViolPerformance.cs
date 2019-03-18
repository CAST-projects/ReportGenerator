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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Cast.Util.Date;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_TREND_CRIT_VIOL")]
    public class PortfolioCritViolPerformance : GraphBlock
    {
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int metricId = options.GetIntOption("BCID",60017);

            var rowData = new List<string>();
            rowData.AddRange(new[] {
				" ",
				Labels.ViolationsCritical + " - " + Labels.ViolationsRemoved ,
				Labels.ViolationsCritical + " - " + Labels.ViolationsAdded,
				Labels.ViolationsCritical + " - " + Labels.Total
			});

            DataTable dtDates = new DataTable();
            dtDates.Columns.Add("Quarter", typeof(int));
            dtDates.Columns.Add("Year", typeof(int));
            dtDates.Columns.Add("RemovedCritViol", typeof(double));
            dtDates.Columns.Add("AddedCritViol", typeof(double));
            dtDates.Columns.Add("TotalCritViol", typeof(double));
            dtDates.AcceptChanges();

            #region Fetch SnapshotsPF

            if (reportData?.Applications != null && reportData.Snapshots != null)
            {
                Snapshot[] _allSnapshots = reportData.Snapshots;

                int generateQuater = 6;
                DateTime _dateNow = DateTime.Now;
                int currentYear = _dateNow.Year;
                int currentQuater = DateUtil.GetQuarter(_dateNow);
                for (int i = generateQuater; i > 0; i--)
                {
                    DataRow dr = dtDates.NewRow();
                    dr["Quarter"] = currentQuater;
                    dr["Year"] = currentYear;
                    dtDates.Rows.InsertAt(dr, 0);

                    currentYear = DateUtil.GetPreviousQuarterYear(currentQuater, currentYear);
                    currentQuater = DateUtil.GetPreviousQuarter(currentQuater);
                }

                for (int i = 0; i < dtDates.Rows.Count; i++)
                {
                    double? _removedCritViol = 0;
                    double? _addedCritViol = 0;
                    double? _totalCritViol = 0;

                    if (_allSnapshots.Length > 0)
                    {
                        foreach (Snapshot snapshot in _allSnapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                        {
                            if (snapshot.Annotation.Date.DateSnapShot == null) continue;
                            DateTime _snapshotDate = Convert.ToDateTime(snapshot.Annotation.Date.DateSnapShot.Value);
                            int intQuarter = Convert.ToInt32(dtDates.Rows[i]["Quarter"]);
                            int intYear = Convert.ToInt32(dtDates.Rows[i]["Year"]);

                            int intSnapshotQuarter = DateUtil.GetQuarter(_snapshotDate);
                            int intSnapshotYear = _snapshotDate.Year;

                            if (intQuarter != intSnapshotQuarter || intYear != intSnapshotYear) continue;
                            ViolationsStatisticsDTO results = RulesViolationUtility.GetBCEvolutionSummary(snapshot, metricId).First();
                            if (results == null) continue;
                            _removedCritViol = _removedCritViol + results.RemovedCriticalViolations;
                            _addedCritViol = _addedCritViol + results.AddedCriticalViolations;
                            _totalCritViol = _totalCritViol + results.TotalCriticalViolations;
                        }
                    }

                    dtDates.Rows[i]["RemovedCritViol"] = _removedCritViol * -1;
                    dtDates.Rows[i]["AddedCritViol"] = _addedCritViol;
                    dtDates.Rows[i]["TotalCritViol"] = _totalCritViol;
                }

                for (int i = 0; i < dtDates.Rows.Count; i++)
                {
                    string strQuarter = dtDates.Rows[i]["Year"] + " Q" + dtDates.Rows[i]["Quarter"];
                    rowData.Add(strQuarter);
                    rowData.Add(dtDates.Rows[i]["RemovedCritViol"].ToString());
                    rowData.Add(dtDates.Rows[i]["AddedCritViol"].ToString());
                    rowData.Add(dtDates.Rows[i]["TotalCritViol"].ToString());
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
