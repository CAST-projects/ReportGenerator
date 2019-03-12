
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
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<RuleDetails> GetRulesDetails(string domain, int businessCriteria, long snapshotId);

        IEnumerable<Contributor> GetRulesInTechnicalCriteria(string domain, string technicalCriteria, long snapshotHRef);

        /// <summary>
        /// Get the list of added,updated or deleted artifacts for application, module or technology
        /// for technology, href param is the one of application and technology param is not null
        /// </summary>
        /// <param name="href"></param>
        /// <param name="status"></param>
        /// <param name="currentSnapshotId"></param>
        /// <param name="previousSnapshotId"></param>
        /// <param name="technology"></param>
        /// <returns>list of components with risk categories</returns>

        IEnumerable<DeltaComponent> GetDeltaComponents(string href, string status, string currentSnapshotId, string previousSnapshotId, string technology);
    }
}
