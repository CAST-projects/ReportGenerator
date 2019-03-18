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
using CastReporting.Domain;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("LOC_BY_MODULE")]
    public class LocByModule : TableBlock
    {
        /// <summary>
        /// 
        /// </summary>
        private const string MetricFormat = "N0";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbTot = 0;
            List<string> rowData = new List<string>();

            bool kloc = options.GetOption("FORMAT", "LOC").Equals("KLOC");

            rowData.AddRange(kloc ? new[] {Labels.ModuleName, Labels.kLoC} : new[] {Labels.ModuleName, Labels.LoC});

            if (reportData?.CurrentSnapshot?.Modules != null)
            {
               var result = reportData.CurrentSnapshot.SizingMeasuresResults.FirstOrDefault(v => v.Reference.Key == (int)Constants.SizingInformations.CodeLineNumber);

               if (result != null)
               {
                   foreach (var res in result.ModulesResult)
                   {
                       double? codeLineNb = res.DetailResult.Value;
                       rowData.AddRange(kloc ?
                           new[] { res.Module.Name, (codeLineNb / 1000)?.ToString(MetricFormat) }
                           : new[] { res.Module.Name, codeLineNb?.ToString(MetricFormat) }
                       );
                   }
                   nbTot = result.ModulesResult.Length;
               }
            }
           var resultTable = new TableDefinition
           {
               HasRowHeaders = false,
               HasColumnHeaders = true,
               NbRows = nbTot + 1,
               NbColumns = 2,
               Data = rowData
           };
            return resultTable;
        }




    }
}
