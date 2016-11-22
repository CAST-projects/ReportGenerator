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
using System.Net;
using Cast.Util.Version;

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
        public List<string> SetQualityIndicators()
        {
            List<string> Ignore_Snaps = new List<string>();
            string qualityIndicators = "business-criteria,technical-criteria,quality-rules,quality-distributions,quality-measures";

            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            var qualityIndicatorsResults = castRepsitory.GetResultsQualityIndicators(_Snapshot[i].Href, qualityIndicators, string.Empty, "$all", "$all", "$all")
                                                                                  .Where(_ => _.ApplicationResults != null)
                                                                                  .SelectMany(_ => _.ApplicationResults)
                                                                                  .ToList();


                            _Snapshot[i].BusinessCriteriaResults = qualityIndicatorsResults.Where(_ => _.Type == "business-criteria").ToList();
                            _Snapshot[i].QualityDistributionsResults = qualityIndicatorsResults.Where(_ => _.Type == "quality-distributions").ToList();
                            _Snapshot[i].QualityMeasuresResults = qualityIndicatorsResults.Where(_ => _.Type == "quality-measures").ToList();
                            _Snapshot[i].QualityRulesResults = qualityIndicatorsResults.Where(_ => _.Type == "quality-rules").ToList();
                            _Snapshot[i].TechnicalCriteriaResults = qualityIndicatorsResults.Where(_ => _.Type == "technical-criteria").ToList();
                        }
                        catch (WebException ex)
                        {
                            Ignore_Snaps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }


            List<string> Ignore_SetBusinessCriteriaCCRulesViolations = SetBusinessCriteriaCCRulesViolations();
            List<string> Ignore_SetBusinessCriteriaNCRulesViolations = SetBusinessCriteriaNCRulesViolations();
            List<string> Ignore_SetTechnicalCriteriaRulesViolations = SetTechnicalCriteriaRulesViolations();

            var All = Ignore_Snaps.Concat(Ignore_SetBusinessCriteriaCCRulesViolations).Concat(Ignore_SetBusinessCriteriaNCRulesViolations).Concat(Ignore_SetTechnicalCriteriaRulesViolations).ToList();

            return All;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<string> SetModules()
        {
            List<string> IgnoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            _Snapshot[i].Modules = castRepsitory.GetModules(_Snapshot[i].Href);
                        }
                        catch (WebException ex)
                        {
                            IgnoreSnaps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }
            return IgnoreSnaps;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SetSizingMeasure()
        {
            List<string> IgnoreApps = new List<string>();

            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            if (VersionUtil.isAdgVersion82Compliant(_Snapshot[i].AdgVersion))
                            {
                                string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                                _Snapshot[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot[i].Href, strSizingMeasures, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                            }
                            else
                            {
                                string strSizingMeasureOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                                _Snapshot[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot[i].Href, strSizingMeasureOld, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                            }

                        }
                        catch (WebException ex)
                        {
                            IgnoreApps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }
            return IgnoreApps;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>A verifier s'il faut utiliser la conf</remarks>
        public List<string> SetConfigurationBusinessCriterias()
        {
            List<string> IgnoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            _Snapshot[i].QIBusinessCriterias = castRepsitory.GetConfBusinessCriteriaBySnapshot(_Snapshot[i].DomainId, _Snapshot[i].Id);
                            List<QIBusinessCriteria> fullQibusinesCriterias = new List<QIBusinessCriteria>();
                            foreach (var QIBusinessCriteria in _Snapshot[i].QIBusinessCriterias)
                            {
                                fullQibusinesCriterias.Add(castRepsitory.GetConfBusinessCriteria(QIBusinessCriteria.HRef));
                            }
                            _Snapshot[i].QIBusinessCriterias = fullQibusinesCriterias;
                        }
                        catch (WebException ex)
                        {
                            IgnoreSnaps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }
            return IgnoreSnaps;
        }

     

        /// <summary>
        /// 
        /// </summary>
        public List<string> SetComplexity()
        {
            List<string> IgnoreSnaps = new List<string>();
            var values = (int[])Enum.GetValues(typeof(Constants.QualityDistribution));

            List<ApplicationResult> results = new List<ApplicationResult>();

            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int j = 0; j < _Snapshot.Count(); j++)
                    {
                        try
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
                        catch (WebException ex)
                        {
                            IgnoreSnaps.Add(_Snapshot[j].Href);
                            continue;
                        }
                    }
                }
            }
            return IgnoreSnaps;
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


       

        
        /// <summary>
        /// 
        /// </summary>
        private List<string> SetBusinessCriteriaCCRulesViolations()
        {
            List<string> IgnoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            foreach (var businessCriteria in _Snapshot[i].BusinessCriteriaResults)
                            {
                                var results = castRepsitory.GetRulesViolations(_Snapshot[i].Href, "cc", businessCriteria.Reference.Key.ToString());

                                businessCriteria.CriticalRulesViolation = results.SelectMany(x => x.ApplicationResults).ToList();
                            }
                        }
                        catch (WebException ex)
                        {
                            IgnoreSnaps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }
            return IgnoreSnaps;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<string> SetBusinessCriteriaNCRulesViolations()
        {
            List<string> Ignore_snaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            foreach (var businessCriteria in _Snapshot[i].BusinessCriteriaResults)
                            {
                                var results = castRepsitory.GetRulesViolations(_Snapshot[i].Href, "nc", businessCriteria.Reference.Key.ToString());

                                businessCriteria.NonCriticalRulesViolation = results.SelectMany(x => x.ApplicationResults).ToList();
                            }
                        }
                        catch (WebException ex)
                        {
                            Ignore_snaps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }
            return Ignore_snaps;
        }


        /// <summary>
        /// 
        /// </summary>
        private List<string> SetTechnicalCriteriaRulesViolations()
        {
            List<string> IgnoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (_Snapshot.Count() > 0)
                {
                    for (int i = 0; i < _Snapshot.Count(); i++)
                    {
                        try
                        {
                            foreach (var technicalCriteria in _Snapshot[i].TechnicalCriteriaResults)
                            {
                                var results = castRepsitory.GetRulesViolations(_Snapshot[i].Href, "c", technicalCriteria.Reference.Key.ToString());

                                technicalCriteria.RulesViolation = results.SelectMany(x => x.ApplicationResults).ToList();
                            }
                        }
                        catch (WebException ex)
                        {
                            IgnoreSnaps.Add(_Snapshot[i].Href);
                            continue;
                        }
                    }
                }
            }
            return IgnoreSnaps;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        static public string[] BuildSnapshotResult(WSConnection connection, Snapshot[] snapshot, bool withActionPlan)
        {
            //Build modules
            using (PortfolioSnapshotsBLL snapshotBll = new PortfolioSnapshotsBLL(connection, snapshot))
            {
                List<string> Snaps_SetModules = snapshotBll.SetModules();
                List<string> Snaps_SetQualityIndicators = snapshotBll.SetQualityIndicators();
                List<string> Snaps_SetSizingMeasure = snapshotBll.SetSizingMeasure();
                List<string> Snaps_SetConfigurationBusinessCriterias = snapshotBll.SetConfigurationBusinessCriterias();
                List<string> Snaps_SetComplexity = snapshotBll.SetComplexity();

                string[] AllSnapsToIgnore = Snaps_SetComplexity.Concat(Snaps_SetConfigurationBusinessCriterias).Concat(Snaps_SetModules).Concat(Snaps_SetQualityIndicators).Concat(Snaps_SetSizingMeasure).ToArray();

                return AllSnapsToIgnore;
            }
        }


    }
}
