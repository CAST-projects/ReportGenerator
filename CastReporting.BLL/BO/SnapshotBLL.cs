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

using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cast.Util.Version;
using Cast.Util.Log;
// ReSharper disable AccessToDisposedClosure

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class SnapshotBLL : BaseBLL, ISnapshotExplorer
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        Snapshot _Snapshot;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="snapshot"></param>
        public SnapshotBLL(WSConnection connection, Snapshot snapshot)
            : base(connection)
        {
            _Snapshot = snapshot;
        }



        /// <summary>
        /// 
        /// </summary>
        public void SetQualityIndicators()
        {
            const string qualityIndicators = "business-criteria,technical-criteria,quality-rules,quality-distributions,quality-measures";

            using (var castRepsitory = GetRepository())
            {
                var qualityIndicatorsResults = castRepsitory.GetResultsQualityIndicators(_Snapshot.Href, qualityIndicators, string.Empty, "$all", "$all", "$all")
                                                                      .Where(_ => _.ApplicationResults != null)
                                                                      .SelectMany(_ => _.ApplicationResults)
                                                                      .ToList();
            


                var businessCriteriaResults = new List<ApplicationResult>();
                var qualityDistributionsResults = new List<ApplicationResult>();
                var qualityMeasuresResults = new List<ApplicationResult>();
                var qualityRulesResults = new List<ApplicationResult>();
                var technicalCriteriaResults = new List<ApplicationResult>();

                foreach (var appRes in qualityIndicatorsResults) {
                    switch (appRes.Type) {
                        case "business-criteria":       businessCriteriaResults.Add(appRes); break;
                        case "quality-distributions":   qualityDistributionsResults.Add(appRes); break;
                        case "quality-measures":        qualityMeasuresResults.Add(appRes); break;
                        case "quality-rules":           qualityRulesResults.Add(appRes); break;
                        case "technical-criteria":      technicalCriteriaResults.Add(appRes); break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }

                _Snapshot.BusinessCriteriaResults = businessCriteriaResults;
                _Snapshot.QualityDistributionsResults = qualityDistributionsResults;
                _Snapshot.QualityMeasuresResults = qualityMeasuresResults;
                _Snapshot.QualityRulesResults = qualityRulesResults;
                _Snapshot.TechnicalCriteriaResults = technicalCriteriaResults;
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
                _Snapshot.Modules = castRepsitory.GetModules(_Snapshot.Href);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetSizingMeasure()
        {
            using (var castRepsitory = GetRepository())
            {
                try
                {
                    if (VersionUtil.IsAdgVersion82Compliant(_Snapshot.AdgVersion))
                    {
                        const string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                        _Snapshot.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot.Href, strSizingMeasures, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                    }
                    else
                    {
                        const string strSizingMeasuresOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                        _Snapshot.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot.Href, strSizingMeasuresOld, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                    }
                }
                catch (System.Net.WebException ex)
                {
                    LogHelper.Instance.LogInfo(ex.Message);
                    const string strSizingMeasuresOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                    _Snapshot.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot.Href, strSizingMeasuresOld, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
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
                _Snapshot.QIBusinessCriterias = castRepsitory.GetConfBusinessCriteriaBySnapshot(_Snapshot.DomainId, _Snapshot.Id);

                List<QIBusinessCriteria> fullQibusinesCriterias = _Snapshot.QIBusinessCriterias.Select(_ => castRepsitory.GetConfBusinessCriteria(_.HRef)).ToList();

                _Snapshot.QIBusinessCriterias = fullQibusinesCriterias;
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
                foreach (int val in values)
                {
                    var appResults = castRepsitory.GetComplexityIndicators(_Snapshot.Href, val.ToString());
                    foreach (var result in appResults)
                    {
                        results.AddRange(result.ApplicationResults);
                    }
                }
            }

            _Snapshot.CostComplexityResults = results;
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
                    _Snapshot.ActionsPlan = castRepsitory.GetActionPlanBySnapshot(_Snapshot.Href);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                _Snapshot.ActionsPlan = null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetTransactions(_Snapshot.Href, businessCriteria, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }
            
        }

        public IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetResultsBackgroundFacts(snapshotHref, backgroundFacts, string.Empty, string.Empty, string.Empty);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts, bool modules, bool technologies)
        {
            string modParam = string.Empty;
            if (modules) modParam = "$all";

            string technoParam = string.Empty;
            if (technologies) technoParam = "$all";

            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetResultsBackgroundFacts(snapshotHref, backgroundFacts, string.Empty, technoParam, modParam);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetSizingMeasureResults(string snapshotHref, string sizingMeasure)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetResultsSizingMeasures(snapshotHref, sizingMeasure, string.Empty, string.Empty, string.Empty);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetQualityIndicatorResults(string snapshotHref, string qualityIndicator)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetResultsQualityIndicators(snapshotHref, qualityIndicator, string.Empty, string.Empty, string.Empty,string.Empty);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<CommonCategories> GetCommonCategories(WSConnection connection)
        {
            try
            {
                using (var castRepsitory = GetRepository(connection))
                {
                    return castRepsitory.GetCommonCategories();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        public string GetCommonCategoriesJson(WSConnection connection)
        {
            try
            {
                using (var castRepsitory = GetRepository(connection))
                {
                    return castRepsitory.GetCommonCategoriesJson();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetIfpugFunctions(_Snapshot.Href, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetIfpugFunctionsEvolutions(_Snapshot.Href, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="ruleId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string ruleId, int count)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetMetricTopArtefact(_Snapshot.Href, ruleId, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="ruleId"></param>
        /// <param name="bcId"></param>
        /// <param name="count"></param>
        /// <param name="technos"></param>
        /// <returns></returns>
        public IEnumerable<Violation> GetViolationsListIDbyBC(string snapshotHref, string ruleId, string bcId, int count, string technos)
        {
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetViolationsListIDbyBC(snapshotHref, ruleId, bcId, count, technos);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Violation> GetViolationsInActionPlan(string snapshotHref, int count)
        {
            try
            {
                using (var castRepository = GetRepository())
                {
                    return castRepository.GetViolationsInActionPlan(snapshotHref, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count)
        {

            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetComponents(_Snapshot.Href, businessCriteria, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="moduleId"></param>
        /// <param name="snapshotId"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        {           
            try
            {
                using (var castRepsitory = GetRepository())
                {
                    return castRepsitory.GetComponentsByModule(domainId, moduleId, snapshotId, businessCriteria, count);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                return null;
            }                      
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void SetBusinessCriteriaCCRulesViolations()
        {
            using (var castRepsitory = GetRepository())
            {
                foreach (var businessCriteria in _Snapshot.BusinessCriteriaResults)
                {
                    var results = castRepsitory.GetRulesViolations(_Snapshot.Href, "cc", businessCriteria.Reference.Key.ToString());

                    businessCriteria.CriticalRulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
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
                foreach (var businessCriteria in _Snapshot.BusinessCriteriaResults)
                {
                    var results = castRepsitory.GetRulesViolations(_Snapshot.Href, "nc", businessCriteria.Reference.Key.ToString());

                    businessCriteria.NonCriticalRulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
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
                foreach (var technicalCriteria in _Snapshot.TechnicalCriteriaResults)
                {

                    var results = castRepsitory.GetRulesViolations(_Snapshot.Href, "c", technicalCriteria.Reference.Key.ToString());


                    technicalCriteria.RulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
                }
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="connection"></param>
        /// <param name="withActionPlan"></param>
        /// <returns></returns>
        public static void BuildSnapshotResult(WSConnection connection, Snapshot snapshot, bool withActionPlan)
        {
            //Build modules
            using (SnapshotBLL snapshotBll = new SnapshotBLL(connection, snapshot))
            {
                Task taskModules = new Task(() => snapshotBll.SetModules());
                taskModules.Start();

                //Build Quality Indicators
                Task taskQualityIndicators = new Task(() => snapshotBll.SetQualityIndicators());
                taskQualityIndicators.Start();

                //Build Sizing Measures
                Task taskSizingMeasure = new Task(() => snapshotBll.SetSizingMeasure());
                taskSizingMeasure.Start();

                //Build Configuration for Business Criteria
                Task taskConfigurationBusinessCriterias = new Task(() => snapshotBll.SetConfigurationBusinessCriterias());
                taskConfigurationBusinessCriterias.Start();

                taskModules.Wait();
                taskQualityIndicators.Wait();
                taskSizingMeasure.Wait();
                taskConfigurationBusinessCriterias.Wait();

                //Build Configuration for Business Criteria
                Task taskComplexity = new Task(() => snapshotBll.SetComplexity());
                taskComplexity.Start();


                //build action plan
                // ReSharper disable once InconsistentNaming
                Task taskAP = null;
                if (withActionPlan)
                {
                    taskAP = new Task(() => snapshotBll.SetActionsPlan());
                    taskAP.Start();
                }

                taskComplexity.Wait();
                taskAP?.Wait();
            }
        }

       
    }
}
