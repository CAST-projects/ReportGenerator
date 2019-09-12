using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System.Collections.Generic;

namespace CastReporting.BLL
{
    public class RuleBLL : BaseBLL, IRuleExplorer
    {
       
        public RuleBLL(WSConnection connection)
            : base(connection) {}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public RuleDescription GetSpecificRule(string domain, string ruleId)
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetSpecificRule(domain, ruleId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<StandardTag> GetQualityStandardTagsApplicabilityByCategory(string domain, string category)
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetQualityStandardsTagsApplicabilityByCategory(domain, category);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public IEnumerable<Result> GetRulesViolations(string snapshotHRef, string ruleId)
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetRulesViolations(snapshotHRef, string.Empty, ruleId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessCriteria"></param>
        /// <param name="snapshotHRef"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public IEnumerable<RuleDetails> GetRulesDetails(string domain, int businessCriteria, long snapshotHRef)
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetRulesDetails(domain, businessCriteria, snapshotHRef);
            }
        }

        public IEnumerable<Contributor> GetRulesInTechnicalCriteria(string domain, string technicalCriteria, long snapshotHRef)
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetRulesForTechnicalCriteria(domain, technicalCriteria, snapshotHRef);
            }
        }

        public IEnumerable<DeltaComponent> GetDeltaComponents(string href, string status, string currentSnapshotId, string previousSnapshotId, string technology)
        {
            using (var castRepository = GetRepository())
            {
                return castRepository.GetDeltaComponents(href, currentSnapshotId, previousSnapshotId, status, technology);
            }
        }
    }
}
