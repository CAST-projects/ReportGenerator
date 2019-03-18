
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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNICAL_DEBT")]
    public class TechnicalDebt : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            const string numberFormat = "N0";

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

            List<string> rowData = new List<string>();

            if (reportData?.CurrentSnapshot != null)
            {

                
                //Build Debt row          
                double? technicalDebtBuild = MeasureUtility.GetTechnicalDebtMetric(reportData.CurrentSnapshot);
                rowData.AddRange(new[] { Labels.Name, Labels.Value });
                rowData.AddRange(new[] {
                    displayShortHeader ? Labels.Debt : Labels.TechnicalDebt  + " (" + reportData.CurrencySymbol + ")",
                   technicalDebtBuild?.ToString(numberFormat) ?? Constants.No_Value ,                   
                });


                //Build Debt added row            
                double? technicalDebtadded = MeasureUtility.SumDeltaIndicator(reportData.CurrentSnapshot, reportData.PreviousSnapshot, reportData.Application, Constants.SizingInformations.AddedViolationsTechnicalDebt);

                rowData.AddRange(new[] {
                     displayShortHeader ? Labels.DebtAdded : Labels.TechnicalDebtAdded + " (" + reportData.CurrencySymbol + ")",
                   technicalDebtadded?.ToString(numberFormat) ?? Constants.No_Value,                   
                });

                //Build Debt removed row            
                double? technicalDebtremoved = MeasureUtility.SumDeltaIndicator(reportData.CurrentSnapshot, reportData.PreviousSnapshot, reportData.Application, Constants.SizingInformations.RemovedViolationsTechnicalDebt);

                rowData.AddRange(new[] {
                     displayShortHeader ? Labels.DebtRemoved : Labels.TechnicalDebtRemoved + " (" + reportData.CurrencySymbol + ")",
                   technicalDebtremoved?.ToString(numberFormat) ?? Constants.No_Value,                   
                });
            }

            //Build Table Definition            
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 3,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }       
  
    }
}
