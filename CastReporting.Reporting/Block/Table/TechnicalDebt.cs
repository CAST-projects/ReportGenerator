
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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNICAL_DEBT")]
    public class TechnicalDebt : TableBlock
    {


        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
          
            string numberFormat = "N0";

            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

            List<string> rowData = new List<string>();

            if (null != reportData &&
                  null != reportData.CurrentSnapshot)
            {

                
                //Build Debt row          
                Double? technicalDebtBuild = MeasureUtility.GetTechnicalDebtMetric(reportData.CurrentSnapshot);
                rowData.AddRange(new string[] { "Name", "Value" });
                rowData.AddRange(new string[] {
                    displayShortHeader ? "Debt" : "Technical Debt",
                   technicalDebtBuild.HasValue? technicalDebtBuild.Value.ToString(numberFormat):CastReporting.Domain.Constants.No_Value ,                   
                });


                //Build Debt added row            
                Double? technicalDebtadded = MeasureUtility.SumDeltaIndicator(reportData.CurrentSnapshot, reportData.PreviousSnapshot, reportData.Application, Constants.SizingInformations.AddedViolationsTechnicalDebt);

                rowData.AddRange(new string[] {
                     displayShortHeader ? "Debt added" : "Technical Debt added",
                   technicalDebtadded.HasValue? technicalDebtadded.Value.ToString(numberFormat) : CastReporting.Domain.Constants.No_Value,                   
                });

                //Build Debt removed row            
                Double? technicalDebtremoved = MeasureUtility.SumDeltaIndicator(reportData.CurrentSnapshot, reportData.PreviousSnapshot, reportData.Application, Constants.SizingInformations.RemovedViolationsTechnicalDebt);

                rowData.AddRange(new string[] {
                    displayShortHeader ? "Debt removed" : "Technical Debt removed",
                   technicalDebtremoved.HasValue? technicalDebtremoved.Value.ToString(numberFormat):CastReporting.Domain.Constants.No_Value,                   
                });
            }

            //Build Table Definition            
            resultTable = new TableDefinition
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
