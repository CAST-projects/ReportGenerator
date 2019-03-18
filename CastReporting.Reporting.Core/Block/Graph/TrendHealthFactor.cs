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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
   
    [Block("TREND_HEALTH_FACTOR")]
    public class TrendHealthFactor : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
             
            double minVVal = short.MaxValue;
            double maxVVal = 0;
            double stepV;           
            int count = 0;
        
            bool hasVerticalZoom = options.ContainsKey("ZOOM");
            if (!options.ContainsKey("ZOOM") || !double.TryParse(options["ZOOM"], out stepV)) {
                stepV = 1;
            }
          
            var rowData = new List<string>();
			rowData.AddRange(new[] {
				" ",
				Labels.Trans,
				Labels.Chang,
				Labels.Robu,
				Labels.Efcy,
				Labels.Secu,
				Labels.LoC
			});

            
            #region Fetch Snapshots
			int nbSnapshots = reportData?.Application.Snapshots?.Count() ?? 0;
			if (nbSnapshots > 0)
			{
			    var _snapshots = reportData?.Application?.Snapshots?.OrderBy(_ => _.Annotation.Date);
			    if (_snapshots != null)
			        foreach (Snapshot snapshot in _snapshots)
                    {
			            BusinessCriteriaDTO bcGrade = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(snapshot, true);
			            double? locValue = MeasureUtility.GetCodeLineNumber(snapshot);
			            string snapshotDate = snapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty;

                        rowData.Add(snapshotDate);
                        rowData.Add(bcGrade.Transferability.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(bcGrade.Changeability.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(bcGrade.Robustness.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(bcGrade.Performance.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(bcGrade.Security.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(locValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));

			            List<double> values = new List<double>
			            {
                            bcGrade.Changeability.GetValueOrDefault(), 
			                bcGrade.Performance.GetValueOrDefault(), 
			                bcGrade.Robustness.GetValueOrDefault(), 
			                bcGrade.Security.GetValueOrDefault(), 
			                bcGrade.TQI.GetValueOrDefault(), 
			                bcGrade.Transferability.GetValueOrDefault() 
			            };
			            minVVal = Math.Min(minVVal, values.Min());
			            maxVVal = Math.Max(maxVVal, values.Max());
                       
			        }
			    count = nbSnapshots;
			}
            #endregion

            #region just 1 snapshot
			if (nbSnapshots == 1)
            {
			    var bcGrade = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData?.CurrentSnapshot, true);
                double? locValue = MeasureUtility.GetCodeLineNumber(reportData?.CurrentSnapshot);
                string snapshotDate = reportData?.CurrentSnapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty;
                rowData.AddRange(new[] {
                    snapshotDate,
                    bcGrade.Transferability.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                    bcGrade.Changeability.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                    bcGrade.Robustness.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                    bcGrade.Performance.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                    bcGrade.Security.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                    locValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),

                });
                List<double> values = new List<double>
                {
                    bcGrade.Changeability.GetValueOrDefault(), 
                    bcGrade.Performance.GetValueOrDefault(), 
                    bcGrade.Robustness.GetValueOrDefault(), 
                    bcGrade.Security.GetValueOrDefault(), 
                    bcGrade.TQI.GetValueOrDefault(), 
                    bcGrade.Transferability.GetValueOrDefault()
                };
                minVVal = Math.Min(minVVal, values.Min());
                maxVVal = Math.Max(maxVVal, values.Max());
                count = count + 1;
            }
            #endregion just 1 snapshot
               
           

            #region Graphic Options
            GraphOptions graphOptions = null;
            if (hasVerticalZoom) {
                graphOptions = new GraphOptions() { AxisConfiguration = new AxisDefinition() };
                graphOptions.AxisConfiguration.VerticalAxisMinimal = MathUtility.GetVerticalMinValue(minVVal, stepV);
                graphOptions.AxisConfiguration.VerticalAxisMaximal = MathUtility.GetVerticalMaxValue(maxVVal, stepV);
            }
            #endregion Graphic Options

            TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = count+1,
                NbColumns = 7,
                Data = rowData,
                GraphOptions = graphOptions
            };

           
            return resultTable;
        }
    }
}