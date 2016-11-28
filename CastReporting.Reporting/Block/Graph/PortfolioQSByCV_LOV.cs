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
using System.Globalization;
using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_QS_BY_CVLOC")]
    class PortfolioQSByCV_LOV : GraphBlock
    {
        #region METHODS

        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int count = 0;

            List<String> rowData = new List<String>();
            rowData.AddRange(new string[] {
				Labels.TQI ,
				Labels.ViolationsCritical + "/" + Labels.kLoC ,
				Labels.AutomatedFP,
                Labels.Application
			});

            #region Fetch SnapshotsPF

            if (reportData != null && reportData.Applications != null && reportData.snapshots != null)
            {
                Application[] AllApps = reportData.Applications;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];

                    Snapshot _snapshot = App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();

                    BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(_snapshot, false);
                    double? strCurrentTQI = currSnapshotBisCriDTO.TQI.HasValue ? currSnapshotBisCriDTO.TQI.Value : 0;
                    double? numCritPerKLOC = MeasureUtility.GetSizingMeasure(_snapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);
                    double? result = MeasureUtility.GetAutomatedIFPUGFunction(_snapshot);

                    rowData.AddRange(new string[] {
                        strCurrentTQI.GetValueOrDefault().ToString("N2"),
                        numCritPerKLOC.GetValueOrDefault().ToString("N2"),
                        result.GetValueOrDefault().ToString(),
                        App.Name.ToString()
                    });

                    count++;
                }

                if (reportData.Applications.Count() == 1)
                {
                    rowData.AddRange(new string[] { "0", "0", "0", "" });

                    count++;
                }

            }
            #endregion Fetch SnapshotsPF

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = count + 1,
                NbColumns = 4,
                Data = rowData
            };
            return resultTable;
        }

        #endregion METHODS
    }
}
