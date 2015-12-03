
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
using CastReporting.BLL.Computing.Properties;



namespace CastReporting.Reporting.Block.Graph 
{
    [Block("MODULES_ARTIFACTS")]
    class PieModuleArtifact : GraphBlock
    {


        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

            int nbResult = reportData.Parameter.NbResultDefault, tmpNb;
            if (null != options && options.ContainsKey("COUNT") && Int32.TryParse(options["COUNT"], out tmpNb) && tmpNb > 0)
            {
                nbResult = tmpNb;
            }

             

             if (null != reportData && null != reportData.CurrentSnapshot)
             {
                 var moduleArtifacts = MeasureUtility.GetModulesMeasure(reportData.CurrentSnapshot, nbResult, Constants.SizingInformations.ArtifactNumber);

                 List<string> rowData = new List<string>();
                 rowData.AddRange(new string[] { "Name", "Artifacts" });
          
                 foreach (var mod in moduleArtifacts)
                 {
                     rowData.AddRange(new string[] { mod.Name, Convert.ToInt32(mod.Value).ToString() });
                 }
              

                 TableDefinition resultTable = new TableDefinition
                 {
                     HasRowHeaders = true,
                     HasColumnHeaders = false,
                     NbRows = moduleArtifacts.Count() + 1,
                     NbColumns = 2,
                     Data = rowData
                 };

                 return resultTable;
             }

             return null;
        
        }

        
        #endregion METHODS
    }

}