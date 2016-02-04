using System;
using System.Collections.Generic;
using CastReporting.Domain;


namespace CastReporting.Repositories.Interfaces
{
    /// <summary>
    /// Defines the minimal data access layer methods that must be
    /// enabled in class that inherit of this class.
    /// </summary>
    public interface ICastRepsitory : IDisposable
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsServiceValid();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<CastDomain> GetDomains();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Application> GetApplicationsByDomain(string domainHRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Application GetApplication(string hRef);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Snapshot> GetSnapshotsByApplication(string applicationHRef);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Domain.System> GetSystemsByApplication(string applicationHRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Snapshot GetSnapshot(string hRef);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Module> GetModules(string hRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Module GetModule(string hRef);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> GetResultsQualityIndicators(string hRef, string qiParam, string snapshotsParam, string modulesParam, string technologiesParam, string categoriesParam); 
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <param name="param"></param>
        /// <param name="modulesParam"></param>
        /// <returns></returns>
        IEnumerable<Result> GetResultsSizingMeasures(string hRef, string param, string snapshotsParam, string technologiesParam, string moduleParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainHRef"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<QIQualityRules> GetConfQualityRulesBySnapshot(string domainHRef, Int64 snapshotId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        QIQualityRules GetConfQualityRules(string href);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <returns></returns>
        IEnumerable<QIBusinessCriteria> GetConfBusinessCriteriaBySnapshot(string domainHRef, Int64 snapshotId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        QIBusinessCriteria GetConfBusinessCriteria(string href);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationHRef"></param>
        /// <param name="snapshotHRef"></param>
        /// <returns></returns>
        IEnumerable<ActionPlan> GetActionPlanBySnapshot(string snapshotHRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="businessCriteria"></param>
        /// <returns></returns>
        IEnumerable<Result> GetRulesViolations(string snapshotHRef, string criticity, string businessCriteria);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="qualityDistribution"></param>
        /// <returns></returns>
        IEnumerable<Result> GetComplexityIndicators(string snapshotHRef, string qualityDistribution);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ruleId"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        RuleDescription GetSpecificRule(string domain, string ruleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<RuleDetails> GetRulesDetails(string domain, string businessCriteria, string snapshotId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <returns></returns>
        IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="moduleId"></param>
        /// <param name="snapshotId"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count);
    }
}
