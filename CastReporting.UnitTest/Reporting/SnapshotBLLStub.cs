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

        public IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string ruleId, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Result> GetQualityIndicatorResults(string snapshotHref, string qualityIndicator)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Result> GetSizingMeasureResults(string snapshotHref, string sizingMeasure)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            throw new NotImplementedException();
        }
            }
        }
