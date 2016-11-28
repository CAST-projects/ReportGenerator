using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    static public class RulesViolationUtility
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="ruleExplorer"></param>
        /// <param name="businessCriteriasIds"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<RuleViolationResultDTO> GetNbViolationByRule(Snapshot snapshot, IRuleExplorer ruleExplorer, List<int> businessCriteriasIds, Int32 count)
        {
            if (snapshot == null || snapshot.TechnicalCriteriaResults == null) return null;


            //Get rules
            List<RuleDetails> rules = new List<RuleDetails>();
            foreach (var metricId in businessCriteriasIds)
            {
                var bcRules = ruleExplorer.GetRulesDetails(snapshot.DomainId, metricId, snapshot.Id);

                rules.AddRange(bcRules);
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
                RuleViolationResultDTO ruleViolationResult = new RuleViolationResultDTO();

                var technicalCriterias = snapshot.TechnicalCriteriaResults
                                                 .Where(_ => _.RulesViolation!=null && _.RulesViolation.Any(p => rule.Key.HasValue && p.Reference.Key == rule.Key.Value))
                                                 .FirstOrDefault();

                if (technicalCriterias != null)
                {
                    ruleViolationResult.Rule = new RuleDetailsDTO { Name = rule.Name, Critical = rule.Critical, CompoundedWeight = (rule.CompoundedWeight.HasValue ? rule.CompoundedWeight.Value : 0) };
                   
                    ruleViolationResult.Grade = technicalCriterias.DetailResult.Grade;

                    ruleViolationResult.TechnicalCriteraiName = technicalCriterias.Reference.Name;

                    var violationRatio = technicalCriterias.RulesViolation.Where(_ => rule.Key.HasValue && _.Reference.Key == rule.Key.Value)
                                                                          .Select(_ => _.DetailResult.ViolationRatio)
                                                                          .FirstOrDefault();
                    if (violationRatio != null)
                    {
                        ruleViolationResult.TotalFailed = violationRatio.FailedChecks;
                        ruleViolationResult.TotalChecks = violationRatio.TotalChecks;                      
                    }

                    reslutByTechnicalCriterias.Add(ruleViolationResult);
                }
            }

            return reslutByTechnicalCriterias.OrderBy(_ => _.Rule.Name).Take(count).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="ruleExplorer"></param>
        /// <param name="businessCriteriasIds"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<TechnicalCriteriaResultDTO> GetTechnicalCriteriaViolations(Snapshot snapshot, Constants.BusinessCriteria businessCriteriaId, Int32 count)
        {
            if (snapshot.QIBusinessCriterias != null && snapshot.TechnicalCriteriaResults!=null)
            {
                IEnumerable<Int32> technicalCriteriaId = snapshot.QIBusinessCriterias.Where(_ => (Int32)businessCriteriaId == _.Key)
                                                                                      .SelectMany(_ => _.Contributors).Select( _ => _.Key);

                return snapshot.TechnicalCriteriaResults.Where(_ => technicalCriteriaId.Contains(_.Reference.Key) && _.Reference !=null && _.DetailResult != null && _.RulesViolation != null)
                                                       .Select(_ => new TechnicalCriteriaResultDTO
                                                                        {
                                                                            Name = _.Reference.Name, 
                                                                            Grade = _.DetailResult.Grade,
                                                                            TotalChecks = _.RulesViolation.Sum(r => (r.DetailResult != null && r.DetailResult.ViolationRatio!=null) ? r.DetailResult.ViolationRatio.TotalChecks : 0),
                                                                            TotalFailed = _.RulesViolation.Sum(r => (r.DetailResult != null && r.DetailResult.ViolationRatio!=null) ? r.DetailResult.ViolationRatio.FailedChecks : 0)
                                                                        })
                                                        .OrderByDescending(_ => _.TotalFailed)
                                                        .Take(count)
                                                        .ToList();     
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="rulesViolationType"></param>
        /// <param name="businessCriteriaId"></param>
        /// <param name="nbTopResult"></param>
        /// <returns></returns>
        public static List<RuleViolationResultDTO> GetRuleViolations(Snapshot snapshot,
                                                                        Constants.RulesViolation rulesViolationType,
                                                                        Constants.BusinessCriteria businessCriteriaId,
                                                                        bool onlyFailedChecks,
                                                                        Int32 nbTopResult)
        {            
          
            var query = GetQueryRuleViolations(snapshot, rulesViolationType, businessCriteriaId, onlyFailedChecks);

            if (query != null)                            
                return query.Select(_ => new RuleViolationResultDTO
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
            else
                 return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="rulesViolationType"></param>
        /// <param name="businessCriteriaId"></param>
        /// <returns></returns>
        public static List<RuleViolationResultDTO> GetAllRuleViolations(Snapshot snapshot,
                                                                        Constants.RulesViolation rulesViolationType,
                                                                        Constants.BusinessCriteria businessCriteriaId,
                                                                        bool onlyFailedChecks)
        {

            var query = GetQueryRuleViolations(snapshot, rulesViolationType, businessCriteriaId, onlyFailedChecks);

            if (query != null)
                return query.Select(_ => new RuleViolationResultDTO
                {
                    Rule = new RuleDetailsDTO { Name = _.Reference.Name, Key = _.Reference.Key },
                    TotalChecks = _.DetailResult.ViolationRatio.TotalChecks,
                    TotalFailed = _.DetailResult.ViolationRatio.FailedChecks,
                    Grade = _.DetailResult.Grade
                })
                                        .OrderByDescending(_ => _.TotalFailed)
                                        .ToList();
            else
                return null;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="onlyFailedChecks"></param>
        /// <returns></returns>
        public static Int32? GetNbRuleWithViolations(Snapshot snapshot,
                                                     Constants.RulesViolation rulesViolationType,
                                                     Constants.BusinessCriteria businessCriteriaId,
                                                     bool onlyFailedChecks)
        {

            var query = GetQueryRuleViolations(snapshot, rulesViolationType, businessCriteriaId, onlyFailedChecks);

            return (query != null) ? query.Select(_ => _.Reference.HRef).Distinct().Count() : (Int32?)null;
        }


        /// <summary> 
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static List<ViolationsStatisticsModuleDTO> GetStatViolation(Snapshot snapshot)
        {
            if (snapshot == null || snapshot.BusinessCriteriaResults == null) return null;

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
            if (snapshot == null || snapshot.BusinessCriteriaResults == null) return null;

            var query = snapshot.BusinessCriteriaResults.AsQueryable();

            if (businessCriteriaId != 0) { 
            	query = query.Where(_ => _.Reference.Key == businessCriteriaId.GetHashCode());
            }

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


            query = (from bc in query
                     where bc.DetailResult != null
                     && bc.DetailResult.ViolationRatio != null
                     && (!onlyFailedChecks || bc.DetailResult.ViolationRatio.FailedChecks > 0)
                     select bc);

            return query;
        }


        public static IEnumerable<ViolationsStatisticsDTO> GetBCEvolutionSummary(Snapshot snapshot, int bcid)
        {
            if (snapshot == null || snapshot.BusinessCriteriaResults == null) return null;

            return snapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == bcid && _.DetailResult.EvolutionSummary != null)
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
            if (snapshot == null || snapshot.BusinessCriteriaResults == null || module==null) return null;

            return snapshot.BusinessCriteriaResults.Where(_ => _.ModulesResult.Any(m => m.Module != null && m.Module.Equals(module) && m.DetailResult != null && m.DetailResult.EvolutionSummary != null))
                                                   .Select(_ => new ViolationsStatisticsDTO
                                                   {
                                                       BusinessCriteria = (Constants.BusinessCriteria)_.Reference.Key,
                                                       TotalCriticalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module)).DetailResult.EvolutionSummary.TotalCriticalViolations,
                                                       AddedCriticalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module)).DetailResult.EvolutionSummary.AddedCriticalViolations,
                                                       RemovedCriticalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module)).DetailResult.EvolutionSummary.RemovedCriticalViolations,
                                                       TotalViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module)).DetailResult.EvolutionSummary.TotalViolations,
                                                       AddedViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module)).DetailResult.EvolutionSummary.AddedViolations,
                                                       RemovedViolations = _.ModulesResult.FirstOrDefault(m => m.Module.Equals(module)).DetailResult.EvolutionSummary.RemovedViolations
                                                   })
                                                   .ToList();
        }

        public static int? GetTotalChecks(Result violations)
        {
            Int32? totalChecks = null;
            if (violations != null && violations.ApplicationResults.Any())
            {
                totalChecks = violations.ApplicationResults[0].DetailResult?.ViolationRatio?.TotalChecks;
            }
            return totalChecks;
        }

        public static int? GetFailedChecks(Result violations)
        {
            Int32? failedChecks = null;
            if (violations != null && violations.ApplicationResults.Any())
            {
                failedChecks = violations.ApplicationResults[0].DetailResult?.ViolationRatio?.FailedChecks;
            }
            return failedChecks;
        }



    }

}
