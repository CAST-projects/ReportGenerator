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
using CastReporting.Domain;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessCriteriaUtility 
    {
        
         #region METHODS

         /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<BusinessCriteriaDTO> GetBusinessCriteriaGradesModules(Snapshot snapshot, bool round)
        {
            if (snapshot?.BusinessCriteriaResults == null) return null;

            var modules = snapshot.BusinessCriteriaResults.SelectMany(_ => _.ModulesResult).Select(_ => _.Module).Distinct();

            var result = modules.Select(module => new BusinessCriteriaDTO
                {
                    Name = module.Name,
                    TQI = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.TechnicalQualityIndex, round),
                    Robustness = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Robustness, round),
                    Performance = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Performance, round),
                    Security = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Security, round),
                    Changeability = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Changeability, round),
                    Transferability = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Transferability, round),
                })
                .ToList();
              
            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static BusinessCriteriaDTO GetBusinessCriteriaGradesSnapshot(Snapshot snapshot, bool round)
        {
            if (null == snapshot) return null;
            BusinessCriteriaDTO result = new BusinessCriteriaDTO
            {
                TQI = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.TechnicalQualityIndex, round),
                Robustness = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Robustness, round),
                Performance = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Performance, round),
                Security = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Security, round),
                Transferability = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Transferability, round),
                Changeability = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Changeability, round),
                ProgrammingPractices = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.ProgrammingPractices, round),
                ArchitecturalDesign = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.ArchitecturalDesign, round),
                Documentation = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Documentation, round),
                SEIMaintainability = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.SEIMaintainability, round)
            };
            
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="moduleHRef"></param>
        /// <param name="bcId"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static double? GetBusinessCriteriaModuleGrade(Snapshot snapshot, string moduleHRef, Constants.BusinessCriteria bcId, bool round)
        {
            double? res = null;
            if (snapshot?.BusinessCriteriaResults == null) return null;
            double? result = snapshot.BusinessCriteriaResults
                .Where(_ => _.Reference.Key == (int)bcId && _.ModulesResult != null)
                .SelectMany(_ => _.ModulesResult)
                .Where(_ => _.Module.Href == moduleHRef && _.DetailResult != null )
                .Select(_ => _.DetailResult.Grade)
                .FirstOrDefault();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (result != null)
            {
                res = round ? MathUtility.GetRound(result) : result;
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="moduleId"></param>
        /// <param name="bcId"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static double? GetBusinessCriteriaModuleGrade(Snapshot snapshot, int moduleId, Constants.BusinessCriteria bcId, bool round)
        {
            if (snapshot?.BusinessCriteriaResults == null) return null;
            double? res = null;
            double? result = snapshot.BusinessCriteriaResults
                .Where(_ => _.Reference.Key == (int)bcId && _.ModulesResult != null)
                .SelectMany(_ => _.ModulesResult)
                .Where(_ => _.Module.Id == moduleId && _.DetailResult != null)
                .Select(_ => _.DetailResult.Grade)
                .FirstOrDefault();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (result != null)
            {
                res = round ? MathUtility.GetRound(result) : result;
            }
            return res;
        }


        /// <summary>
        /// Get TQI for snapshot
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="bcId"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static double? GetSnapshotBusinessCriteriaGrade(Snapshot snapshot, Constants.BusinessCriteria bcId, bool round)
        {
            double? res = null;
            var resultBC = snapshot?.BusinessCriteriaResults?.SingleOrDefault(_ => _.Reference.Key == bcId.GetHashCode());
            if (resultBC != null)
            {
                res = round ? MathUtility.GetRound(resultBC.DetailResult.Grade) : resultBC.DetailResult.Grade;
            }
            return res;
        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        public static Double? GetMetricValue(Snapshot snapshot, int metricId)
        {
            double? retour = null;
            if (null != snapshot )
            {
                var bizGrade = snapshot.BusinessCriteriaResults!=null? snapshot.BusinessCriteriaResults.SingleOrDefault(_ => _.Reference.Key == metricId) :null;
                if (null != bizGrade)
                {
                    retour = bizGrade.DetailResult.Grade;
                }
                else 
                {
                    #region qalDisGrade

                    var qalDisGrade =snapshot.QualityDistributionsResults !=null? snapshot.QualityDistributionsResults.SingleOrDefault(_ => _.Reference.Key == metricId):null;
                    if (null != qalDisGrade)
                    {
                        retour = qalDisGrade.DetailResult.Grade;
                    }
                    else 
                    {
                        #region qalMesGrade

                        var qalMesGrade = snapshot.QualityMeasuresResults!=null?snapshot.QualityMeasuresResults.SingleOrDefault(_ => _.Reference.Key == metricId):null;
                        if (null != qalMesGrade)
                        {
                            retour = qalMesGrade.DetailResult.Grade;
                        }
                        else 
                        {
                            #region qalRulGrade

                            var qalRulGrade = snapshot.QualityRulesResults != null ? snapshot.QualityRulesResults.SingleOrDefault(_ => _.Reference.Key == metricId) : null;
                            if (null != qalRulGrade)
                            {
                                retour = qalRulGrade.DetailResult.Grade;
                            }
                            else 
                            {
                                #region tecCrtGrade
                                var tecCrtGrade = snapshot.TechnicalCriteriaResults !=null?snapshot.TechnicalCriteriaResults.SingleOrDefault(_ => _.Reference.Key == metricId):null;
                                if (null != tecCrtGrade)
                                {
                                    retour = tecCrtGrade.DetailResult.Grade;
                                }
                                else 
                                {
                                    #region sizIndic

                                    var sizIndic =snapshot.SizingMeasuresResults!=null? snapshot.SizingMeasuresResults.SingleOrDefault(_ => _.Reference.Key == metricId):null;
                                    if (null != sizIndic)
                                    {
                                        retour = sizIndic.DetailResult.Value;
                                    }
                                    else 
                                    {
                                        var ccgrade = snapshot.CostComplexityResults != null ? snapshot.CostComplexityResults.SingleOrDefault(_ => _.Reference.Key == metricId) : null;
                                        if (null != ccgrade)
                                        {
                                            retour = ccgrade.DetailResult.Grade;
                                        }
                                    }
                                    #endregion sizIndic
                                }
                                #endregion tecCrtGrade
                            }
                            #endregion qalRulGrade
                        }
                        #endregion qalMesGrade
                    }
                    #endregion qalDisGrade
                }
            }
            return retour;
        }

       
        /// <summary>
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="businessCriterias"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<TechnicalCriteriaResultDTO> GetTechnicalCriteriasByBusinessCriterias(Snapshot snapshot, IEnumerable<string> businessCriterias, int count)
        {
            var listBCs = businessCriterias as IList<string> ?? businessCriterias.ToList();
            if (snapshot?.QIBusinessCriterias == null || listBCs.Count == 0 || !listBCs.Any())
                return null;
            List<TechnicalCriteriaResultDTO> technicalCriteriasResults = null;
                
            var technicalCriterias = snapshot.QIBusinessCriterias.Where(_ => listBCs.Contains(_.Key.ToString()) && _.Contributors != null)
                .SelectMany(_ => _.Contributors)
                .Select(_ => _.Key).ToList();
               
            if(technicalCriterias.Count !=0)
                technicalCriteriasResults = (from result in snapshot.TechnicalCriteriaResults
                        where technicalCriterias.Contains(result.Reference.Key) && result.DetailResult!=null
                        select new TechnicalCriteriaResultDTO { Key = result.Reference.Key, Name = result.Reference.Name, Grade =MathUtility.GetRound( result.DetailResult.Grade) })
                    .OrderBy(_ => _.Name)
                    .ToList();

            if (count > 0)
            {
                technicalCriteriasResults = technicalCriteriasResults?.Take(count).ToList();
            }

            return technicalCriteriasResults;
        }
        
        #endregion METHODS
    }
}
