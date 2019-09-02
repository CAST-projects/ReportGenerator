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

using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cast.Util.Version;
using Cast.Util.Log;

namespace CastReporting.BLL
{
    public class PortfolioSnapshotsBLL : BaseBLL
    {
        /// <summary>
        /// 
        /// </summary>
        protected Snapshot[] Snapshots;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="snapshots"></param>
        public PortfolioSnapshotsBLL(WSConnection connection, Snapshot[] snapshots)
            : base(connection)
        {
            Snapshots = snapshots;
        }



        /// <summary>
        /// 
        /// </summary>
        public List<string> SetQualityIndicators()
        {
            List<string> _ignore_snaps = new List<string>();
            const string qualityIndicators = "business-criteria,technical-criteria,quality-rules,quality-distributions,quality-measures";

            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length > 0)
                {
                    foreach (Snapshot snap in Snapshots)
                    {
                        try
                        {
                            var qualityIndicatorsResults = castRepsitory.GetResultsQualityIndicators(snap.Href, qualityIndicators, string.Empty, "$all", "$all")
                                .Where(_ => _.ApplicationResults != null)
                                .SelectMany(_ => _.ApplicationResults)
                                .ToList();


                            snap.BusinessCriteriaResults = qualityIndicatorsResults.Where(_ => _.Type == "business-criteria").ToList();
                            snap.QualityDistributionsResults = qualityIndicatorsResults.Where(_ => _.Type == "quality-distributions").ToList();
                            snap.QualityMeasuresResults = qualityIndicatorsResults.Where(_ => _.Type == "quality-measures").ToList();
                            snap.QualityRulesResults = qualityIndicatorsResults.Where(_ => _.Type == "quality-rules").ToList();
                            snap.TechnicalCriteriaResults = qualityIndicatorsResults.Where(_ => _.Type == "technical-criteria").ToList();
                        }
                        catch (WebException ex)
                        {
                            LogHelper.LogInfo(ex.Message);
                            _ignore_snaps.Add(snap.Href);
                        }
                    }
                }
            }


            IEnumerable<string> _ignore_setBusinessCriteriaCCRulesViolations = SetBusinessCriteriaCCRulesViolations();
            IEnumerable<string> _ignore_setBusinessCriteriaNCRulesViolations = SetBusinessCriteriaNCRulesViolations();
            IEnumerable<string> _ignore_setTechnicalCriteriaRulesViolations = SetTechnicalCriteriaRulesViolations();

            var _all = _ignore_snaps.Concat(_ignore_setBusinessCriteriaCCRulesViolations).Concat(_ignore_setBusinessCriteriaNCRulesViolations).Concat(_ignore_setTechnicalCriteriaRulesViolations).ToList();

