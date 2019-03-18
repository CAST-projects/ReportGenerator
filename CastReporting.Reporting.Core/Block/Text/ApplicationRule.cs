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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
// ReSharper disable InconsistentNaming

namespace CastReporting.Reporting.Block.Text
{
	[Block("APPLICATION_RULE"), Block("APPLICATION_METRIC")]
    public class ApplicationRule : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
          
            int metricId = options.GetIntOption("ID");
            int metricSzId = options.GetIntOption("SZID");
            int metricBfId = options.GetIntOption("BFID");
            string _format = options.GetOption("FORMAT", "N0");
            string _snapshot = options.GetOption("SNAPSHOT", "CURRENT");
           
            if (reportData?.CurrentSnapshot != null && metricId != 0)
            {
                if (_snapshot == "PREVIOUS")
                {
                    if (reportData.PreviousSnapshot == null) return Constants.No_Value;
                    double? result = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, metricId);
                    return result?.ToString("N2") ?? Constants.No_Value;
                }
                else
                {
                    double? result = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot, metricId);
                    return result?.ToString("N2") ?? Constants.No_Value;
                }
            }
            if (reportData?.CurrentSnapshot != null && metricSzId != 0)
            {
                if (_snapshot == "PREVIOUS")
                {
                    if (reportData.PreviousSnapshot == null) return Constants.No_Value;
                    double? result = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, metricSzId);
                    return result?.ToString(_format) ?? Constants.No_Value;
                }
                else
                {
                    double? result = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, metricSzId);
                    return result?.ToString(_format) ?? Constants.No_Value;
                }
            }
            // ReSharper disable once InvertIf
            if (reportData?.CurrentSnapshot != null && metricBfId != 0)
            {
                if (_snapshot == "PREVIOUS")
                {
                    if (reportData.PreviousSnapshot == null) return Constants.No_Value;
                    Result bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.PreviousSnapshot.Href, metricBfId.ToString()).FirstOrDefault();
                    if (bfValue == null || !bfValue.ApplicationResults.Any())
                        return Constants.No_Value;
                    double? result = bfValue.ApplicationResults[0].DetailResult?.Value;
                    return result?.ToString(_format) ?? Constants.No_Value;
                }
                else
                {
                    Result bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.CurrentSnapshot.Href, metricBfId.ToString()).FirstOrDefault();
                    if (bfValue == null || !bfValue.ApplicationResults.Any()) return Constants.No_Value;
                    double? result = bfValue.ApplicationResults[0].DetailResult?.Value;
                    return result?.ToString(_format) ?? Constants.No_Value;
                }
            }

            return Constants.No_Value;
        }
        #endregion METHODS
    }
}
