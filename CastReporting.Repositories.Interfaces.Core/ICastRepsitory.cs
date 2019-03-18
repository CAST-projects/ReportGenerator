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
        /// <param name="hRef"></param>
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
        /// <param name="hRef"></param>
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
        /// <param name="hRef"></param>
        /// <returns></returns>
        Module GetModule(string hRef);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> GetResultsQualityIndicators(string hRef, string qiParam, string snapshotsParam, string modulesParam, string technologiesParam);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> GetResultsQualityStandardsRules(string hRef, string stgTagParam, string modulesParam, string technologiesParam);

        IEnumerable<Result> GetResultsQualityStandardsTags(string hRef, string stgTagParam);

        IEnumerable<StandardTag> GetQualityStandardsTagsDoc(string hRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <param name="param"></param>
        /// <param name="snapshotsParam"></param>
        /// /// <param name="moduleParam"></param>
        /// /// <param name="technologiesParam"></param>
        /// <returns></returns>
        IEnumerable<Result> GetResultsSizingMeasures(string hRef, string param, string snapshotsParam, string technologiesParam, string moduleParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <param name="param"></param>
        /// <param name="snapshotsParam"></param>
        /// <param name="technologiesParam"></param>
        /// <param name="moduleParam"></param>
        /// <returns></returns>
        IEnumerable<Result> GetResultsBackgroundFacts(string hRef, string param, string snapshotsParam, string technologiesParam, string moduleParam);

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
        /// <param name="domainHRef"></param>
        /// <param name="snapshotId"></param>
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
        /// <param name="snapshotHRef"></param>
        /// <returns></returns>
        IEnumerable<ActionPlan> GetActionPlanBySnapshot(string snapshotHRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="criticity"></param>
        /// <param name="businessCriteria"></param>
        /// <returns></returns>
        IEnumerable<Result> GetRulesViolations(string snapshotHRef, string criticity, string businessCriteria);


        IEnumerable<Violation> GetRemovedViolations(string snapshotHRef, string businessCriteria, int count);

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
        /// <returns></returns>
        RuleDescription GetSpecificRule(string domain, string ruleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<RuleDetails> GetRulesDetails(string domain, int businessCriteria, long snapshotId);

        IEnumerable<Contributor> GetRulesForTechnicalCriteria(string domain, string technicalCriteria, long snapshotId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<CommonCategories> GetCommonCategories();
        
        string GetCommonCategoriesJson();


        string GetCommonTagsJson();

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
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="ruleId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string ruleId, int count);

        TypedComponent GetTypedComponent(string domainHRef, string componentId, string snapshotId);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="ruleId"></param>
        /// <param name="bcId"></param>
        /// <param name="count"></param>
        /// <param name="technos"></param>
        /// <returns></returns>
        IEnumerable<Violation> GetViolationsListIDbyBC(string snapshotHref, string ruleId, string bcId, int count, string technos);
        IEnumerable<Violation> GetViolationsInActionPlan(string snapshotHref, int count);

        AssociatedValue GetAssociatedValue(string domainHRef, string snapshotId, string objectId, string metricId);
        AssociatedValuePath GetAssociatedValuePath(string domainHRef, string snapshotId, string objectId, string metricId);
        AssociatedValueGroup GetAssociatedValueGroup(string domainHRef, string snapshotId, string objectId, string metricId);
        AssociatedValueObject GetAssociatedValueObject(string domainHRef, string snapshotId, string objectId, string metricId);

        IEnumerable<CodeFragment> GetSourceCode(string domainHRef, string snapshotId, string objectId);
        List<string> GetFileContent(string domainHRef, string siteId, string fileId, int startLine, int endLine);
        IEnumerable<ComponentWithProperties> GetComponentsWithProperties(string snapshothref, int bcId, string prop1, string prop2, string order1, string order2, int count);
        IEnumerable<OmgFunction> GetOmgFunctionsEvolutions(string snapshotHref, int count);

        IEnumerable<OmgFunctionTechnical> GetOmgFunctionsTechnical(string snapshotHref, int count);

        IEnumerable<DeltaComponent> GetDeltaComponents(string levelHRef, string snapshotId, string previousSnapshotId, string status, string technology);
    }
}
