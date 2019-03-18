using System;
using System.Collections.Generic;

namespace CastReporting.Domain.Interfaces
{
    public interface ISnapshotExplorer : IDisposable
    {
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
        /// <param name="snapshotHref"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count);

        IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count);

        IEnumerable<OmgFunction> GetOmgFunctionsEvolutions(string snapshotHref, int count);
        IEnumerable<OmgFunctionTechnical> GetOmgFunctionsTechnical(string snapshotHref, int count);

        IEnumerable<CommonCategories> GetCommonCategories(WSConnection connection);

        string GetCommonCategoriesJson(WSConnection connection);

        IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts);
        IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts, bool modules, bool technologies);

        IEnumerable<Result> GetSizingMeasureResults(string snapshotHref, string sizingMeasure);
        IEnumerable<Result> GetQualityIndicatorResults(string snapshotHref, string qualityIndicator);

        IEnumerable<Result> GetQualityStandardsRulesResults(string snapshotHref, string standardTag);

        IEnumerable<Result> GetQualityStandardsTagsResults(string snapshotHref, string standardTag);

        List<string> GetQualityStandardsRulesList(string snapshotHref, string standardTag);

        IEnumerable<MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string ruleId, int count);

        IEnumerable<Violation> GetViolationsListIDbyBC(string snapshotHref, string ruleId, string bcId, int count, string technos);

        IEnumerable<Violation> GetRemovedViolationsbyBC(string snapshotHref, string bcId, int count);

        IEnumerable<Violation> GetViolationsInActionPlan(string snapshotHref, int count);

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

        IEnumerable<ComponentWithProperties> GetComponentsByProperties(string snapshotHref, int businessCriteria, string prop1, string prop2, string order1, string order2, int count);

        AssociatedValue GetAssociatedValue(string domainId, string componentId, string snapshotId, string metricId);
        AssociatedValuePath GetAssociatedValuePath(string domainId, string componentId, string snapshotId, string metricId);
        AssociatedValueGroup GetAssociatedValueGroup(string domainId, string componentId, string snapshotId, string metricId);
        AssociatedValueObject GetAssociatedValueObject(string domainId, string componentId, string snapshotId, string metricId);


        List<Tuple<string, Dictionary<int, string>>> GetSourceCode(string domainId, string componentId, string snapshotId, int offset);

        Dictionary<int, string> GetSourceCodeBookmark(string domainId, CodeBookmark bookmark, int offset);

        TypedComponent GetTypedComponent(string domainId, string componentId, string snapshotId);
    }
}
