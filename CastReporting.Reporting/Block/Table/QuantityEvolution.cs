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
    class QuantityEvolution : TableBlock
    {
        private const string _MetricFormat = "N2";
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            Boolean hasPrevious = reportData.PreviousSnapshot != null;

            List<TechnologyResultDTO> TechnologyResultCurrentSnapshot = new List<TechnologyResultDTO>();
            List<TechnologyResultDTO> TechnologyResultPreviousSnapshot = new List<TechnologyResultDTO>();
            List<EvolutionSnapshots> ResultCompartTecno = new List<EvolutionSnapshots>();

            List<TechnologyResultDTO> TechnologyDecisionPointsResultCurrentSnapshot = new List<TechnologyResultDTO>();
            List<TechnologyResultDTO> TechnologyDecisionPointsResultPreviousSnapshot = new List<TechnologyResultDTO>();
            List<EvolutionSnapshots> ResultCompartTecnoDecisionPoints = new List<EvolutionSnapshots>();

            List<TechnologyResultDTO> TechnologyClassesResultCurrentSnapshot = new List<TechnologyResultDTO>();
            List<TechnologyResultDTO> TechnologyClassesResultPreviousSnapshot = new List<TechnologyResultDTO>();
            List<EvolutionSnapshots> ResultCompartTecnoClasses = new List<EvolutionSnapshots>();

            DataTable dtFinalRepository = new DataTable();
            dtFinalRepository.Columns.Add("Name");
            dtFinalRepository.Columns.Add("DecisionP");
            dtFinalRepository.Columns.Add("KLOC");
            dtFinalRepository.Columns.Add("Classes");
            dtFinalRepository.AcceptChanges();

            List<string> rowData = new List<string>();
            rowData.AddRange(new string[] {
				Labels.Name,
				Labels.DecisionP, 
				"kLOC's",
				"Objects"
			});
            int nbLimitTop = 0;
            if (null == options || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }


            #region LOC Evolution
            if (null != reportData && null != reportData.CurrentSnapshot)
            {
                TechnologyResultCurrentSnapshot = MeasureUtility.GetTechnoLoc(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious)
                {

                    #region Current Snapshot
                    ResultCompartTecno = (from techLocC in TechnologyResultCurrentSnapshot
                                          select new EvolutionSnapshots()
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
                    TechnologyResultPreviousSnapshot = MeasureUtility.GetTechnoLoc(reportData.PreviousSnapshot, nbLimitTop);

                    ResultCompartTecno = (from techLocC in TechnologyResultCurrentSnapshot
                                          from techLocP in TechnologyResultPreviousSnapshot
                                          where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
                                          select new EvolutionSnapshots()
                                          {
                                              name = techLocC.Name,
                                              curValue = techLocC.Value,
                                              preValue = techLocP.Value,
                                              evolValue = techLocC.Value - techLocP.Value,

                                          }).ToList();

                    if (TechnologyResultPreviousSnapshot.Count != TechnologyResultCurrentSnapshot.Count)
                    {
                        ResultCompartTecno.AddRange((from ResultCompart in ResultCompartTecno
                                                     from techLocC in TechnologyResultCurrentSnapshot
                                                     where ResultCompartTecno.TrueForAll(_ => _.name != techLocC.Name)
                                                     select new EvolutionSnapshots()
                                                     {
                                                         name = techLocC.Name,
                                                         curValue = techLocC.Value,
                                                         preValue = null,
                                                         evolValue = null,
                                                     }));

                        ResultCompartTecno.AddRange((from ResultCompart in ResultCompartTecno
                                                     from techLocP in TechnologyResultPreviousSnapshot
                                                     where ResultCompartTecno.TrueForAll(_ => _.name != techLocP.Name)
                                                     select new EvolutionSnapshots()
                                                     {
                                                         name = techLocP.Name,
                                                         curValue = null,
                                                         preValue = techLocP.Value,
                                                         evolValue = null,
                                                     }));
                    }
                    #endregion Previous Snapshot

                }
            }
            #endregion LOC Evolution

            #region Decision Points
            if (null != reportData && null != reportData.CurrentSnapshot)
            {
                TechnologyDecisionPointsResultCurrentSnapshot = MeasureUtility.GetTechnoComplexity(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious)
                {

                    #region Current Snapshot
                    ResultCompartTecnoDecisionPoints = (from techLocC in TechnologyDecisionPointsResultCurrentSnapshot
                                                        select new EvolutionSnapshots()
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
                    TechnologyDecisionPointsResultPreviousSnapshot = MeasureUtility.GetTechnoComplexity(reportData.PreviousSnapshot, nbLimitTop);

                    ResultCompartTecnoDecisionPoints = (from techLocC in TechnologyDecisionPointsResultCurrentSnapshot
                                                        from techLocP in TechnologyDecisionPointsResultPreviousSnapshot
                                                        where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
                                                        select new EvolutionSnapshots()
                                                        {
                                                            name = techLocC.Name,
                                                            curValue = techLocC.Value,
                                                            preValue = techLocP.Value,
                                                            evolValue = techLocC.Value - techLocP.Value,

                                                        }).ToList();

                    if (TechnologyDecisionPointsResultPreviousSnapshot.Count != TechnologyDecisionPointsResultCurrentSnapshot.Count)
                    {
                        ResultCompartTecnoDecisionPoints.AddRange((from ResultCompart in ResultCompartTecnoDecisionPoints
                                                                   from techLocC in TechnologyDecisionPointsResultCurrentSnapshot
                                                                   where ResultCompartTecnoDecisionPoints.TrueForAll(_ => _.name != techLocC.Name)
                                                                   select new EvolutionSnapshots()
                                                                   {
                                                                       name = techLocC.Name,
                                                                       curValue = techLocC.Value,
                                                                       preValue = null,
                                                                       evolValue = null,
                                                                   }));

                        ResultCompartTecnoDecisionPoints.AddRange((from ResultCompart in ResultCompartTecnoDecisionPoints
                                                                   from techLocP in TechnologyDecisionPointsResultPreviousSnapshot
                                                                   where ResultCompartTecnoDecisionPoints.TrueForAll(_ => _.name != techLocP.Name)
                                                                   select new EvolutionSnapshots()
                                                                   {
                                                                       name = techLocP.Name,
                                                                       curValue = null,
                                                                       preValue = techLocP.Value,
                                                                       evolValue = null,
                                                                   }));
                    }
                    #endregion Previous Snapshot

                }
            }
            #endregion Decision Points

            #region Classes
            if (null != reportData && null != reportData.CurrentSnapshot)
            {
                TechnologyClassesResultCurrentSnapshot = MeasureUtility.GetTechnoClasses(reportData.CurrentSnapshot, nbLimitTop);

                if (!hasPrevious)
                {

                    #region Current Snapshot
                    ResultCompartTecnoClasses = (from techLocC in TechnologyClassesResultCurrentSnapshot
                                                 select new EvolutionSnapshots()
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
                    TechnologyClassesResultPreviousSnapshot = MeasureUtility.GetTechnoComplexity(reportData.PreviousSnapshot, nbLimitTop);

                    ResultCompartTecnoClasses = (from techLocC in TechnologyClassesResultCurrentSnapshot
                                                 from techLocP in TechnologyClassesResultPreviousSnapshot
                                                 where techLocC.Name.Equals(techLocP.Name) && (techLocP != null)
                                                 select new EvolutionSnapshots()
                                                 {
                                                     name = techLocC.Name,
                                                     curValue = techLocC.Value,
                                                     preValue = techLocP.Value,
                                                     evolValue = techLocC.Value - techLocP.Value,

                                                 }).ToList();

                    if (TechnologyClassesResultPreviousSnapshot.Count != TechnologyClassesResultCurrentSnapshot.Count)
                    {
                        ResultCompartTecnoClasses.AddRange((from ResultCompart in ResultCompartTecnoClasses
                                                            from techLocC in TechnologyClassesResultCurrentSnapshot
                                                            where ResultCompartTecnoClasses.TrueForAll(_ => _.name != techLocC.Name)
                                                            select new EvolutionSnapshots()
                                                            {
                                                                name = techLocC.Name,
                                                                curValue = techLocC.Value,
                                                                preValue = null,
                                                                evolValue = null,
                                                            }));

                        ResultCompartTecnoClasses.AddRange((from ResultCompart in ResultCompartTecnoClasses
                                                            from techLocP in TechnologyClassesResultPreviousSnapshot
                                                            where ResultCompartTecnoClasses.TrueForAll(_ => _.name != techLocP.Name)
                                                            select new EvolutionSnapshots()
                                                            {
                                                                name = techLocP.Name,
                                                                curValue = null,
                                                                preValue = techLocP.Value,
                                                                evolValue = null,
                                                            }));
                    }
                    #endregion Previous Snapshot

                }
            }
            #endregion Classes


            foreach (var item in ResultCompartTecno)
            {
                dtFinalRepository.Rows.Add(item.name, "", (item.evolValue.HasValue) ? FormatEvolution((Int32)item.evolValue.Value) : CastReporting.Domain.Constants.No_Value, "");
            }
            dtFinalRepository.AcceptChanges();

            foreach (var item in ResultCompartTecnoDecisionPoints)
            {
                for (int i = 0; i < dtFinalRepository.Rows.Count; i++)
                {
                    if (dtFinalRepository.Rows[i]["Name"].ToString() == item.name.ToString())
                    {
                        dtFinalRepository.Rows[i]["DecisionP"] = (item.evolValue.HasValue) ? FormatEvolution((Int32)item.evolValue.Value) : CastReporting.Domain.Constants.No_Value;
                        break;
                    }
                }
            }
            dtFinalRepository.AcceptChanges();


            foreach (var item in ResultCompartTecnoClasses)
            {
                for (int i = 0; i < dtFinalRepository.Rows.Count; i++)
                {
                    if (dtFinalRepository.Rows[i]["Name"].ToString() == item.name.ToString())
                    {
                        dtFinalRepository.Rows[i]["Classes"] = (item.evolValue.HasValue) ? FormatEvolution((Int32)item.evolValue.Value) : CastReporting.Domain.Constants.No_Value;
                        break;
                    }
                }
            }

            dtFinalRepository.AcceptChanges();



            for (int i = 0; i < dtFinalRepository.Rows.Count; i++)
            {
                rowData.AddRange(new string[] {
                    dtFinalRepository.Rows[i]["Name"].ToString()
                    , dtFinalRepository.Rows[i]["DecisionP"].ToString()
                    , dtFinalRepository.Rows[i]["KLOC"].ToString()
                    , dtFinalRepository.Rows[i]["Classes"].ToString()
                });
            }
            //foreach (var item in ResultCompartTecno)
            //{
            //    rowData.AddRange(new string[] { 
            //        item.name
            //        , ""
            //        , (item.evolValue.HasValue) ? FormatEvolution((Int32)item.evolValue.Value) : CastReporting.Domain.Constants.No_Value
            //        , ""
            //    });
            //}

            resultTable = new TableDefinition
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
