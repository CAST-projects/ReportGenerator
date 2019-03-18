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

namespace CastReporting.Reporting.Block.Table
{
    [Block("CRITICAL_VIOL_BY_MODULE")]
    public class CriticalViolationByModule : TableBlock
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
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex]?.TotalCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value,

                          resultModule[Constants.BusinessCriteria.Robustness]?.TotalCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Performance]?.TotalCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Security]?.TotalCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Transferability]?.TotalCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Changeability]?.TotalCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                        });

               
            }
            rowData.AddRange(new[] { Labels.ViolationsAdded, " ", " ", " ", " ", " ", " " });

            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
                rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex]?.AddedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value,

                          resultModule[Constants.BusinessCriteria.Robustness]?.AddedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Performance]?.AddedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Security]?.AddedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Transferability]?.AddedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Changeability]?.AddedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                        });

            }
            rowData.AddRange(new[] { Labels.ViolationsRemoved, " ", " ", " ", " ", " ", " " });

            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
                rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex]?.RemovedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value,

                          resultModule[Constants.BusinessCriteria.Robustness]?.RemovedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Performance]?.RemovedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Security]?.RemovedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Transferability]?.RemovedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

                          resultModule[Constants.BusinessCriteria.Changeability]?.RemovedCriticalViolations?.ToString(MetricFormat) ?? Constants.No_Value ,

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
