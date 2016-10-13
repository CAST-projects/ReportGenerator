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
        /// <param name="snapshotHRef"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public IEnumerable<RuleDetails> GetRulesDetails(string domain, int businessCriteria, long snapshotHRef)
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetRulesDetails(domain, businessCriteria, snapshotHRef);
            }
        }
    }
}
