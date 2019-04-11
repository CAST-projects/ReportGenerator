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
using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TOP_COMPONENTS_BY_PROPERTIES")]
    public class TopComponentsByProperties : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string prop1 = options.GetOption("PROP1","cyclomaticComplexity");
            string prop2 = options.GetOption("PROP2", "cyclomaticComplexity");
            string order1 = options.GetOption("ORDER1", "desc");
            string order2 = options.GetOption("ORDER2", "desc");
            int nbLimitTop = options.GetIntOption("COUNT", 50);

            if (!order1.ToLower().Equals("asc") && !order1.ToLower().Equals("desc")) order1 = "desc";
            if (!order2.ToLower().Equals("asc") && !order2.ToLower().Equals("desc")) order2 = "desc";

            if (nbLimitTop == -1) nbLimitTop = 50;

            List<string> rowData = new List<string> {Labels.ObjectName};

            if (!VersionUtil.Is18Compatible(reportData.ServerVersion))
            {
                LogHelper.Instance.LogError("Bad version of RestAPI. Should be 1.8 at least for component TOP_COMPONENTS_BY_PROPERTIES");
                rowData.Add(Labels.NoData);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 1,
                    Data = rowData
                };
            }

            string _prop1Name = MetricsUtility.GetPropertyName(prop1);
            rowData.Add(_prop1Name);
            string _prop2Name = MetricsUtility.GetPropertyName(prop2);
            rowData.Add(_prop2Name);

            if (_prop1Name == string.Empty || _prop2Name == string.Empty)
            {
                rowData.AddRange(new List<string>() { Labels.PropertiesNotAvailable, string.Empty, string.Empty });
                rowData.AddRange(new List<string>()
                {
                    "codeLines", string.Empty, string.Empty,
                    "commentedCodeLines", string.Empty, string.Empty,
                    "commentLines", string.Empty, string.Empty,
                    "coupling", string.Empty, string.Empty,
                    "fanIn", string.Empty, string.Empty,
                    "fanOut", string.Empty, string.Empty,
                    "cyclomaticComplexity", string.Empty, string.Empty,
                    "ratioCommentLinesCodeLines", string.Empty, string.Empty,
                    "halsteadProgramLength", string.Empty, string.Empty,
                    "halsteadProgramVocabulary", string.Empty, string.Empty,
                    "halsteadVolume", string.Empty, string.Empty,
                    "distinctOperators", string.Empty, string.Empty,
                    "distinctOperands", string.Empty, string.Empty,
                    "integrationComplexity", string.Empty, string.Empty,
                    "essentialComplexity", string.Empty, string.Empty
                });
                nbLimitTop = 16;
            }
            else
            {
                if (SnapshotUtility.IsLatestSnapshot(reportData.Application, reportData.CurrentSnapshot))
                {
                    List<ComponentWithProperties> components = reportData.SnapshotExplorer.GetComponentsByProperties(reportData.CurrentSnapshot.Href, 60017, prop1, prop2, order1, order2, nbLimitTop).ToList();
                    if (components.Count <= 0)
                    {
                        rowData.AddRange(new List<string> { Labels.NoData, string.Empty, string.Empty });
                    }
                    else
                    {
                        foreach (ComponentWithProperties _component in components)
                        {
                            rowData.Add(_component.Name);
                            rowData.Add(_component.GetPropertyValueString(prop1));
                            rowData.Add(_component.GetPropertyValueString(prop2));
                        }
                    }
                }
                else
                {
                    rowData.AddRange(new List<string>() { Labels.SnapshotNotTheLatestOne, string.Empty, string.Empty });
                }
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbLimitTop + 1,
                NbColumns = 3,
                Data = rowData
            };
            return resultTable;
        }
    }
}
