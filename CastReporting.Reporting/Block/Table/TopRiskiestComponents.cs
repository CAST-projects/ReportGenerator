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
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TOP_RISKIEST_COMPONENTS")]
    class TopRiskiestComponents : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            const string metricFormat = "N0";
            TableDefinition resultTable = null;
            int nbLimitTop = 0;
            string dataSource = string.Empty;
            int moduleId = -1;
            IEnumerable<Component> components = null;

            #region Business Criteria
            if (options != null &&
                options.ContainsKey("SRC"))
            {
                dataSource = options["SRC"];
            }
            #endregion Business Criteria

            #region Module Id
            if (options != null &&
                options.ContainsKey("MOD") &&
                !int.TryParse(options["MOD"], out moduleId))
            {
                moduleId = -1;
            }
            #endregion Module Id

            #region Item Count
            if (options == null ||
                !options.ContainsKey("COUNT") ||
                !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }
            #endregion Item Count

            List<string> rowData = new List<string>();
            int nbRows = 1;

            if (reportData.CurrentSnapshot != null)
            {               
                rowData.AddRange(new string[] { "Artefact Name", "PRI" });
                Constants.BusinessCriteria bizCrit;
                switch (dataSource)
                {
                    case "ARCH": { bizCrit = Constants.BusinessCriteria.ArchitecturalDesign; } break;
                    case "CHAN": { bizCrit = Constants.BusinessCriteria.Changeability; } break;
                    case "DOC": { bizCrit = Constants.BusinessCriteria.Documentation; } break;
                    case "PERF": { bizCrit = Constants.BusinessCriteria.Performance; } break;
                    case "PROG": { bizCrit = Constants.BusinessCriteria.ProgrammingPractices; } break;
                    case "ROB": { bizCrit = Constants.BusinessCriteria.Robustness; } break;
                    case "SEC": { bizCrit = Constants.BusinessCriteria.Security; } break;
                    case "MAIN": { bizCrit = Constants.BusinessCriteria.SEIMaintainability; } break;
                    case "TQI": { bizCrit = Constants.BusinessCriteria.TechnicalQualityIndex; } break;
                    case "TRAN":
                    default: { bizCrit = Constants.BusinessCriteria.Transferability; } break;
                }

                
                if (moduleId > 0)
                {
                    components = reportData.SnapshotExplorer.GetComponentsByModule(reportData.CurrentSnapshot.DomainId, moduleId, (int)reportData.CurrentSnapshot.Id, ((int)bizCrit).ToString(), nbLimitTop);
                }
                else
                {
                    components = reportData.SnapshotExplorer.GetComponents(reportData.CurrentSnapshot.Href, ((int)bizCrit).ToString(), nbLimitTop);
                }              
            }


            if (components != null && components.Count() > 0)
            {
                foreach (var component in components)
                {
                    rowData.AddRange(new string[] { component.Name, component.PropagationRiskIndex.ToString(metricFormat) });
                    nbRows++;
                }
            }
            else
            {
                rowData.AddRange(new string[] { "No enable item.", string.Empty });
            }

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }
    }
}
