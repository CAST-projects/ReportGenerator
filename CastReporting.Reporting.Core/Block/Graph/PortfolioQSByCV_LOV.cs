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
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_QS_BY_CVLOC")]
    public class PortfolioQsbyCvLov : GraphBlock
    {
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int count = 0;

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] {
				Labels.TQI ,
				Labels.ViolationsCritical + "/" + Labels.kLoC ,
				Labels.AutomatedFP,
                Labels.Application
			});

            #region Fetch SnapshotsPF

            if (reportData?.Applications != null && reportData.Snapshots != null)
            {
                Application[] _allApps = reportData.Applications;
                foreach (Application _app in _allApps)
                {
                    Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();

                    BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(_snapshot, false);
                    double? strCurrentTQI = currSnapshotBisCriDTO.TQI ?? 0;
                    double? _numCritPerKloc = MeasureUtility.GetSizingMeasure(_snapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);
                    double? result = MeasureUtility.GetAutomatedIFPUGFunction(_snapshot);

                    rowData.Add(strCurrentTQI.GetValueOrDefault().ToString("N2"));
                    rowData.Add(_numCritPerKloc.GetValueOrDefault().ToString("N2"));
                    rowData.Add(result.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                    rowData.Add(_app.Name);

                    count++;
                }

                if (reportData.Applications.Length == 1)
                {
                    rowData.AddRange(new[] { "0", "0", "0", "" });

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
