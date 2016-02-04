
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
using CastReporting.BLL.Computing.DTO;
using System.Globalization;
using System.Threading;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Graph
{
     
   
    [Block("TREND_TECH_DEBT")]
    class TrendTechDebt : GraphBlock
    {

        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
           
            int count = 0;

            var rowData = new List<String>();
			rowData.AddRange(new string[] {
				" ",
				Labels.DebtRemoved + " (" + reportData.CurrencySymbol + ")",
				Labels.DebtAdded + " (" + reportData.CurrencySymbol + ")",
				Labels.Debt + " (" + reportData.CurrencySymbol + ")"
			}); 

            #region Fetch Snapshots
			int nbSnapshots = (reportData != null && reportData.Application != null) ? reportData.Application.Snapshots.Count() : 0;
			if (nbSnapshots > 0) {
				foreach (Snapshot snapshot in reportData.Application.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot)) {
                    double? prevDoubleSnapshotDate = snapshot.Annotation.Date.DateSnapShot.HasValue ? snapshot.Annotation.Date.DateSnapShot.Value.ToOADate() : 0;
                    double? prevRemovedTechDebtValue = MeasureUtility.GetRemovedTechDebtMetric(snapshot);
                    double? prevAddedTechDebtValue = MeasureUtility.GetAddedTechDebtMetric(snapshot);
                    double? prevTotalTechDebtValue = MeasureUtility.GetTechnicalDebtMetric(snapshot);
                    rowData.AddRange(new string[] {
                                                    prevDoubleSnapshotDate.GetValueOrDefault().ToString(),
                                                    prevRemovedTechDebtValue.GetValueOrDefault().ToString(),
                                                    prevAddedTechDebtValue.GetValueOrDefault().ToString(),
                                                    prevTotalTechDebtValue.GetValueOrDefault().ToString(),
                                                });
                                 
                }
				count = nbSnapshots;
            }
            #endregion Previous Snapshots

            #region just 1 snapshot
            if (reportData.Application != null && 
                reportData.Application.Snapshots != null && 
			             nbSnapshots == 1 &&
			             reportData.CurrentSnapshot != null) {
                double? prevDoubleSnapshotDate = reportData.CurrentSnapshot.Annotation.Date.DateSnapShot.HasValue ? reportData.CurrentSnapshot.Annotation.Date.DateSnapShot.Value.ToOADate() : 0;
                double? prevRemovedTechDebtValue = MeasureUtility.GetRemovedTechDebtMetric(reportData.CurrentSnapshot);
                double? prevAddedTechDebtValue = MeasureUtility.GetAddedTechDebtMetric(reportData.CurrentSnapshot);
                double? prevTotalTechDebtValue = MeasureUtility.GetTechnicalDebtMetric(reportData.CurrentSnapshot);
                rowData.AddRange(new string[] {
                        prevDoubleSnapshotDate.GetValueOrDefault().ToString(),
                        prevRemovedTechDebtValue.GetValueOrDefault().ToString(),
                        prevAddedTechDebtValue.GetValueOrDefault().ToString(),
                        prevTotalTechDebtValue.GetValueOrDefault().ToString(),
                    });
                count = count + 1;
            }
            #endregion just 1 snapshot



            TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = count + 1,
                NbColumns = 4,
                Data = rowData,
                GraphOptions = null
            };
            return resultTable;
        }
       
        #endregion METHODS
 
    }
}

