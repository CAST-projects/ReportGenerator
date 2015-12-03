
/*
 *   Copyright (c) 2015 CAST
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
using CastReporting.BLL.Computing;
using System.Globalization;
using CastReporting.Domain;
namespace CastReporting.Reporting.Block.Graph
{
    [Block("RADAR_HEALTH_FACTOR_2_LAST_SNAPSHOTS")]
    class RadarHealthFactor2LastSnapshots : GraphBlock
    {
        
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

            string prevSnapshotLabel = string.Empty;
            BusinessCriteriaDTO prevSnapshotBCResult = null;

            if (reportData != null && reportData.CurrentSnapshot != null)
            {

                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                BusinessCriteriaDTO currSnapshotBCDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot);

                if (reportData.PreviousSnapshot != null)
                {
                    prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot);
                    prevSnapshotBCResult = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot);
                }
                else
                    if (reportData.Application.Snapshots != null)
                    {
                        Snapshot PreviousSnapshot = reportData.Application.Snapshots.FirstOrDefault(_ => _.Annotation.Date.DateSnapShot < reportData.CurrentSnapshot.Annotation.Date.DateSnapShot);
                        if (PreviousSnapshot != null)
                        {
                            prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(PreviousSnapshot);
                            prevSnapshotBCResult = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(PreviousSnapshot);
                        }

                    }



                var rowData = new List<String>();
                rowData.Add(null);
                rowData.Add(currSnapshotLabel.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotLabel ?? Constants.No_Value); }


                #region Transferability
                rowData.Add("Trsf");
                rowData.Add(currSnapshotBCDTO.Transferability.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Transferability.ToString()); }
                #endregion Transferability

                #region Changeability
                rowData.Add("Chng");
                rowData.Add(currSnapshotBCDTO.Changeability.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Changeability.ToString()); }
                #endregion Changeability

                #region Robustness
                rowData.Add("Rbst");
                rowData.Add(currSnapshotBCDTO.Robustness.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Robustness.ToString()); }
                #endregion Robustness

                #region Performance
                rowData.Add("Effi");
                rowData.Add(currSnapshotBCDTO.Performance.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Performance.ToString()); }
                #endregion Performance

                #region Security
                rowData.Add("Secu");
                rowData.Add(currSnapshotBCDTO.Security.ToString());
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

            return null;
        }
     
    }

}
