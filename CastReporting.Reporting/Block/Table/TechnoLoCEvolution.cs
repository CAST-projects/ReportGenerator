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
using CastReporting.BLL.Computing.DTO;


namespace CastReporting.Reporting.Block.Table
{
    /// <summary>
    /// TechnoLoCEvolution Class
    /// </summary>
    [Block("TECHNO_LOC_EVOLUTION")]
    class TechnoLoCEvolution : TableBlock
    {
          #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            Boolean hasPrevious = reportData.PreviousSnapshot != null;
            List<TechnologyResultDTO> TechnologyResultCurrentSnapshot = null;
            List<TechnologyResultDTO> TechnologyResultPreviousSnapshot = null;
            List<EvolutionSnapshots> ResultCompartTecno = null;

            List<string> rowData = new List<string>();
			rowData.AddRange(new string[] {
				Labels.Name,
				Labels.LoCCurrent,
				Labels.LoCPrevious,
				Labels.Evolution,
				Labels.EvolutionPercent
			});
            int nbLimitTop = 0;
            if (null == options || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            if (null != reportData && null != reportData.CurrentSnapshot) {
                TechnologyResultCurrentSnapshot = MeasureUtility.GetTechnoLoc(reportData.CurrentSnapshot, nbLimitTop);
                
				if (!hasPrevious) {

					#region Current Snapshot
	                ResultCompartTecno = (from cur in TechnologyResultCurrentSnapshot
	                                      select new EvolutionSnapshots() {
	                                          name = cur.Name,
	                                          curValue = cur.Value,
	                                          preValue = null,
	                                          evolValue = 0,
	
	                                      }).ToList();
	            	#endregion Current Snapshot

				} else {

            		#region Previous Snapshot
					TechnologyResultPreviousSnapshot = MeasureUtility.GetTechnoLoc(reportData.PreviousSnapshot, nbLimitTop);

                	ResultCompartTecno = (from cur in TechnologyResultCurrentSnapshot
	                                      from prev in TechnologyResultPreviousSnapshot
	                                      where prev != null && cur.Name == prev.Name
	                                      select new EvolutionSnapshots() {
	                                          name = cur.Name,
	                                          curValue = cur.Value,
	                                          preValue = prev.Value,
	                                          evolValue = cur.Value - prev.Value,
	
	                                      }).ToList();

            		if (TechnologyResultPreviousSnapshot.Count != TechnologyResultCurrentSnapshot.Count) {
	                   	ResultCompartTecno.AddRange(from cur in TechnologyResultCurrentSnapshot
	                                                where ResultCompartTecno.TrueForAll(_ => _.name != cur.Name)
	                                                select new EvolutionSnapshots() {
	                                                    name = cur.Name,
	                                                    curValue = cur.Value,
	                                                    preValue = null,
	                                                    evolValue = null,
	                                                });

                 		ResultCompartTecno.AddRange(from prev in TechnologyResultPreviousSnapshot
                                                    where ResultCompartTecno.TrueForAll(_ => _.name != prev.Name)
                                                    select new EvolutionSnapshots() {
                                                        name = prev.Name,
                                                        curValue = 0,
                                                        preValue = prev.Value,
                                                        evolValue = -prev.Value,
                                                    }); 
					}
					#endregion Previous Snapshot

				}
           }

            int nbRows = 0;
            if (ResultCompartTecno != null)
            {
                foreach (var item in ResultCompartTecno.Take(nbLimitTop))
                {
                    rowData.AddRange(new string[] {
                                item.name
                                , (item.curValue.HasValue)? item.curValue.Value.ToString("N0") :CastReporting.Domain.Constants.No_Value
                                , (item.preValue.HasValue)? item.preValue.Value.ToString("N0"):CastReporting.Domain.Constants.No_Value
                                , (item.evolValue.HasValue)? FormatEvolution((Int32)item.evolValue.Value) : CastReporting.Domain.Constants.No_Value
                                , (item.evolValue.HasValue && item.preValue.HasValue && item.preValue.Value !=0) ? FormatPercent((item.evolValue.Value/item.preValue.Value))
                                                                                                                 : CastReporting.Domain.Constants.No_Value
                    });
                    nbRows++;
                }
            }

           resultTable = new TableDefinition {
               HasRowHeaders = false,
               HasColumnHeaders = true,
               NbRows = nbRows + 1,
               NbColumns = 5,
               Data = rowData
           };
           return resultTable;
        }
        #endregion METHODS
    }
}
