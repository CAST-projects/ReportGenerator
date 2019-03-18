
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
using System;
using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("TECHNO_LOC")]
    public class PieTechnoLoC : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbResult = reportData.Parameter.NbResultDefault, tmpNb;
            if (null != options && options.ContainsKey("COUNT") && int.TryParse(options["COUNT"], out tmpNb) && tmpNb > 0)
            {
                nbResult = tmpNb;
            }

            if (reportData.CurrentSnapshot == null) return null;
            List<TechnologyResultDTO> technologyInfos = MeasureUtility.GetTechnoLoc(reportData.CurrentSnapshot, nbResult);

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Name, Labels.LoC });

            foreach (var elt in technologyInfos)
            {
                rowData.Add(elt.Name);
                rowData.Add(Convert.ToInt32(elt.Value).ToString());
            }


            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = technologyInfos.Count + 1,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }
     
    }
}
