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
using System.Globalization;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Domain.Interfaces;

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
        public static List<BusinessCriteriaDTO> GetBusinessCriteriaGradesModules(Snapshot snapshot, bool Round)
        {

            if (snapshot != null && snapshot.BusinessCriteriaResults != null)
            {
                List<BusinessCriteriaDTO> result = new List<BusinessCriteriaDTO>();

                var modules = snapshot.BusinessCriteriaResults.SelectMany(_ => _.ModulesResult).Select(_ => _.Module).Distinct();

                result = modules.Select(module => new BusinessCriteriaDTO
                                                {
                                                        Name = module.Name,

                                                        TQI = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.TechnicalQualityIndex, Round),

                                                        Robustness = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Robustness, Round),

                                                        Performance = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Performance, Round),

                                                        Security = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Security, Round),

                                                        Changeability = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Changeability, Round),

                                                        Transferability = GetBusinessCriteriaModuleGrade(snapshot, module.Href, Constants.BusinessCriteria.Transferability, Round),
                                                                                    })
                                .ToList();
              
                return result;
            }
            return null;
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static BusinessCriteriaDTO GetBusinessCriteriaGradesSnapshot(Snapshot snapshot, bool Round)
        {

            if (null != snapshot)
            {
                BusinessCriteriaDTO result = new BusinessCriteriaDTO();
                              
                result.TQI = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.TechnicalQualityIndex, Round);
                result.Robustness = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Robustness, Round);
                result.Performance = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Performance, Round);
                result.Security = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Security, Round);
                result.Transferability = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Transferability, Round);
                result.Changeability = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Changeability, Round);
                result.ProgrammingPractices = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.ProgrammingPractices, Round);
                result.ArchitecturalDesign = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.ArchitecturalDesign, Round);
                result.Documentation = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.Documentation, Round);
                result.SEIMaintainability = GetSnapshotBusinessCriteriaGrade(snapshot, Constants.BusinessCriteria.SEIMaintainability, Round);

                return result;
            }
            return null;
        }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="snapshot"></param>
       /// <param name="moduleHRef"></param>
       /// <param name="bcId"></param>
       /// <returns></returns>
        public static Double? GetBusinessCriteriaModuleGrade(Snapshot snapshot, string moduleHRef, Constants.BusinessCriteria bcId, bool Round)
        {
                        double? res = null;
            if (null != snapshot && null != snapshot.BusinessCriteriaResults)
            {
                var result = snapshot.BusinessCriteriaResults
                           .Where(_ => _.Reference.Key == (Int32)bcId && _.ModulesResult != null)
                           .SelectMany(_ => _.ModulesResult)
                           .Where(_ => _.Module.Href == moduleHRef && _.DetailResult != null )
                           .Select(_ => _.DetailResult.Grade)
                           .FirstOrDefault();
                if (result != null)
                {
                    res = Round ? MathUtility.GetRound(result) : result;
                }
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="moduleHRef"></param>
        /// <param name="bcId"></param>
        /// <returns></returns>
        public static Double? GetBusinessCriteriaModuleGrade(Snapshot snapshot, Int32 moduleId, Constants.BusinessCriteria bcId, bool Round)
        {
            double? res = null;
            if (null != snapshot && null != snapshot.BusinessCriteriaResults)
            {
                var result = snapshot.BusinessCriteriaResults
                           .Where(_ => _.Reference.Key == (Int32)bcId && _.ModulesResult != null)
                           .SelectMany(_ => _.ModulesResult)
                           .Where(_ => _.Module.Id == moduleId && _.DetailResult != null)
                           .Select(_ => _.DetailResult.Grade)
                           .FirstOrDefault();
                if (result != null)
                {
                    res = Round ? MathUtility.GetRound(result) : result;
                }
            }
            return res;
        }


        /// <summary>
        /// Get TQI for snapshot
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetSnapshotBusinessCriteriaGrade(Snapshot snapshot, Constants.BusinessCriteria bcId, bool Round)
        {
            double? res = null;
            if (null != snapshot && null != snapshot.BusinessCriteriaResults)
            {
                var resultBC = snapshot.BusinessCriteriaResults.SingleOrDefault(_ => _.Reference.Key == bcId.GetHashCode());
                if (resultBC != null)
                {
                    res = Round ? MathUtility.GetRound(resultBC.DetailResult.Grade) : resultBC.DetailResult.Grade;
                }
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

       
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="businessCriterias"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<TechnicalCriteriaResultDTO> GetTechnicalCriteriasByBusinessCriterias(Snapshot snapshot, IEnumerable<string> businessCriterias, int count)
        {
            if (snapshot != null && snapshot.QIBusinessCriterias != null && businessCriterias != null && businessCriterias.Count() != 0)
            {
                List<TechnicalCriteriaResultDTO> technicalCriteriasResults = null;
                
                var technicalCriterias = snapshot.QIBusinessCriterias.Where(_ => businessCriterias.Contains(_.Key.ToString()) && _.Contributors != null)
                                                                           .SelectMany(_ => _.Contributors)
                                                                           .Select(_ => _.Key).ToList();
               
                if(technicalCriterias != null && technicalCriterias.Count !=0)
                   technicalCriteriasResults = (from result in snapshot.TechnicalCriteriaResults
                                                where technicalCriterias.Contains(result.Reference.Key) && result.DetailResult!=null
                                                select new TechnicalCriteriaResultDTO { Key = result.Reference.Key, Name = result.Reference.Name, Grade =MathUtility.GetRound( result.DetailResult.Grade) })
                                               .OrderBy(_ => _.Name)
                                               .ToList();

                if (count > 0 && technicalCriteriasResults !=null)
                {
                    technicalCriteriasResults = technicalCriteriasResults.Take(count).ToList();
                }

                return technicalCriteriasResults;
            }
            return null;
        }
        
        #endregion METHODS
    }
}
