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
            List<TechnologyResultDTO> TechnologyResultCurrentSnapshot = new List<TechnologyResultDTO>();
            List<TechnologyResultDTO> TechnologyResultPreviousSnapshot = new List<TechnologyResultDTO>();
            List<EvolutionSnapshots> ResultCompartTecno = new List<EvolutionSnapshots>();

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
	                ResultCompartTecno = (from techLocC in TechnologyResultCurrentSnapshot
	                                      select new EvolutionSnapshots() {
	                                          name = techLocC.Name,
	                                          curValue = techLocC.Value,
	                                          preValue = null,
	                                          evolValue = 0,
	
	                                      }).ToList();
	            	#endregion Current Snapshot

				} else {

            		#region Previous Snapshot
					TechnologyResultPreviousSnapshot = MeasureUtility.GetTechnoLoc(reportData.PreviousSnapshot, nbLimitTop);

                	ResultCompartTecno = (from techLocC in TechnologyResultCurrentSnapshot
	                                      from techLocP in TechnologyResultPreviousSnapshot
	                                      where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
	                                      select new EvolutionSnapshots() {
	                                          name = techLocC.Name,
	                                          curValue = techLocC.Value,
	                                          preValue = techLocP.Value,
	                                          evolValue = techLocC.Value - techLocP.Value,
	
	                                      }).ToList();

            		if (TechnologyResultPreviousSnapshot.Count != TechnologyResultCurrentSnapshot.Count) {
	                   	ResultCompartTecno.AddRange((from ResultCompart in ResultCompartTecno
	                                                from techLocC in TechnologyResultCurrentSnapshot
	                                                where ResultCompartTecno.TrueForAll(_ => _.name != techLocC.Name)
	                                                select new EvolutionSnapshots() {
	                                                    name = techLocC.Name,
	                                                    curValue = techLocC.Value,
	                                                    preValue = null,
	                                                    evolValue = null,
	                                                }));

                 		ResultCompartTecno.AddRange((from ResultCompart in ResultCompartTecno
                                                    from techLocP in TechnologyResultPreviousSnapshot
                                                    where ResultCompartTecno.TrueForAll(_ => _.name != techLocP.Name)
                                                    select new EvolutionSnapshots() {
                                                        name = techLocP.Name,
                                                        curValue = null,
                                                        preValue = techLocP.Value,
                                                        evolValue = null,
                                                    })); 
					}
					#endregion Previous Snapshot

				}
           }

           foreach (var item in ResultCompartTecno) {
               rowData.AddRange(new string[] {
								item.name
                                , item.curValue.HasValue? item.curValue.Value.ToString("N0") :CastReporting.Domain.Constants.No_Value
                                , (item.preValue.HasValue)? item.preValue.Value.ToString("N0"):CastReporting.Domain.Constants.No_Value 
                                , (item.evolValue.HasValue)? FormatEvolution((Int32)item.evolValue.Value) : CastReporting.Domain.Constants.No_Value
                                , (item.preValue.HasValue && item.evolValue.HasValue && item.preValue.Value !=0) ? FormatPercent(((item.curValue.Value-item.preValue.Value)/item.preValue.Value)) 
                                                                                                                 : CastReporting.Domain.Constants.No_Value
				});
           }

           resultTable = new TableDefinition {
               HasRowHeaders = false,
               HasColumnHeaders = true,
               NbRows = ResultCompartTecno.Count + 1,
               NbColumns = 5,
               Data = rowData
           };
           return resultTable;
        }
        #endregion METHODS
    }
}
