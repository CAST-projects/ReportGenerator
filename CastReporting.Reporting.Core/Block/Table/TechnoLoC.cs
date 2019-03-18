
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
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNO_LOC")]
    public class TechnoLoC : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS
            int _intLocFlag = 0;
            int nbResult = reportData.Parameter.NbResultDefault;
            int nbTot = 0;
            int nb;
            if (null != options && options.ContainsKey("COUNT") && int.TryParse(options["COUNT"], out nb) && 0 < nb)
            {
                nbResult = nb;
            }

            if (null != options && options.ContainsKey("NOSIZE"))
            {
                _intLocFlag = 1;
            }

            List<string> rowData = new List<string>();

            rowData.AddRange(_intLocFlag == 1 ? new[] {Labels.Name} : new[] {Labels.Name, Labels.LoC});
            if (reportData.CurrentSnapshot?.Technologies != null)
            {
                var technologyInfos = MeasureUtility.GetTechnoLoc(reportData.CurrentSnapshot, nbResult);

                foreach (var elt in technologyInfos)
                {
                    rowData.AddRange(_intLocFlag == 1 ? new[] {elt.Name} : new[] {elt.Name, elt.Value?.ToString("N0")});
                }
                nbTot = technologyInfos.Count;
            }
            TableDefinition resultTable;
            if (_intLocFlag == 1)
            {
                resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbTot + 1,
                    NbColumns = 1,
                    Data = rowData
                };
            }
            else
            {
                resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbTot + 1,
                    NbColumns = 2,
                    Data = rowData
                };
            }
            return resultTable;
        }
            #endregion METHODS
    }
}
