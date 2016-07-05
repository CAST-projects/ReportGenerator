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

using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CastReporting.BLL
{
    public class PortfolioSnapshotsBLL : BaseBLL
    {
        /// <summary>
        /// 
        /// </summary>
        Snapshot[] _Snapshot;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSnapshot"></param>
        /// <param name="previousSnapshot"></param>
        public PortfolioSnapshotsBLL(WSConnection connection, Snapshot[] snapshot)
            : base(connection)
        {
            _Snapshot = snapshot;
        }



        /// <summary>
        /// 
        /// </summary>
        public void SetQualityIndicators()
        {
            Int32[] businessCriterias = (Int32[])Enum.GetValues(typeof(Constants.BusinessCriteria));

            string strBusinessCriterias = string.Join(",", businessCriterias);

            Int32[] qualityDistribution = (Int32[])Enum.GetValues(typeof(Constants.QualityDistribution));

            string strQualityDistribution = string.Join(",", qualityDistribution);

            string qualityMeasure = "quality-measures";
            string qualityRules = "quality-rules";
            string technicalCriterias = "technical-criteria";

            string qualityParams = string.Format("{0},{1},{2},{3},{4}", strBusinessCriterias, strQualityDistribution, qualityMeasure, qualityRules, technicalCriterias);

            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        var qualityIndicators = castRepsitory.GetResultsQualityIndicators(_Snapshot[i].Href, qualityParams, string.Empty, "$all", "$all", "$all")
                                                                              .Where(_ => _.ApplicationResults != null)
                                                                              .SelectMany(_ => _.ApplicationResults)
                                                                              .ToList();


                        _Snapshot[i].BusinessCriteriaResults = qualityIndicators.Where(_ => _.Type == "business-criteria").ToList();
                        _Snapshot[i].QualityDistributionsResults = qualityIndicators.Where(_ => _.Type == "quality-distributions").ToList();
                        _Snapshot[i].QualityMeasuresResults = qualityIndicators.Where(_ => _.Type == "quality-measures").ToList();
                        _Snapshot[i].QualityRulesResults = qualityIndicators.Where(_ => _.Type == "quality-rules").ToList();
                        _Snapshot[i].TechnicalCriteriaResults = qualityIndicators.Where(_ => _.Type == "technical-criteria").ToList();
                    }
                }
            }

            SetBusinessCriteriaCCRulesViolations();
            SetBusinessCriteriaNCRulesViolations();
            SetTechnicalCriteriaRulesViolations();
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetModules()
        {
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        _Snapshot[i].Modules = castRepsitory.GetModules(_Snapshot[i].Href);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetSizingMeasure()
        {
            Int32[] sizingMeasures = (Int32[])Enum.GetValues(typeof(Constants.SizingInformations));
            string strSizingMeasures = string.Join(",", sizingMeasures);

            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        _Snapshot[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot[i].Href, strSizingMeasures, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>A verifier s'il faut utiliser la conf</remarks>
        public void SetConfigurationBusinessCriterias()
        {
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        _Snapshot[i].QIBusinessCriterias = castRepsitory.GetConfBusinessCriteriaBySnapshot(_Snapshot[i].DomainId, _Snapshot[i].Id);

                        List<QIBusinessCriteria> fullQibusinesCriterias = new List<QIBusinessCriteria>();

                        foreach (var QIBusinessCriteria in _Snapshot[i].QIBusinessCriterias)
                        {
                            fullQibusinesCriterias.Add(castRepsitory.GetConfBusinessCriteria(QIBusinessCriteria.HRef));

                        }

                        _Snapshot[i].QIBusinessCriterias = fullQibusinesCriterias;
                    }
                }
            }
           
        }

     

        /// <summary>
        /// 
        /// </summary>
        public void SetComplexity()
        {
            var values = (int[])Enum.GetValues(typeof(Constants.QualityDistribution));

            List<ApplicationResult> results = new List<ApplicationResult>();

            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int j = 0; j < _Snapshot.Count(); j++)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            var appResults = castRepsitory.GetComplexityIndicators(_Snapshot[j].Href, values[i].ToString());
                            foreach (var result in appResults)
                            {
                                foreach (var appResult in result.ApplicationResults)
                                {
                                    results.Add(appResult);
                                }

                            }
                        }
                        _Snapshot[j].CostComplexityResults = results;
                    }
                }
            }

        }



        /// <summary>
        /// 
        /// </summary>
        public void SetActionsPlan()
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    if (_Snapshot.Count() > 0)
                    {
                        for (int i = 0; i < _Snapshot.Count(); i++)
                        {
                            _Snapshot[i].ActionsPlan = castRepsitory.GetActionPlanBySnapshot(_Snapshot[i].Href);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        _Snapshot[i].ActionsPlan = null;
                    }
                }
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="snapshotHref"></param>
        ///// <param name="businessCriteria"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count)
        //{
        //    try
        //    {
        //        using (var castRepsitory = GetRepository())
        //        {
        //            return castRepsitory.GetTransactions(_Snapshot.Href, businessCriteria, count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {                
        //        return null;
        //    }
            
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="snapshotHref"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        ///// 
        //public IEnumerable<CommonCategories> GetCommonCategories(WSConnection Connection)
        //{
        //    try
        //    {
        //        using (var castRepsitory = GetRepository(Connection))
        //        {
        //            return castRepsitory.GetCommonCategories();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        //public string GetCommonCategoriesJson(WSConnection Connection)
        //{
        //    try
        //    {
        //        using (var castRepsitory = GetRepository(Connection))
        //        {
        //            return castRepsitory.GetCommonCategoriesJson();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="snapshotHref"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count)
        //{
        //    try
        //    {
        //        using (var castRepsitory = GetRepository())
        //        {
        //            return castRepsitory.GetIfpugFunctions(_Snapshot.Href, count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        //public IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count)
        //{
        //    try
        //    {
        //        using (var castRepsitory = GetRepository())
        //        {
        //            return castRepsitory.GetIfpugFunctionsEvolutions(_Snapshot.Href, count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="snapshotHref"></param>
        ///// <param name="RuleId"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public IEnumerable<CastReporting.Domain.MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string RuleId, int count)
        //{
        //    try
        //    {
        //        using (var castRepsitory = GetRepository())
        //        {
        //            return castRepsitory.GetMetricTopArtefact(_Snapshot.Href, RuleId, count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="snapshotHref"></param>
        ///// <param name="businessCriteria"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count)
        //{

        //    try
        //    {
        //        using (var castRepsitory = GetRepository())
        //        {
        //            return castRepsitory.GetComponents(_Snapshot.Href, businessCriteria, count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {               
        //        return null;
        //    }
           
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="domainId"></param>
        ///// <param name="moduleId"></param>
        ///// <param name="snapshotId"></param>
        ///// <param name="businessCriteria"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        //{           
        //    try
        //    {
        //        using (var castRepsitory = GetRepository())
        //        {
        //            return castRepsitory.GetComponentsByModule(domainId, moduleId, snapshotId, businessCriteria, count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }                      
        //}

        
        /// <summary>
        /// 
        /// </summary>
        private void SetBusinessCriteriaCCRulesViolations()
        {
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        foreach (var businessCriteria in _Snapshot[i].BusinessCriteriaResults)
                        {
                            var results = castRepsitory.GetRulesViolations(_Snapshot[i].Href, "cc", businessCriteria.Reference.Key.ToString());

                            businessCriteria.CriticalRulesViolation = results.SelectMany(x => x.ApplicationResults).ToList();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetBusinessCriteriaNCRulesViolations()
        {
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        foreach (var businessCriteria in _Snapshot[i].BusinessCriteriaResults)
                        {
                            var results = castRepsitory.GetRulesViolations(_Snapshot[i].Href, "nc", businessCriteria.Reference.Key.ToString());

                            businessCriteria.NonCriticalRulesViolation = results.SelectMany(x => x.ApplicationResults).ToList();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetTechnicalCriteriaRulesViolations()
        {
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        foreach (var technicalCriteria in _Snapshot[i].TechnicalCriteriaResults)
                        {

                            var results = castRepsitory.GetRulesViolations(_Snapshot[i].Href, "c", technicalCriteria.Reference.Key.ToString());


                            technicalCriteria.RulesViolation = results.SelectMany(x => x.ApplicationResults).ToList();
                        }
                    }
                }
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        static public void BuildSnapshotResult(WSConnection connection, Snapshot[] snapshot, bool withActionPlan)
        {
            //Build modules
            using (PortfolioSnapshotsBLL snapshotBll = new PortfolioSnapshotsBLL(connection, snapshot))
            {
                snapshotBll.SetModules();
                snapshotBll.SetQualityIndicators();
                snapshotBll.SetSizingMeasure();
                snapshotBll.SetConfigurationBusinessCriterias();
                snapshotBll.SetComplexity();
                //snapshotBll.SetActionsPlan();


                //Task taskModules = new Task(() => snapshotBll.SetModules());
                //taskModules.Start();

                ////Build Quality Indicators
                //Task taskQualityIndicators = new Task(() => snapshotBll.SetQualityIndicators());
                //taskQualityIndicators.Start();



                ////Build Sizing Measures
                //Task taskSizingMeasure = new Task(() => snapshotBll.SetSizingMeasure());
                //taskSizingMeasure.Start();

                ////Build Configuration for Business Criteria
                //Task taskConfigurationBusinessCriterias = new Task(() => snapshotBll.SetConfigurationBusinessCriterias());
                //taskConfigurationBusinessCriterias.Start();


                ////Build Configuration for Business Criteria
                //Task taskComplexity = new Task(() => snapshotBll.SetComplexity());
                //taskComplexity.Start();


                ////build action plan
                //Task taskAP = null;
                //if (withActionPlan)
                //{
                //    taskAP = new Task(() => snapshotBll.SetActionsPlan());
                //    taskAP.Start();
                //}


                //taskModules.Wait();
                //taskQualityIndicators.Wait();
                //taskSizingMeasure.Wait();
                //taskConfigurationBusinessCriterias.Wait();
                //taskComplexity.Wait();
                //if (taskAP != null) taskAP.Wait();
            }
        }

       
    }
}
