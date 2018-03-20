/*
 *   Copyright (c) 2018 CAST
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
using System.Data;
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
    [Block("QUANTITY_EVOLUTION")]
    internal class QuantityEvolution : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            bool hasPrevious = reportData.PreviousSnapshot != null;

            List<EvolutionSnapshots> _resultCompartTecno = new List<EvolutionSnapshots>();
            List<EvolutionSnapshots> _resultCompartTecnoDecisionPoints = new List<EvolutionSnapshots>();
            List<EvolutionSnapshots> _resultCompartTecnoClasses = new List<EvolutionSnapshots>();

            DataTable dtFinalRepository = new DataTable();
            dtFinalRepository.Columns.Add("Name");
            dtFinalRepository.Columns.Add("DecisionP");
            dtFinalRepository.Columns.Add("KLOC");
            dtFinalRepository.Columns.Add("Classes");
            dtFinalRepository.AcceptChanges();

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] {
				Labels.Name,
				Labels.DecisionP, 
				"kLOC's",
				"Objects"
			});
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }


            #region LOC Evolution
            if (null != reportData.CurrentSnapshot)
            {
                var _technologyResultCurrentSnapshot = MeasureUtility.GetTechnoLoc(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious)
                {

                    #region Current Snapshot
                    _resultCompartTecno = (from techLocC in _technologyResultCurrentSnapshot
                                          select new EvolutionSnapshots
                                          {
                                              name = techLocC.Name,
                                              curValue = techLocC.Value,
                                              preValue = null,
                                              evolValue = 0,

                                          }).ToList();
                    #endregion Current Snapshot

                }
                else
                {

                    #region Previous Snapshot
                    var _technologyResultPreviousSnapshot = MeasureUtility.GetTechnoLoc(reportData.PreviousSnapshot, nbLimitTop);

                    _resultCompartTecno = (from techLocC in _technologyResultCurrentSnapshot
                                          from techLocP in _technologyResultPreviousSnapshot
                                          where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
                                          select new EvolutionSnapshots
                                          {
                                              name = techLocC.Name,
                                              curValue = techLocC.Value,
                                              preValue = techLocP.Value,
                                              evolValue = techLocC.Value - techLocP.Value,

                                          }).ToList();

                    if (_technologyResultPreviousSnapshot.Count != _technologyResultCurrentSnapshot.Count)
                    {
                        _resultCompartTecno.AddRange(from _resultCompart in _resultCompartTecno
                            from techLocC in _technologyResultCurrentSnapshot
                            where _resultCompartTecno.TrueForAll(_ => _.name != techLocC.Name)
                            select new EvolutionSnapshots
                            {
                                name = techLocC.Name,
                                curValue = techLocC.Value,
                                preValue = null,
                                evolValue = null,
                            });

                        _resultCompartTecno.AddRange(from _resultCompart in _resultCompartTecno
                            from techLocP in _technologyResultPreviousSnapshot
                            where _resultCompartTecno.TrueForAll(_ => _.name != techLocP.Name)
                            select new EvolutionSnapshots
                            {
                                name = techLocP.Name,
                                curValue = null,
                                preValue = techLocP.Value,
                                evolValue = null,
                            });
                    }
                    #endregion Previous Snapshot

                }
            }
            #endregion LOC Evolution

            #region Decision Points
            if (reportData.CurrentSnapshot != null)
            {
                var _technologyDecisionPointsResultCurrentSnapshot = MeasureUtility.GetTechnoComplexity(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious)
                {

                    #region Current Snapshot
                    _resultCompartTecnoDecisionPoints = (from techLocC in _technologyDecisionPointsResultCurrentSnapshot
                                                        select new EvolutionSnapshots
                                                        {
                                                            name = techLocC.Name,
                                                            curValue = techLocC.Value,
                                                            preValue = null,
                                                            evolValue = 0,

                                                        }).ToList();
                    #endregion Current Snapshot

                }
                else
                {

                    #region Previous Snapshot
                    var _technologyDecisionPointsResultPreviousSnapshot = MeasureUtility.GetTechnoComplexity(reportData.PreviousSnapshot, nbLimitTop);

                    _resultCompartTecnoDecisionPoints = (from techLocC in _technologyDecisionPointsResultCurrentSnapshot
                                                        from techLocP in _technologyDecisionPointsResultPreviousSnapshot
                                                        where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
                                                        select new EvolutionSnapshots
                                                        {
                                                            name = techLocC.Name,
                                                            curValue = techLocC.Value,
                                                            preValue = techLocP.Value,
                                                            evolValue = techLocC.Value - techLocP.Value,

                                                        }).ToList();

                    if (_technologyDecisionPointsResultPreviousSnapshot.Count != _technologyDecisionPointsResultCurrentSnapshot.Count)
                    {
                        _resultCompartTecnoDecisionPoints.AddRange(from _resultCompart in _resultCompartTecnoDecisionPoints
                            from techLocC in _technologyDecisionPointsResultCurrentSnapshot
                            where _resultCompartTecnoDecisionPoints.TrueForAll(_ => _.name != techLocC.Name)
                            select new EvolutionSnapshots
                            {
                                name = techLocC.Name,
                                curValue = techLocC.Value,
                                preValue = null,
                                evolValue = null,
                            });

                        _resultCompartTecnoDecisionPoints.AddRange(from _resultCompart in _resultCompartTecnoDecisionPoints
                            from techLocP in _technologyDecisionPointsResultPreviousSnapshot
                            where _resultCompartTecnoDecisionPoints.TrueForAll(_ => _.name != techLocP.Name)
                            select new EvolutionSnapshots
                            {
                                name = techLocP.Name,
                                curValue = null,
                                preValue = techLocP.Value,
                                evolValue = null,
                            });
                    }
                    #endregion Previous Snapshot

                }
            }
            #endregion Decision Points

            #region Classes
            if (reportData.CurrentSnapshot != null)
            {
                var _technologyClassesResultCurrentSnapshot = MeasureUtility.GetTechnoClasses(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious)
                {

                    #region Current Snapshot
                    _resultCompartTecnoClasses = (from techLocC in _technologyClassesResultCurrentSnapshot
                                                 select new EvolutionSnapshots
                                                 {
                                                     name = techLocC.Name,
                                                     curValue = techLocC.Value,
                                                     preValue = null,
                                                     evolValue = 0,

                                                 }).ToList();
                    #endregion Current Snapshot

                }
                else
                {

                    #region Previous Snapshot
                    var _technologyClassesResultPreviousSnapshot = MeasureUtility.GetTechnoComplexity(reportData.PreviousSnapshot, nbLimitTop);

                    _resultCompartTecnoClasses = (from techLocC in _technologyClassesResultCurrentSnapshot
                                                 from techLocP in _technologyClassesResultPreviousSnapshot
                                                 where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
                                                 select new EvolutionSnapshots
                                                 {
                                                     name = techLocC.Name,
                                                     curValue = techLocC.Value,
                                                     preValue = techLocP.Value,
                                                     evolValue = techLocC.Value - techLocP.Value,

                                                 }).ToList();

                    if (_technologyClassesResultPreviousSnapshot.Count != _technologyClassesResultCurrentSnapshot.Count)
                    {
                        _resultCompartTecnoClasses.AddRange(from _resultCompart in _resultCompartTecnoClasses
                            from techLocC in _technologyClassesResultCurrentSnapshot
                            where _resultCompartTecnoClasses.TrueForAll(_ => _.name != techLocC.Name)
                            select new EvolutionSnapshots
                            {
                                name = techLocC.Name,
                                curValue = techLocC.Value,
                                preValue = null,
                                evolValue = null,
                            });

                        _resultCompartTecnoClasses.AddRange(from _resultCompart in _resultCompartTecnoClasses
                            from techLocP in _technologyClassesResultPreviousSnapshot
                            where _resultCompartTecnoClasses.TrueForAll(_ => _.name != techLocP.Name)
                            select new EvolutionSnapshots
                            {
                                name = techLocP.Name,
                                curValue = null,
                                preValue = techLocP.Value,
                                evolValue = null,
                            });
                    }
                    #endregion Previous Snapshot

                }
            }
            #endregion Classes


            foreach (var item in _resultCompartTecno)
            {
                dtFinalRepository.Rows.Add(item.name, "", (item.evolValue.HasValue) ? FormatEvolution((int)item.evolValue.Value) : Domain.Constants.No_Value, "");
            }
            dtFinalRepository.AcceptChanges();

            foreach (var item in _resultCompartTecnoDecisionPoints)
            {
                for (int i = 0; i < dtFinalRepository.Rows.Count; i++)
                {
                    if (dtFinalRepository.Rows[i]["Name"].ToString() != item.name) continue;
                    dtFinalRepository.Rows[i]["DecisionP"] = (item.evolValue.HasValue) ? FormatEvolution((int)item.evolValue.Value) : Domain.Constants.No_Value;
                    break;
                }
            }
            dtFinalRepository.AcceptChanges();


            foreach (var item in _resultCompartTecnoClasses)
            {
                for (int i = 0; i < dtFinalRepository.Rows.Count; i++)
                {
                    if (dtFinalRepository.Rows[i]["Name"].ToString() != item.name) continue;
                    dtFinalRepository.Rows[i]["Classes"] = (item.evolValue.HasValue) ? FormatEvolution((int)item.evolValue.Value) : Domain.Constants.No_Value;
                    break;
                }
            }

            dtFinalRepository.AcceptChanges();



            for (int i = 0; i < dtFinalRepository.Rows.Count; i++)
            {
                rowData.AddRange(new[] {
                    dtFinalRepository.Rows[i]["Name"].ToString()
                    , dtFinalRepository.Rows[i]["DecisionP"].ToString()
                    , dtFinalRepository.Rows[i]["KLOC"].ToString()
                    , dtFinalRepository.Rows[i]["Classes"].ToString()
                });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbLimitTop + 1,
                NbColumns = 4,
                Data = rowData
            };
            return resultTable;
        }
    }
}
