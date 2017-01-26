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
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("CRITICAL_VIOL_BY_MODULE")]
    internal class CriticalViolationByModule : TableBlock
    {
        private const string MetricFormat = "N0";

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            var results = RulesViolationUtility.GetStatViolation(reportData.CurrentSnapshot);
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { " ", Labels.TQI, Labels.Robu, Labels.Efcy, Labels.Secu, Labels.Trans, Labels.Chang });

            rowData.AddRange(new[] { Labels.Current, " ", " ", " ", " ", " ", " " });
          
            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
               rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          (resultModule[Constants.BusinessCriteria.TechnicalQualityIndex]?.TotalCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].TotalCriticalViolations.Value.ToString(MetricFormat) : Constants.No_Value,

                          (resultModule[Constants.BusinessCriteria.Robustness]?.TotalCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Robustness].TotalCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Performance]?.TotalCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Performance].TotalCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Security]?.TotalCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Security].TotalCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Transferability]?.TotalCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Transferability].TotalCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Changeability]?.TotalCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Changeability].TotalCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                        });

               
            }
            rowData.AddRange(new[] { Labels.ViolationsAdded, " ", " ", " ", " ", " ", " " });

            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
                rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          (resultModule[Constants.BusinessCriteria.TechnicalQualityIndex]?.AddedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].AddedCriticalViolations.Value.ToString(MetricFormat) : Constants.No_Value,

                          (resultModule[Constants.BusinessCriteria.Robustness]?.AddedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Robustness].AddedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Performance]?.AddedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Performance].AddedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Security]?.AddedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Security].AddedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Transferability]?.AddedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Transferability].AddedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Changeability]?.AddedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Changeability].AddedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                        });

            }
            rowData.AddRange(new[] { Labels.ViolationsRemoved, " ", " ", " ", " ", " ", " " });

            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
                rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          (resultModule[Constants.BusinessCriteria.TechnicalQualityIndex]?.RemovedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].RemovedCriticalViolations.Value.ToString(MetricFormat) : Constants.No_Value,

                          (resultModule[Constants.BusinessCriteria.Robustness]?.RemovedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Robustness].RemovedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Performance]?.RemovedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Performance].RemovedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Security]?.RemovedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Security].RemovedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Transferability]?.RemovedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Transferability].RemovedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                          (resultModule[Constants.BusinessCriteria.Changeability]?.RemovedCriticalViolations != null)?
                          resultModule[Constants.BusinessCriteria.Changeability].RemovedCriticalViolations.Value.ToString(MetricFormat):Constants.No_Value ,

                        });



            }
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = results.Count,
                NbColumns = 7,
                Data = rowData
            };
            
            return resultTable;
        }
    }
}
