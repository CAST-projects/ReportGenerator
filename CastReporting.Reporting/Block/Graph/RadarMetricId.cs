
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
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Graph
{
	[Block("RADAR_METRIC_ID")]
    class RadarMetricId : GraphBlock
    {
        
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string[] QIDlist = options.GetOption("ID")?.Split('|');
            string _version = options.GetOption("SNAPSHOT", "BOTH");

            var rowData = new List<String>();
            rowData.Add(null);


            if (reportData != null && reportData.CurrentSnapshot != null)
            {

                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                rowData.Add(currSnapshotLabel.ToString());

                if (reportData.PreviousSnapshot != null)
                {
                    string prevSnapshotLabel = string.Empty;
                    prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot);
                    rowData.Add(prevSnapshotLabel ?? Constants.No_Value);

                }




                TableDefinition resultTable = new TableDefinition
                {
                    HasRowHeaders = true,
                    HasColumnHeaders = true,
                    //NbRows = ,
                    //NbColumns = ,
                    Data = rowData
                };

                return resultTable;
            }

            return null;
        }
     
    }

}
