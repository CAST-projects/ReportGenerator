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
        private BusinessCriteriaUtility()
        {
            // Avoid instanciation of the class
        }

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
        public static double? GetMetricValue(Snapshot snapshot, int metricId)
        {
            if (null == snapshot) return null;

            var bizGrade = snapshot.BusinessCriteriaResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != bizGrade)
            {
                return bizGrade.DetailResult.Grade;
            }

            var qalDisGrade =snapshot.QualityDistributionsResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != qalDisGrade)
            {
                return qalDisGrade.DetailResult.Grade;
            }

            var qalMesGrade = snapshot.QualityMeasuresResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != qalMesGrade)
            {
                return qalMesGrade.DetailResult.Grade;
            }

            var qalRulGrade = snapshot.QualityRulesResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != qalRulGrade)
            {
                return qalRulGrade.DetailResult.Grade;
            }

            var tecCrtGrade = snapshot.TechnicalCriteriaResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != tecCrtGrade)
            {
                return tecCrtGrade.DetailResult.Grade;
            }

            var sizIndic =snapshot.SizingMeasuresResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != sizIndic)
            {
                return sizIndic.DetailResult.Value;
            }

            var ccgrade = snapshot.CostComplexityResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            // ReSharper disable once UseNullPropagation
            if (null != ccgrade)
            {
                return ccgrade.DetailResult.Grade;
            }

            return null;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static string GetMetricName(Snapshot snapshot, int metricId, bool shortName = false)
        {
            if (null == snapshot) return null;

            var bizGrade = snapshot.BusinessCriteriaResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != bizGrade)
            {
                if (shortName & bizGrade.Reference.ShortName != null)
                    return bizGrade.Reference.ShortName;
                return bizGrade.Reference.Name ;
            }

            var qalDisGrade = snapshot.QualityDistributionsResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != qalDisGrade)
            {
                if (shortName & qalDisGrade.Reference.ShortName != null)
                    return qalDisGrade.Reference.ShortName;
                return qalDisGrade.Reference.Name;
            }

            var qalMesGrade = snapshot.QualityMeasuresResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != qalMesGrade)
            {
                if (shortName & qalMesGrade.Reference.ShortName != null)
                    return qalMesGrade.Reference.ShortName;
                return qalMesGrade.Reference.Name;
            }

            var qalRulGrade = snapshot.QualityRulesResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != qalRulGrade)
            {
                if (shortName & qalRulGrade.Reference.ShortName != null)
                    return qalRulGrade.Reference.ShortName;
                return qalRulGrade.Reference.Name;
            }

            var tecCrtGrade = snapshot.TechnicalCriteriaResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != tecCrtGrade)
            {
                if (shortName & tecCrtGrade.Reference.ShortName != null)
                    return tecCrtGrade.Reference.ShortName;
                return tecCrtGrade.Reference.Name;
            }

            var sizIndic = snapshot.SizingMeasuresResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            if (null != sizIndic)
            {
                if (shortName & sizIndic.Reference.ShortName != null)
                    return sizIndic.Reference.ShortName;
                return sizIndic.Reference.Name;
            }

            var ccgrade = snapshot.CostComplexityResults?.SingleOrDefault(_ => _.Reference.Key == metricId);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (null != ccgrade)
            {
                if (shortName & ccgrade.Reference.ShortName != null)
                    return ccgrade.Reference.ShortName;
                return ccgrade.Reference.Name;
            }

            return string.Empty;

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
