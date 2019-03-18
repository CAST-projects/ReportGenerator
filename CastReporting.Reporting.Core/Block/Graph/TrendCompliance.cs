
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
   
    [Block("TREND_COMPLIANCE")]
    public class TrendCompliance : GraphBlock
    {
     
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            double minValy = short.MaxValue;
            double maxValy = 0;
            double stepV;
            int count = 0;         

            bool hasVerticalZoom = options.ContainsKey("ZOOM");
            if (!options.ContainsKey("ZOOM") || !double.TryParse(options["ZOOM"], out stepV)) {
                stepV = 1;
            }
          
            
            var rowData = new List<string>();
			rowData.AddRange(new[] {
				" ",
				Labels.Prog,
				Labels.Arch,
				Labels.Doc,
				Labels.LoC
			});

          
            #region Previous Snapshots

            var nbSnapshots = reportData?.Application.Snapshots?.Count() ?? 0;
			if (nbSnapshots > 0)
			{
			    var _snapshots = reportData?.Application?.Snapshots?.OrderBy(_ => _.Annotation.Date.DateSnapShot);
			    if (_snapshots != null)
			        foreach (Snapshot snapshot in _snapshots)
                    {
			            BusinessCriteriaDTO bcGrade = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(snapshot, true);
			            double? locValue = MeasureUtility.GetCodeLineNumber(snapshot);
			            string prevSnapshotDate = snapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty;
			            rowData.Add(prevSnapshotDate);
			            rowData.Add(bcGrade.ProgrammingPractices.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        rowData.Add(bcGrade.ArchitecturalDesign.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
			            rowData.Add(bcGrade.Documentation.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
			            rowData.Add(locValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));

			            List<double> values = new List<double>
                        {
                            bcGrade.ProgrammingPractices.GetValueOrDefault(), 
			                bcGrade.ArchitecturalDesign.GetValueOrDefault(), 
			                bcGrade.Documentation.GetValueOrDefault()
                        };
			            minValy = Math.Min( minValy, values.Min());
			            maxValy = Math.Max( maxValy, values.Max());
			        }
			    count = nbSnapshots;
			}
            #endregion Previous Snapshots               

            #region just 1 snapshot
			if (nbSnapshots == 1) {
                BusinessCriteriaDTO bcGrade = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData?.CurrentSnapshot, true);
                double? locValue = MeasureUtility.GetCodeLineNumber(reportData?.CurrentSnapshot);
                string prevSnapshotDate = reportData?.CurrentSnapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty;
                rowData.AddRange
                    (new[] { prevSnapshotDate
                            , bcGrade.ProgrammingPractices.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)
                            , bcGrade.ArchitecturalDesign.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)
                            , bcGrade.Documentation.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)
                            , locValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)
                            });

                List<double> values = new List<double>() { bcGrade.ProgrammingPractices.GetValueOrDefault(), 
                                                           bcGrade.ArchitecturalDesign.GetValueOrDefault(), 
                                                           bcGrade.Documentation.GetValueOrDefault() };
                minValy = Math.Min( minValy, values.Min());
                maxValy = Math.Max( maxValy, values.Max());
                count = count +1;
            }
            #endregion just 1 snapshot
            
            
            #region Graphic Options
            GraphOptions graphOptions = null;
            if (hasVerticalZoom) {
                graphOptions = new GraphOptions() { AxisConfiguration = new AxisDefinition() };
                graphOptions.AxisConfiguration.VerticalAxisMinimal = MathUtility.GetVerticalMinValue(minValy, stepV);
                graphOptions.AxisConfiguration.VerticalAxisMaximal = MathUtility.GetVerticalMaxValue(maxValy, stepV);
            }
            #endregion Graphic Options

            TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows =count+1 ,
                NbColumns = 5,
                Data = rowData,
                GraphOptions = graphOptions
            };


            return resultTable;
        }
        #endregion METHODS

    }
}
