
using System;
using System.Collections.Generic;


namespace CastReporting.Domain.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRuleExplorer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        RuleDescription GetSpecificRule(string domain, string ruleId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        IEnumerable<Result> GetRulesViolations(string snapshotHRef, string ruleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="snapshotHRef"></param>
        /// <returns></returns>
        IEnumerable<RuleDetails> GetRulesDetails(string domain, string businessCriteria, string snapshotHRef);
    }
}
