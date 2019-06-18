using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public static class RulesViolationUtility
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="ruleExplorer"></param>
        /// <param name="businessCriteriasIds"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<RuleViolationResultDTO> GetNbViolationByRule(Snapshot snapshot, IRuleExplorer ruleExplorer, List<int> businessCriteriasIds, int count)
        {
            if (snapshot?.TechnicalCriteriaResults == null) return null;


            //Get rules
            List<RuleDetails> rules = new List<RuleDetails>();
            foreach (var metricId in businessCriteriasIds)
            {
                rules.AddRange(ruleExplorer.GetRulesDetails(snapshot.DomainId, metricId, snapshot.Id));
            }

            rules = rules.GroupBy(_ => new { _.Key, _.Href, _.Name })
                         .Select(_ => new RuleDetails {
                                                         Key = _.Key.Key,
                                                         Href = _.Key.Href,
                                                         Name = _.Key.Name,
                                                         CompoundedWeight = _.Sum(x => x.CompoundedWeight),
                                                         Critical = _.Any(x => x.Critical)
                                                     })
                         .ToList();

            //Get result by technical criterias
            List<RuleViolationResultDTO> reslutByTechnicalCriterias = new List<RuleViolationResultDTO>();
            foreach (var rule in rules)
            {
                // ruleViolationResult instanciation can not be out outside the loop, because the Add in reslutByTechnicalCriterias is by reference and list is corrupted
                RuleViolationResultDTO ruleViolationResult = new RuleViolationResultDTO();
                var technicalCriterias = snapshot.TechnicalCriteriaResults
                                                 .FirstOrDefault(_ => _.RulesViolation!=null && _.RulesViolation.Any(p => rule.Key.HasValue && p.Reference.Key == rule.Key.Value));

                if (technicalCriterias == null) continue;

                ruleViolationResult.Rule = new RuleDetailsDTO { Key = rule.Key ?? 0, Name = rule.Name, Critical = rule.Critical, CompoundedWeight = rule.CompoundedWeight ?? 0 };
                ruleViolationResult.Grade = technicalCriterias.DetailResult.Grade;
                ruleViolationResult.TechnicalCriteraiName = technicalCriterias.Reference.Name;

                var violationRatio = technicalCriterias.RulesViolation.Where(_ => rule.Key.HasValue && _.Reference.Key == rule.Key.Value)
                    .Select(_ => _.DetailResult?.ViolationRatio)
                    .FirstOrDefault();
                if (violationRatio != null)
                {
                    ruleViolationResult.TotalFailed = violationRatio.FailedChecks;
                    ruleViolationResult.TotalChecks = violationRatio.TotalChecks;                      
                }

                reslutByTechnicalCriterias.Add(ruleViolationResult);
            }

            return count == -1 ? reslutByTechnicalCriterias.OrderBy(_ => _.Rule.Name).ToList() : reslutByTechnicalCriterias.OrderBy(_ => _.Rule.Name).Take(count).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="businessCriteriaId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<TechnicalCriteriaResultDTO> GetTechnicalCriteriaViolations(Snapshot snapshot, Constants.BusinessCriteria businessCriteriaId, int count)
        {
            if (snapshot.QIBusinessCriterias == null || snapshot.TechnicalCriteriaResults == null) return null;

            IEnumerable<int> technicalCriteriaId = snapshot.QIBusinessCriterias.Where(_ => (int)businessCriteriaId == _.Key)
                .SelectMany(_ => _.Contributors).Select( _ => _.Key);

            return snapshot.TechnicalCriteriaResults.Where(_ => technicalCriteriaId.Contains(_.Reference.Key) && _.Reference !=null && _.DetailResult != null && _.RulesViolation != null)
                .Select(_ => new TechnicalCriteriaResultDTO
                {
                    Name = _.Reference.Name, 
                    Grade = _.DetailResult.Grade,
                    TotalChecks = _.RulesViolation.Sum(r => r.DetailResult?.ViolationRatio != null ? r.DetailResult.ViolationRatio.TotalChecks : 0),
                    TotalFailed = _.RulesViolation.Sum(r => r.DetailResult?.ViolationRatio != null ? r.DetailResult.ViolationRatio.FailedChecks : 0)
                })
                .OrderByDescending(_ => _.TotalFailed)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="rulesViolationType"></param>
        /// <param name="businessCriteriaId"></param>
        /// <param name="onlyFailedChecks"></param>
        /// <param name="nbTopResult"></param>
        /// <returns></returns>
        public static List<RuleViolationResultDTO> GetRuleViolations(Snapshot snapshot,
                                                                        Constants.RulesViolation rulesViolationType,
                                                                        Constants.BusinessCriteria businessCriteriaId,
                                                                        bool onlyFailedChecks,
                                                                        int nbTopResult)
        {            
          
            var query = GetQueryRuleViolations(snapshot, rulesViolationType, businessCriteriaId, onlyFailedChecks);

            return query?.Select(_ => new RuleViolationResultDTO
                {
                    Rule = new RuleDetailsDTO { Name = _.Reference.Name, Key = _.Reference.Key },
                    TotalChecks = _.DetailResult.ViolationRatio.TotalChecks,
                    TotalFailed = _.DetailResult.ViolationRatio.FailedChecks,
                    Grade = _.DetailResult.Grade
                })
                .Distinct(new RuleViolationResultDTO.Comparer())
                .OrderByDescending(_ => _.TotalFailed)
                .Take(nbTopResult)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="rulesViolationType"></param>
        /// <param name="businessCriteriaId"></param>
        /// <param name="onlyFailedChecks"></param>
        /// <returns></returns>
        public static List<RuleViolationResultDTO> GetAllRuleViolations(Snapshot snapshot,
                                                                        Constants.RulesViolation rulesViolationType,
                                                                        Constants.BusinessCriteria businessCriteriaId,
                                                                        bool onlyFailedChecks)
        {

            var query = GetQueryRuleViolations(snapshot, rulesViolationType, businessCriteriaId, onlyFailedChecks);

            return query?.Select(_ => new RuleViolationResultDTO
                {
                    Rule = new RuleDetailsDTO { Name = _.Reference.Name, Key = _.Reference.Key },
                    TotalChecks = _.DetailResult.ViolationRatio.TotalChecks,
                    TotalFailed = _.DetailResult.ViolationRatio.FailedChecks,
                    Grade = _.DetailResult.Grade
                })
                .OrderByDescending(_ => _.TotalFailed)
                .ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="rulesViolationType"></param>
        /// <param name="businessCriteriaId"></param>
        /// <param name="onlyFailedChecks"></param>
        /// <returns></returns>
        public static int? GetNbRuleWithViolations(Snapshot snapshot,
                                                     Constants.RulesViolation rulesViolationType,
                                                     Constants.BusinessCriteria businessCriteriaId,
                                                     bool onlyFailedChecks)
        {

            var query = GetQueryRuleViolations(snapshot, rulesViolationType, businessCriteriaId, onlyFailedChecks);

            return query?.Select(_ => _.Reference.HRef).Distinct().Count();
        }


        /// <summary> 
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static List<ViolationsStatisticsModuleDTO> GetStatViolation(Snapshot snapshot)
        {
            if (snapshot?.BusinessCriteriaResults == null) return null;

            var modules = snapshot.BusinessCriteriaResults.SelectMany(_ => _.ModulesResult).Select(_ => _.Module).Distinct();

            var query = from module in modules
                        select new ViolationsStatisticsModuleDTO
                        {
                            ModuleName = module.Name,
                            Stats = GetEvolutionSummary(snapshot, module)
                        };

            return query.ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="rulesViolationType"></param>
        /// <param name="businessCriteriaId"></param>
        /// <param name="onlyFailedChecks"></param>
        /// <returns></returns>
        private static IQueryable<ApplicationResult> GetQueryRuleViolations(Snapshot snapshot,
                                                                            Constants.RulesViolation rulesViolationType,
                                                                            Constants.BusinessCriteria businessCriteriaId,
                                                                            bool onlyFailedChecks)
        {
            if (snapshot?.BusinessCriteriaResults == null) return null;

            var query = snapshot.BusinessCriteriaResults.AsQueryable();

            if (businessCriteriaId != 0) { 
            	query = query.Where(_ => _.Reference.Key == businessCriteriaId.GetHashCode());
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (rulesViolationType)
            {
                case Constants.RulesViolation.CriticalRulesViolation:
                    query = query.SelectMany(_ => _.CriticalRulesViolation);
                    break;
                case Constants.RulesViolation.NonCriticalRulesViolation:
                    query = query.SelectMany(_ => _.NonCriticalRulesViolation);
                    break;
                default:
                    query = query.SelectMany(_ => _.CriticalRulesViolation.Union(_.NonCriticalRulesViolation));
                    break;
            }


            query = from bc in query
                where bc.DetailResult != null
                      && bc.DetailResult.ViolationRatio != null
                      && (!onlyFailedChecks || bc.DetailResult.ViolationRatio.FailedChecks > 0)
                select bc;

            return query;
        }


        public static IEnumerable<ViolationsStatisticsDTO> GetBCEvolutionSummary(Snapshot snapshot, int bcid)
        {
            return snapshot?.BusinessCriteriaResults?.Where(_ => _.Reference.Key == bcid && _.DetailResult.EvolutionSummary != null)
                .Select(_ => new ViolationsStatisticsDTO
                {
                    BusinessCriteria = (Constants.BusinessCriteria)bcid,
                    TotalCriticalViolations = _.DetailResult.EvolutionSummary.TotalCriticalViolations,
                    AddedCriticalViolations = _.DetailResult.EvolutionSummary.AddedCriticalViolations,
                    RemovedCriticalViolations = _.DetailResult.EvolutionSummary.RemovedCriticalViolations,
                    TotalViolations = _.DetailResult.EvolutionSummary.TotalViolations,
                    AddedViolations = _.DetailResult.EvolutionSummary.AddedViolations,
                    RemovedViolations = _.DetailResult.EvolutionSummary.RemovedViolations
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        private static List<ViolationsStatisticsDTO> GetEvolutionSummary(Snapshot snapshot, Module module)
        {
            if (snapshot?.BusinessCriteriaResults == null || module==null) return null;

            return snapshot.BusinessCriteriaResults.Where(_ => _.ModulesResult.Any(m => m.Module != null && m.Module.Equals(module) && m.DetailResult?.EvolutionSummary != null))
                                                   .Select(_ => new ViolationsStatisticsDTO
                                                   {
                                                       BusinessCriteria = (Constants.BusinessCriteria)_.Reference.Key,
                                                       TotalCriticalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module))?.DetailResult.EvolutionSummary.TotalCriticalViolations,
                                                       AddedCriticalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module))?.DetailResult.EvolutionSummary.AddedCriticalViolations,
                                                       RemovedCriticalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module))?.DetailResult.EvolutionSummary.RemovedCriticalViolations,
                                                       TotalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module))?.DetailResult.EvolutionSummary.TotalViolations,
                                                       AddedViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module))?.DetailResult.EvolutionSummary.AddedViolations,
                                                       RemovedViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module))?.DetailResult.EvolutionSummary.RemovedViolations
                                                   })
                                                   .ToList();
        }

        public static int? GetTotalChecks(Result violations)
        {
            int? totalChecks = null;
            if (violations != null && violations.ApplicationResults.Any())
            {
                totalChecks = violations.ApplicationResults[0].DetailResult?.ViolationRatio?.TotalChecks;
            }
            return totalChecks;
        }

        public static int? GetFailedChecks(Result violations)
        {
            int? failedChecks = null;
            if (violations != null && violations.ApplicationResults.Any())
            {
                failedChecks = violations.ApplicationResults[0].DetailResult?.ViolationRatio?.FailedChecks;
            }
            return failedChecks;
        }

        public static ViolStatMetricIdDTO GetViolStat(Snapshot snapshot, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resbc != null)
            {
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = resbc.DetailResult.EvolutionSummary?.TotalViolations,
                        AddedViolations = resbc.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = resbc.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = resbc.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                        AddedCriticalViolations = resbc.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                        RemovedCriticalViolations = resbc.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                    };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (restc != null)
            {
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = restc.DetailResult.EvolutionSummary?.TotalViolations,
                        AddedViolations = restc.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = restc.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = restc.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                        AddedCriticalViolations = restc.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                        RemovedCriticalViolations = restc.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                    };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resqr != null)
            {
                // for a quality rule, there is no 'critical' violations. to simplify code, the critical are set to the same values than the violations itself
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = resqr.DetailResult.ViolationRatio?.FailedChecks,
                        AddedViolations = resqr.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = resqr.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = resqr.DetailResult.ViolationRatio?.FailedChecks,
                        AddedCriticalViolations = resqr.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedCriticalViolations = resqr.DetailResult.EvolutionSummary?.RemovedViolations
                    };
            }

            return null;
        }

        public static ViolStatMetricIdDTO GetAggregatedViolStat(Dictionary<Application, Snapshot> snapshotList, int metricId)
        {
            // Aggregator is SUM for count of violations and critical violations
            int? totalViol = 0;
            int? addedViol = 0;
            int? removedViol = 0;
            int? totalCritViol = 0;
            int? addedCritViol = 0;
            int? removedCritViol = 0;
            
            foreach (Application _application in snapshotList.Keys)
            {
                ViolStatMetricIdDTO appRes = GetViolStat(snapshotList[_application], metricId);
                totalViol += appRes.TotalViolations;
                addedViol += appRes.AddedViolations;
                removedViol += appRes.RemovedViolations;
                totalCritViol += appRes.TotalCriticalViolations;
                addedCritViol += appRes.AddedCriticalViolations;
                removedCritViol += appRes.RemovedCriticalViolations;
            }
            return new ViolStatMetricIdDTO
            {
                Id = metricId,
                TotalViolations = totalViol,
                AddedViolations = addedViol,
                RemovedViolations = removedViol,
                TotalCriticalViolations = totalCritViol,
                AddedCriticalViolations = addedCritViol,
                RemovedCriticalViolations = removedCritViol
            };
        }

        public static ViolStatMetricIdDTO GetAggregatedViolStatTechno(Dictionary<Application, Snapshot> snapshotList, string techno, int metricId)
        {
            // Aggregator is SUM for count of violations and critical violations
            int? totalViol = 0;
            int? addedViol = 0;
            int? removedViol = 0;
            int? totalCritViol = 0;
            int? addedCritViol = 0;
            int? removedCritViol = 0;

            foreach (Application _application in snapshotList.Keys)
            {
                ViolStatMetricIdDTO appRes = GetViolStatTechno(snapshotList[_application], techno, metricId);
                if (appRes.TotalViolations != null) totalViol += appRes.TotalViolations;
                if (appRes.AddedViolations != null) addedViol += appRes.AddedViolations;
                if (appRes.RemovedViolations != null) removedViol += appRes.RemovedViolations;
                if (appRes.TotalCriticalViolations != null) totalCritViol += appRes.TotalCriticalViolations;
                if (appRes.AddedCriticalViolations != null) addedCritViol += appRes.AddedCriticalViolations;
                if (appRes.RemovedCriticalViolations != null) removedCritViol += appRes.RemovedCriticalViolations;
            }
            return new ViolStatMetricIdDTO
            {
                Id = metricId,
                TotalViolations = totalViol,
                AddedViolations = addedViol,
                RemovedViolations = removedViol,
                TotalCriticalViolations = totalCritViol,
                AddedCriticalViolations = addedCritViol,
                RemovedCriticalViolations = removedCritViol
            };
        }

        public static ViolStatMetricIdDTO GetViolStatModule(Snapshot snapshot, int modId, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resbc != null)
            {
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.TotalViolations,
                        AddedViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                        AddedCriticalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                        RemovedCriticalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                    };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (restc != null)
            {
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.TotalViolations,
                        AddedViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                        AddedCriticalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                        RemovedCriticalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                    };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resqr != null)
            {
                // for a quality rule, there is no 'critical' violations. to simplify code, the critical are set to the same values than the violations itself
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.ViolationRatio?.FailedChecks,
                        AddedViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.ViolationRatio?.FailedChecks,
                        AddedCriticalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedCriticalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.EvolutionSummary?.RemovedViolations
                    };
            }

            return null;
        }

        public static ViolStatMetricIdDTO GetViolStatTechno(Snapshot snapshot, string techno, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resbc != null)
            {
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = resbc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalViolations,
                        AddedViolations = resbc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = resbc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = resbc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                        AddedCriticalViolations = resbc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                        RemovedCriticalViolations = resbc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                    };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (restc != null)
            {
                return new ViolStatMetricIdDTO
                    {
                        Id = metricId,
                        TotalViolations = restc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalViolations,
                        AddedViolations = restc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                        RemovedViolations = restc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations,
                        TotalCriticalViolations = restc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                        AddedCriticalViolations = restc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                        RemovedCriticalViolations = restc.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                    };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resqr != null)
            {
                // for a quality rule, there is no 'critical' violations. to simplify code, the critical are set to the same values than the violations itself
                return new ViolStatMetricIdDTO
                {
                    Id = metricId,
                    TotalViolations = resqr.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.ViolationRatio?.FailedChecks,
                    AddedViolations = resqr.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                    RemovedViolations = resqr.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations,
                    TotalCriticalViolations = resqr.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.ViolationRatio?.FailedChecks,
                    AddedCriticalViolations = resqr.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                    RemovedCriticalViolations = resqr.TechnologyResult.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations
                };
            }

            return null;
        }

        public static ViolStatMetricIdDTO GetViolStatModuleTechno(Snapshot snapshot, int modId, string techno, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resbc != null)
            {
                return new ViolStatMetricIdDTO
                {
                    Id = metricId,
                    TotalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalViolations,
                    AddedViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                    RemovedViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations,
                    TotalCriticalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                    AddedCriticalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                    RemovedCriticalViolations = resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (restc != null)
            {
                return new ViolStatMetricIdDTO
                {
                    Id = metricId,
                    TotalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalViolations,
                    AddedViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                    RemovedViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations,
                    TotalCriticalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.TotalCriticalViolations,
                    AddedCriticalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedCriticalViolations,
                    RemovedCriticalViolations = restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedCriticalViolations
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resqr != null)
            {
                // for a quality rule, there is no 'critical' violations. to simplify code, the critical are set to the same values than the violations itself
                return new ViolStatMetricIdDTO
                {
                    Id = metricId,
                    TotalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.ViolationRatio?.FailedChecks,
                    AddedViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                    RemovedViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations,
                    TotalCriticalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.ViolationRatio?.FailedChecks,
                    AddedCriticalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.AddedViolations,
                    RemovedCriticalViolations = resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults.FirstOrDefault(t => t.Technology == techno)?.DetailResult.EvolutionSummary?.RemovedViolations
                };
            }

            return null;
        }


    }

}
