
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
using System.Globalization;
using CastReporting.Domain;
namespace CastReporting.Reporting.Block.Graph
{
    [Block("RADAR_COMPLIANCE_2_LAST_SNAPSHOTS")]
    class RadarCompliance2LastSnapshots : GraphBlock
    {
      
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string prevSnapshotLabel=string.Empty;
            BusinessCriteriaDTO prevSnapshotBCResult=null;
            
            if (reportData !=null && reportData.CurrentSnapshot != null)
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
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotLabel??Constants.No_Value); }

                
                #region Programming Practices
                rowData.Add(Labels.Prog);
                rowData.Add(currSnapshotBCDTO.ProgrammingPractices.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.ProgrammingPractices.ToString()); }
                #endregion Programming Practices

                #region Architectural Design
                rowData.Add(Labels.Arch);
                rowData.Add(currSnapshotBCDTO.ArchitecturalDesign.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.ArchitecturalDesign.ToString()); }
                #endregion Architectural Design

                #region Documentation
                rowData.Add(Labels.Doc);
                rowData.Add(currSnapshotBCDTO.Documentation.ToString());
                if (prevSnapshotBCResult != null) { rowData.Add(prevSnapshotBCResult.Documentation.ToString()); }
                #endregion Documentation


                TableDefinition resultTable = new TableDefinition
                {
                    HasRowHeaders = true,
                    HasColumnHeaders = true,
                    NbRows = 4,
                    NbColumns = (prevSnapshotBCResult != null) ? 3 : 2,
                    Data = rowData
                };

                 return resultTable;
            }
           
            return null;
        }
       

        
    }
}
