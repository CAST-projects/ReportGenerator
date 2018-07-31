using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting
{
    public class SnapshotBLLStub : BaseBLL, ISnapshotExplorer
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once NotAccessedField.Local
        Snapshot _Snapshot;

        public SnapshotBLLStub(WSConnection connection, Snapshot snapshot)
            : base(connection)
        {
            _Snapshot = snapshot;
        }
        [DeploymentItem(@".\Data\BackFacts.json", "Data")]
        public IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts)
        {
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\BackFacts.json").ToList();
            return res;
        }

        [DeploymentItem(@".\Data\BusinessValue.json", "Data")]
        public IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts, bool modules, bool technologies)
        {
            string[] _sample5Gpt = {"10152", "10154", "10161"};
            if (_sample5Gpt.Contains(backgroundFacts)) return new List<Result>();
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\BusinessValue.json").ToList();
            return res;
        }

        public IEnumerable<CommonCategories> GetCommonCategories(WSConnection connection)
        {
            throw new NotImplementedException();
        }

        public string GetCommonCategoriesJson(WSConnection connection)
        {
            throw new NotImplementedException();
        }

        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        public IEnumerable<Violation> GetViolationsInActionPlan(string snapshotHref, int count)
        {
            return count != -1 ? TestUtility.GetSampleResult<Violation>(@".\Data\ActionPlanViolations1.json").Take(count).ToList() : TestUtility.GetSampleResult<Violation>(@".\Data\ActionPlanViolations1.json").ToList();
        }

        [DeploymentItem(@".\Data\Component60016Snap.json", "Data")]
        public IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count)
        {
            IEnumerable<Component> res = null;
            if (businessCriteria == "60016")
                res = TestUtility.GetSampleResult<Component>(@".\Data\Component60016Snap.json").ToList();

            if (count != -1)
                res = res?.Take(count);

            return res;
        }

        [DeploymentItem(@".\Data\Component60016ModSnap.json", "Data")]
        public IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        {
            IEnumerable<Component> res = null;
            if (businessCriteria == "60016")
                res = TestUtility.GetSampleResult<Component>(@".\Data\Component60016ModSnap.json").ToList();
            if (count != -1)
                res = res?.Take(count);

            return res;
        }

        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_bookmarks.json", "Data")]
        public AssociatedValue GetAssociatedValue(string domainId, string componentId, string snapshotId, string metricId)
        {
            switch (metricId)
            {
                case "8108":
                    return TestUtility.GetSampleResult<AssociatedValue>(@".\Data\findings_bookmarks.json").ToArray()[0];
                case "8032":
                    return TestUtility.GetSampleResult<AssociatedValue>(@".\Data\findings_bookmarks.json").ToArray()[1];
                case "7688":
                    return TestUtility.GetSampleResult<AssociatedValue>(@".\Data\findings_bookmarks.json").ToArray()[2];
                default:
                    return TestUtility.GetSampleResult<AssociatedValue>(@".\Data\findings7392.json").ToArray()[0];
            }
            
        }

        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_path.json", "Data")]
        public AssociatedValuePath GetAssociatedValuePath(string domainId, string componentId, string snapshotId, string metricId)
        {
            switch (metricId)
            {
                case "7390":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[1];
                case "8108":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[2];
                case "8032":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[3];
                case "7156":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[4];
                case "4722":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[5];
                case "7740":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[6];
                case "1596":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[7];
                case "7846":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[8];
                case "7210":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[9];
                case "7688":
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[10];
                default:
                    return TestUtility.GetSampleResult<AssociatedValuePath>(@".\Data\findings7392.json").ToArray()[0];
            }
        }

        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_group.json", "Data")]
        public AssociatedValueGroup GetAssociatedValueGroup(string domainId, string componentId, string snapshotId, string metricId)
        {
            throw new NotImplementedException();
        }

        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_object.json", "Data")]
        public AssociatedValueObject GetAssociatedValueObject(string domainId, string componentId, string snapshotId, string metricId)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<string, Dictionary<int, string>>> GetSourceCode(string domainId, string componentId, string snapshotId, int offset)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, string> GetSourceCodeBookmark(string domainId, CodeBookmark bookmark, int offset)
        {
            Dictionary<int, string> sources = new Dictionary<int, string>
            {
                {1197, "PreparedStatement statement = null;"},
                {1198, "        try"},
                {1199, "        {"},
                {1200, "            statement = consolidatedConn.prepareStatement(insertMessage); "},
                {1201, "            statement.setString(1, message); "},
                {1202, "            statement.executeUpdate(); "},
                {1203, "        }"}
            };
            return sources;
        }

        
        public TypedComponent GetTypedComponent(string domainId, string componentId, string snapshotId)
        {
            ObjectType type = new ObjectType(){Label = "MyObjType", Name="toto"};
            return new TypedComponent() {Type = type};
        }

        [DeploymentItem(@".\Data\IfpugFunctions.csv", "Data")]
        public IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count)
        {
            IEnumerable<IfpugFunction> res = TestUtility.GetCsvSampleResult<IfpugFunction>(@".\Data\IfpugFunctions.csv",count,null).ToList();
            return res;
        }

        public IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count)
        {
            throw new NotImplementedException();
        }

        [DeploymentItem(@".\Data\topArtefacts7212.csv", "Data")]
        [DeploymentItem(@".\Data\topArtefacts3576.csv", "Data")]
        public IEnumerable<MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string ruleId, int count)
        {
            IEnumerable<MetricTopArtifact> res = null;
            if (ruleId == "7212")
            {
                res = TestUtility.GetCsvSampleResult<MetricTopArtifact>(@".\Data\topArtefacts7212.csv", count, null).ToList();
            }
            if (ruleId == "3576")
            {
                res = TestUtility.GetCsvSampleResult<MetricTopArtifact>(@".\Data\topArtefacts3576.csv", count, null).ToList();
            }
            return res;
        }

        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7426_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60012.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60013.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60014.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60016_module.json", "Data")]
        [DeploymentItem(@".\Data\CriticalViolationsList_60017.json", "Data")]
        public IEnumerable<Violation> GetViolationsListIDbyBC(string snapshotHref, string ruleId, string bcId, int count, string technos)
        {
            IEnumerable<Violation> res;
            switch (ruleId)
            {
                case "7424":
                case "7132":
                case "7558":
                case "7388":
                    res = count != -1 ? TestUtility.GetSampleResult<Violation>(@".\Data\Violations7424_60017.json").Take(count).ToList()
                        : TestUtility.GetSampleResult<Violation>(@".\Data\Violations7424_60017.json").ToList();
                    break;
                case "7426":
                    res = count != -1 ? TestUtility.GetSampleResult<Violation>(@".\Data\Violations7426_60017.json").Take(count).ToList()
                        : TestUtility.GetSampleResult<Violation>(@".\Data\Violations7426_60017.json").ToList();
                    break;
                case "7846":
                    res = count != -1 ? TestUtility.GetSampleResult<Violation>(@".\Data\Violations7846_60016.json").Take(count).ToList()
                        : TestUtility.GetSampleResult<Violation>(@".\Data\Violations7846_60016.json").ToList();
                    break;
                case "(critical-rules)":
                    switch (bcId)
                    {
                        case "60012":
                            res = TestUtility.GetSampleResult<Violation>(@".\Data\CriticalViolationsList_60012.json").ToList();
                            break;
                        case "60013":
                            res = TestUtility.GetSampleResult<Violation>(@".\Data\CriticalViolationsList_60013.json").ToList();
                            break;
                        case "60014":
                            res = TestUtility.GetSampleResult<Violation>(@".\Data\CriticalViolationsList_60014.json").ToList();
                            break;
                        case "60016":
                            res = TestUtility.GetSampleResult<Violation>(@".\Data\CriticalViolationsList_60016.json").ToList();
                            break;
                        case "60017":
                            res = TestUtility.GetSampleResult<Violation>(@".\Data\CriticalViolationsList_60017.json").ToList();
                            break;
                        default:
                            res = new List<Violation>();
                            break;
                    }
                    break;
                case "(nc:60016,cc:60016)":
                    res = TestUtility.GetSampleResult<Violation>(@".\Data\CriticalViolationsList_60016.json").ToList();
                    break;
                default:
                    res = new List<Violation>();
                    break;
            }
            return res;
        }

        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        public IEnumerable<Result> GetQualityIndicatorResults(string snapshotHref, string qualityIndicator)
        {
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\Snapshot_QIresults1.json").ToList();
            return res;

        }

        [DeploymentItem(@".\Data\Snapshot_StdTagsOWASPresults1.json", "Data")]
        public IEnumerable<Result> GetQualityStandardsRulesResults(string snapshotHref, string qualityIndicator)
        {
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\Snapshot_QIresults1.json").ToList();
            return res;

        }

        public List<string> GetQualityStandardsRulesList(string snapshotHref, string qualityIndicator)
        {
            switch (qualityIndicator)
            {
                case "OWASP":
                    return new List<string> { "1596", "4656" };
                case "CWE":
                    return new List<string> { "7424" };
                default:
                    return null;
            }
            
        }


        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        public IEnumerable<Result> GetSizingMeasureResults(string snapshotHref, string sizingMeasure)
        {
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\DreamTeamSnap4Sample12.json").ToList();
            return res;
        }

        [DeploymentItem(@".\Data\Transactions60016Snap.json", "Data")]
        public IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            IEnumerable<Transaction> res = null;
            if (businessCriteria == "60016")
                res = TestUtility.GetSampleResult<Transaction>(@".\Data\Transactions60016Snap.json").ToList();
            if (count != -1)
                res = res?.Take(count);

            return res;
        }

    }
}
