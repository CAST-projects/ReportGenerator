
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
namespace CastReporting.Reporting.Block.Graph
{
	[Block("RADAR_HEALTH_FACTOR_2_LAST_SNAPSHOTS"), Block("RADAR_HEALTH_FACTOR_2_SNAPSHOTS")]
	public class RadarHealthFactor2LastSnapshots : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

            string prevSnapshotLabel = string.Empty;
            BusinessCriteriaDTO prevSnapshotBCResult = null;

            if (reportData?.CurrentSnapshot == null) return null;
            string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
            BusinessCriteriaDTO _currSnapshotBcdto = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot, true);

            if (reportData.PreviousSnapshot != null)
            {
                prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot);
                prevSnapshotBCResult = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot, true);
            }
            else
            {
                Snapshot _previousSnapshot = reportData.Application.Snapshots?.FirstOrDefault(_ => _.Annotation.Date.DateSnapShot < reportData.CurrentSnapshot.Annotation.Date.DateSnapShot);
                if (_previousSnapshot != null)
                {
                    prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(_previousSnapshot);
                    prevSnapshotBCResult = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(_previousSnapshot, true);
                }
            }


            var rowData = new List<string> {null, currSnapshotLabel};
            if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotLabel ?? Constants.No_Value); }


            #region Transferability
            rowData.Add(Labels.Trans);
            rowData.Add(_currSnapshotBcdto.Transferability.ToString());
            if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Transferability.ToString()); }
            #endregion Transferability

            #region Changeability
            rowData.Add(Labels.Chang);
            rowData.Add(_currSnapshotBcdto.Changeability.ToString());
            if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Changeability.ToString()); }
            #endregion Changeability

            #region Robustness
            rowData.Add(Labels.Robu);
            rowData.Add(_currSnapshotBcdto.Robustness.ToString());
            if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Robustness.ToString()); }
            #endregion Robustness

            #region Performance
            rowData.Add(Labels.Efcy);
            rowData.Add(_currSnapshotBcdto.Performance.ToString());
            if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Performance.ToString()); }
            #endregion Performance

            #region Security
            rowData.Add(Labels.Secu);
            rowData.Add(_currSnapshotBcdto.Security.ToString());
            if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Security.ToString()); }              
            #endregion Security

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = true,
                NbRows = 6,
                NbColumns = prevSnapshotBCResult != null ? 3 : 2,
                Data = rowData
            };

            return resultTable;
        }
     
    }

}
