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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Text
{
    [Block("LAST_SNAPSHOT_VERSION")]
    public class LastSnapshotVersionNumber : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            return reportData?.CurrentSnapshot != null ? SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot) ?? Domain.Constants.No_Value : Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}
