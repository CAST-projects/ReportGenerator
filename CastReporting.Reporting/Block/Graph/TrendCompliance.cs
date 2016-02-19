
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

using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
   
    [Block("TREND_COMPLIANCE")]
    class TrendCompliance : GraphBlock
    {
     
        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            double minValy = Int16.MaxValue;
            double maxValy = 0;
            double stepV = 0;
            int count = 0;         

            bool hasVerticalZoom = options.ContainsKey("ZOOM");
            if (!options.ContainsKey("ZOOM") || !Double.TryParse(options["ZOOM"], out stepV)) {
                stepV = 1;
            }
          
            
            var rowData = new List<String>();
			rowData.AddRange(new string[] {
				" ",
				Labels.Prog,
				Labels.Arch,
				Labels.Doc,
				Labels.LoC
			});

          
            #region Previous Snapshots
			int nbSnapshots = 0;
			nbSnapshots = (reportData != null && reportData.Application.Snapshots != null) ? reportData.Application.Snapshots.Count() : 0;
			if (nbSnapshots > 0) {
               
                foreach (Snapshot snapshot in reportData.Application.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot)) {
                    BusinessCriteriaDTO bcGrade = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(snapshot, true);
                    double? locValue = MeasureUtility.GetCodeLineNumber(snapshot);
                    string prevSnapshotDate = snapshot.Annotation.Date.DateSnapShot.HasValue ? snapshot.Annotation.Date.DateSnapShot.Value.ToOADate().ToString() 
                                                                                             : string.Empty;
                    rowData.AddRange
                        (new string[] { prevSnapshotDate
                            , bcGrade.ProgrammingPractices.GetValueOrDefault().ToString()
                            , bcGrade.ArchitecturalDesign.GetValueOrDefault().ToString()
                            , bcGrade.Documentation.GetValueOrDefault().ToString()
                            , locValue.GetValueOrDefault().ToString()
                            });
                    List<double> values = new List<double>() { bcGrade.ProgrammingPractices.GetValueOrDefault(), 
                                                               bcGrade.ArchitecturalDesign.GetValueOrDefault(), 
                                                               bcGrade.Documentation.GetValueOrDefault() };
                    minValy = Math.Min( minValy, values.Min());
                    maxValy = Math.Max( maxValy, values.Max());
                }
				count = nbSnapshots;
            }
            #endregion Previous Snapshots               

            #region just 1 snapshot
			if (nbSnapshots == 1) {
                BusinessCriteriaDTO bcGrade = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot, true);
                double? locValue = MeasureUtility.GetCodeLineNumber(reportData.CurrentSnapshot);
                string prevSnapshotDate = reportData.CurrentSnapshot.Annotation.Date.DateSnapShot.HasValue
                        ? reportData.CurrentSnapshot.Annotation.Date.DateSnapShot.Value.ToOADate().ToString() : string.Empty;
                rowData.AddRange
                    (new string[] { prevSnapshotDate
                            , bcGrade.ProgrammingPractices.GetValueOrDefault().ToString()
                            , bcGrade.ArchitecturalDesign.GetValueOrDefault().ToString()
                            , bcGrade.Documentation.GetValueOrDefault().ToString()
                            , locValue.GetValueOrDefault().ToString()
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
