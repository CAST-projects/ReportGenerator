
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
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System.Globalization;

namespace CastReporting.Reporting.Block.Graph
{ 
    [Block("TREND_TECH_DEBT")]
    public class TrendTechDebt : GraphBlock
    {

        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
           
            int count = 0;

            var rowData = new List<string>();
			rowData.AddRange(new[] {
				" ",
				Labels.DebtRemoved + " (" + reportData.CurrencySymbol + ")",
				Labels.DebtAdded + " (" + reportData.CurrencySymbol + ")",
				Labels.Debt + " (" + reportData.CurrencySymbol + ")"
			}); 

            #region Fetch Snapshots
			int nbSnapshots = reportData.Application?.Snapshots.Count() ?? 0;
			if (nbSnapshots > 0)
			{
			    var _snapshots = reportData.Application?.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot);
			    if (_snapshots != null)
			        foreach (Snapshot snapshot in _snapshots)
                    {
			            double? prevDoubleSnapshotDate = snapshot.Annotation.Date.DateSnapShot?.ToOADate() ?? 0;
			            double? prevRemovedTechDebtValue = MeasureUtility.GetRemovedTechDebtMetric(snapshot) * -1;
			            double? prevAddedTechDebtValue = MeasureUtility.GetAddedTechDebtMetric(snapshot);
			            double? prevTotalTechDebtValue = MeasureUtility.GetTechnicalDebtMetric(snapshot);

                        rowData.Add(prevDoubleSnapshotDate.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(prevRemovedTechDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(prevAddedTechDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(prevTotalTechDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
			        }
			    count = nbSnapshots;
			}
            #endregion Previous Snapshots

            #region just 1 snapshot
            if (reportData.Application?.Snapshots != null && nbSnapshots == 1 && reportData.CurrentSnapshot != null)
            {
                double? prevDoubleSnapshotDate = reportData.CurrentSnapshot.Annotation.Date.DateSnapShot?.ToOADate() ?? 0;
                double? prevRemovedTechDebtValue = MeasureUtility.GetRemovedTechDebtMetric(reportData.CurrentSnapshot) * -1;
                double? prevAddedTechDebtValue = MeasureUtility.GetAddedTechDebtMetric(reportData.CurrentSnapshot);
                double? prevTotalTechDebtValue = MeasureUtility.GetTechnicalDebtMetric(reportData.CurrentSnapshot);
                rowData.AddRange(new[] {
                        prevDoubleSnapshotDate.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                        prevRemovedTechDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                        prevAddedTechDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                        prevTotalTechDebtValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
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

