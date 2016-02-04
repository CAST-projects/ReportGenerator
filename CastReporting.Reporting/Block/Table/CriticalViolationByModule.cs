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
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("CRITICAL_VIOL_BY_MODULE")]
    class CriticalViolationByModule : TableBlock
    {
        private const string _MetricFormat = "N0";

        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            var results = RulesViolationUtility.GetStatViolation(reportData.CurrentSnapshot);
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { " ", Labels.TQI, Labels.Robu, Labels.Efcy, Labels.Secu, Labels.Trans, Labels.Chang });

            rowData.AddRange(new[] { Labels.Current, " ", " ", " ", " ", " ", " " });
          
            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
               rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          (resultModule !=null && resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].Total.HasValue)?
                          resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].Total.Value.ToString(_MetricFormat) : Constants.No_Value,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Robustness].Total.HasValue)?
                          resultModule[Constants.BusinessCriteria.Robustness].Total.Value.ToString(_MetricFormat):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Performance].Total.HasValue)?
                          resultModule[Constants.BusinessCriteria.Performance].Total.Value.ToString(_MetricFormat):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Security].Total.HasValue)?
                          resultModule[Constants.BusinessCriteria.Security].Total.Value.ToString(_MetricFormat):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Transferability].Total.HasValue)?
                          resultModule[Constants.BusinessCriteria.Transferability].Total.Value.ToString(_MetricFormat):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Changeability].Total.HasValue)?
                          resultModule[Constants.BusinessCriteria.Changeability].Total.Value.ToString(_MetricFormat):Constants.No_Value ,

                        });

               
            }
            rowData.AddRange(new[] { Labels.ViolationsAdded, " ", " ", " ", " ", " ", " " });

            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
                rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          (resultModule !=null && resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].Added.HasValue)?
                          TableBlock.FormatEvolution(resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].Added.Value) : Constants.No_Value,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Robustness].Added.HasValue)?
                          TableBlock.FormatEvolution(resultModule[Constants.BusinessCriteria.Robustness].Added.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Performance].Added.HasValue)?
                          TableBlock.FormatEvolution(resultModule[Constants.BusinessCriteria.Performance].Added.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Security].Added.HasValue)?
                          TableBlock.FormatEvolution(resultModule[Constants.BusinessCriteria.Security].Added.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Transferability].Added.HasValue)?
                          TableBlock.FormatEvolution(resultModule[Constants.BusinessCriteria.Transferability].Added.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Changeability].Added.HasValue)?
                          TableBlock.FormatEvolution(resultModule[Constants.BusinessCriteria.Changeability].Added.Value):Constants.No_Value ,

                        });

            }
            rowData.AddRange(new[] { Labels.ViolationsRemoved, " ", " ", " ", " ", " ", " " });

            foreach (var resultModule in results.OrderBy(_ => _.ModuleName))
            {
                rowData.AddRange(new[] {
                          resultModule.ModuleName,
                          (resultModule !=null && resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].Removed.HasValue)?
                          TableBlock.FormatEvolution(-resultModule[Constants.BusinessCriteria.TechnicalQualityIndex].Removed.Value) : Constants.No_Value,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Robustness].Removed.HasValue)?
                          TableBlock.FormatEvolution(-resultModule[Constants.BusinessCriteria.Robustness].Removed.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Performance].Removed.HasValue)?
                          TableBlock.FormatEvolution(-resultModule[Constants.BusinessCriteria.Performance].Removed.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Security].Removed.HasValue)?
                          TableBlock.FormatEvolution(-resultModule[Constants.BusinessCriteria.Security].Removed.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Transferability].Removed.HasValue)?
                          TableBlock.FormatEvolution(-resultModule[Constants.BusinessCriteria.Transferability].Removed.Value):Constants.No_Value ,

                          (resultModule !=null && resultModule[Constants.BusinessCriteria.Changeability].Removed.HasValue)?
                          TableBlock.FormatEvolution(-resultModule[Constants.BusinessCriteria.Changeability].Removed.Value):Constants.No_Value ,

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
