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
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.BLL.Computing.DTO;


namespace CastReporting.Reporting.Block.Table
{
    /// <summary>
    /// TechnoLoCEvolution Class
    /// </summary>
    [Block("TECHNO_LOC_EVOLUTION")]
    public class TechnoLoCEvolution : TableBlock
    {
          #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            bool hasPrevious = reportData.PreviousSnapshot != null;
            List<EvolutionSnapshots> _resultCompartTecno = null;

            List<string> rowData = new List<string>();
			rowData.AddRange(new[] {
				Labels.Name,
				Labels.LoCCurrent,
				Labels.LoCPrevious,
				Labels.Evolution,
				Labels.EvolutionPercent
			});
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            if (reportData.CurrentSnapshot != null)
            {
                var _technologyResultCurrentSnapshot = MeasureUtility.GetTechnoLoc(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious) {

					#region Current Snapshot
	                _resultCompartTecno = (from cur in _technologyResultCurrentSnapshot
	                                      select new EvolutionSnapshots
	                                      {
	                                          name = cur.Name,
	                                          curValue = cur.Value,
	                                          preValue = null,
	                                          evolValue = null,
	
	                                      }).ToList();
	            	#endregion Current Snapshot

				} else {

            		#region Previous Snapshot
					var _technologyResultPreviousSnapshot = MeasureUtility.GetTechnoLoc(reportData.PreviousSnapshot, nbLimitTop);

                	_resultCompartTecno = (from cur in _technologyResultCurrentSnapshot
	                                      from prev in _technologyResultPreviousSnapshot
	                                      where prev != null && cur.Name == prev.Name
	                                      select new EvolutionSnapshots
	                                      {
	                                          name = cur.Name,
	                                          curValue = cur.Value,
	                                          preValue = prev.Value,
	                                          evolValue = cur.Value - prev.Value,
	
	                                      }).ToList();

            		if (_technologyResultPreviousSnapshot.Count != _technologyResultCurrentSnapshot.Count) {
	                   	_resultCompartTecno.AddRange(from cur in _technologyResultCurrentSnapshot
	                                                where _resultCompartTecno.TrueForAll(_ => _.name != cur.Name)
	                                                select new EvolutionSnapshots
	                                                {
	                                                    name = cur.Name,
	                                                    curValue = cur.Value,
	                                                    preValue = null,
	                                                    evolValue = null,
	                                                });

                 		_resultCompartTecno.AddRange(from prev in _technologyResultPreviousSnapshot
                                                    where _resultCompartTecno.TrueForAll(_ => _.name != prev.Name)
                                                    select new EvolutionSnapshots
                                                    {
                                                        name = prev.Name,
                                                        curValue = null,
                                                        preValue = prev.Value,
                                                        evolValue = -prev.Value,
                                                    }); 
					}
					#endregion Previous Snapshot

				}
            }

            int nbRows = 0;
            if (_resultCompartTecno != null)
            {
                foreach (var item in _resultCompartTecno.Take(nbLimitTop))
                {
                    rowData.AddRange(new[] {
                                item.name
                                , item.curValue?.ToString("N0") ?? Domain.Constants.No_Value
                                , item.preValue?.ToString("N0") ?? Domain.Constants.No_Value
                                , (item.evolValue.HasValue)? FormatEvolution((int)item.evolValue.Value) : Domain.Constants.No_Value
                                , (item.evolValue.HasValue && item.preValue.HasValue && Math.Abs(item.preValue.Value) > 0) ? FormatPercent((item.evolValue.Value/item.preValue.Value))
                                                                                                                 : Domain.Constants.No_Value
                    });
                    nbRows++;
                }
            }

           var resultTable = new TableDefinition {
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