            return _all;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<string> SetModules()
        {
            List<string> _ignoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignoreSnaps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        snap.Modules = castRepsitory.GetModules(snap.Href);
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignoreSnaps.Add(snap.Href);
                    }
                }
            }
            return _ignoreSnaps;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SetSizingMeasure()
        {
            List<string> _ignoreApps = new List<string>();

            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignoreApps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        if (VersionUtil.IsAdgVersion82Compliant(snap.AdgVersion))
                        {
                            const string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                            snap.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(snap.Href, strSizingMeasures, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                        }
                        else
                        {
                            const string strSizingMeasureOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                            snap.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(snap.Href, strSizingMeasureOld, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                        }

                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignoreApps.Add(snap.Href);
                    }
                }
            }
            return _ignoreApps;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>A verifier s'il faut utiliser la conf</remarks>
        public List<string> SetConfigurationBusinessCriterias()
        {
            List<string> _ignoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignoreSnaps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        snap.QIBusinessCriterias = castRepsitory.GetConfBusinessCriteriaBySnapshot(snap.DomainId, snap.Id);
                        List<QIBusinessCriteria> fullQibusinesCriterias = snap.QIBusinessCriterias
                            .Select(qiBusinessCriteria => castRepsitory.GetConfBusinessCriteria(qiBusinessCriteria.HRef))
                            .ToList();
                        snap.QIBusinessCriterias = fullQibusinesCriterias;
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignoreSnaps.Add(snap.Href);
                    }
                }
            }
            return _ignoreSnaps;
        }

     

        /// <summary>
        /// 
        /// </summary>
        public List<string> SetComplexity()
        {
            List<string> _ignoreSnaps = new List<string>();
            var values = (int[])Enum.GetValues(typeof(Constants.QualityDistribution));

            List<ApplicationResult> results = new List<ApplicationResult>();

            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignoreSnaps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        foreach (int val in values)
                        {
                            var appResults = castRepsitory.GetComplexityIndicators(snap.Href, val.ToString());
                            foreach (var result in appResults)
                            {
                                results.AddRange(result.ApplicationResults);
                            }
                        }
                        snap.CostComplexityResults = results;
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignoreSnaps.Add(snap.Href);
                    }
                }
            }
            return _ignoreSnaps;
        }



        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> SetBusinessCriteriaCCRulesViolations()
        {
            List<string> _ignoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignoreSnaps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        foreach (var businessCriteria in snap.BusinessCriteriaResults)
                        {
                            var results = castRepsitory.GetRulesViolations(snap.Href, "cc", businessCriteria.Reference.Key.ToString());

                            businessCriteria.CriticalRulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignoreSnaps.Add(snap.Href);
                    }
                }
            }
            return _ignoreSnaps;
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> SetBusinessCriteriaNCRulesViolations()
        {
            List<string> _ignore_snaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignore_snaps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        foreach (var businessCriteria in snap.BusinessCriteriaResults)
                        {
                            var results = castRepsitory.GetRulesViolations(snap.Href, "nc", businessCriteria.Reference.Key.ToString());

                            businessCriteria.NonCriticalRulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignore_snaps.Add(snap.Href);
                    }
                }
            }
            return _ignore_snaps;
        }


        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> SetTechnicalCriteriaRulesViolations()
        {
            List<string> _ignoreSnaps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (Snapshots.Length <= 0) return _ignoreSnaps;
                foreach (Snapshot snap in Snapshots)
                {
                    try
                    {
                        foreach (var technicalCriteria in snap.TechnicalCriteriaResults)
                        {
                            var results = castRepsitory.GetRulesViolations(snap.Href, "c", technicalCriteria.Reference.Key.ToString());

                            technicalCriteria.RulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        _ignoreSnaps.Add(snap.Href);
                    }
                }
            }
            return _ignoreSnaps;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="snapshot"></param>
        /// <param name="withActionPlan"></param>
        /// <returns></returns>
        public static string[] BuildSnapshotResult(WSConnection connection, Snapshot[] snapshot, bool withActionPlan)
        {
            //Build modules
            using (PortfolioSnapshotsBLL snapshotBll = new PortfolioSnapshotsBLL(connection, snapshot))
            {
                List<string> _snaps_setModules = snapshotBll.SetModules();
                List<string> _snaps_setQualityIndicators = snapshotBll.SetQualityIndicators();
                List<string> _snaps_setSizingMeasure = snapshotBll.SetSizingMeasure();
                List<string> _snaps_setConfigurationBusinessCriterias = snapshotBll.SetConfigurationBusinessCriterias();
                List<string> _snaps_setComplexity = snapshotBll.SetComplexity();

                string[] _allSnapsToIgnore = _snaps_setComplexity
                    .Concat(_snaps_setConfigurationBusinessCriterias)
                    .Concat(_snaps_setModules)
                    .Concat(_snaps_setQualityIndicators)
                    .Concat(_snaps_setSizingMeasure)
                    .ToArray();

                return _allSnapsToIgnore;
            }
        }


    }
}
