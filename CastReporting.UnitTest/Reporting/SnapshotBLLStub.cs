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

        public IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        {
            throw new NotImplementedException();
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

        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        public IEnumerable<Result> GetQualityIndicatorResults(string snapshotHref, string qualityIndicator)
        {
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\Snapshot_QIresults1.json").ToList();
            return res;

        }

        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        public IEnumerable<Result> GetSizingMeasureResults(string snapshotHref, string sizingMeasure)
        {
            IEnumerable<Result> res = TestUtility.GetSampleResult<Result>(@".\Data\DreamTeamSnap4Sample12.json").ToList();
            return res;
        }

        public IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            throw new NotImplementedException();
        }

    }
}
