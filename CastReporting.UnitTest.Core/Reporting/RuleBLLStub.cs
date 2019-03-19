using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using CastReporting.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting
{
    public class RuleBLLStub : BaseBLL, IRuleExplorer
    {
        [DeploymentItem(@".\Data\RulePattern1634.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern4592.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        public RuleDescription GetSpecificRule(string domain, string ruleId)
        {
            RuleDescription res;

            switch (ruleId)
            {
                case "1634":
                    res = TestUtility.GetSampleResult<RuleDescription>(@".\Data\RulePattern1634.json").FirstOrDefault();
                    break;
                case "4592":
                    res = TestUtility.GetSampleResult<RuleDescription>(@".\Data\RulePattern4592.json").FirstOrDefault();
                    break;
                case "7424":
                    res = TestUtility.GetSampleResult<RuleDescription>(@".\Data\RulePattern7424.json").FirstOrDefault();
                    break;
                case "7846":
                    res = TestUtility.GetSampleResult<RuleDescription>(@".\Data\RulePattern7846.json").FirstOrDefault();
                    break;
                default:
                    res = TestUtility.GetSampleResult<RuleDescription>(@".\Data\RulePatterns.json").FirstOrDefault(_ => _.Key == ruleId);
                    break;
            }

            return res;
        }

        [DeploymentItem(@".\Data\RuleViolation1634.json", "Data")]
        [DeploymentItem(@".\Data\RuleViolation1634Previous.json", "Data")]
        public IEnumerable<Result> GetRulesViolations(string snapshotHRef, string ruleId)
        {
            if (ruleId != "1634") return null;
            IEnumerable<Result>  res = snapshotHRef == "AED/applications/3/snapshots/4" ? TestUtility.GetSampleResult<Result>(@".\Data\RuleViolation1634Previous.json").ToList() 
                : TestUtility.GetSampleResult<Result>(@".\Data\RuleViolation1634.json").ToList();

            return res;
        }

        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60012.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public IEnumerable<RuleDetails> GetRulesDetails(string domain, int businessCriteria, long snapshotId)
        {
            switch (businessCriteria)
            {
                case 60011:
                    return TestUtility.GetSampleResult<RuleDetails>(@".\Data\BaseQI60011.json").ToList();

                case 60012:
                    return TestUtility.GetSampleResult<RuleDetails>(@".\Data\BaseQI60012.json").ToList();

                default:
                    return TestUtility.GetSampleResult<RuleDetails>(@".\Data\BaseQI60017.json").ToList();
            }
        }

        public IEnumerable<Contributor> GetRulesInTechnicalCriteria(string domain, string technicalCriteria, long snapshotHRef)
        {
            switch (technicalCriteria)
            {
                case "66070":
                    return new List<Contributor>
                    {
                        new Contributor() {Key = 7132, Critical = true},
                        new Contributor() {Key = 7846, Critical = false}
                    };
                default:
                    return new List<Contributor>
                    {
                        new Contributor() {Key = 7424},
                        new Contributor() {Key = 7846}
                    };

            }
        }

        [DeploymentItem(@".\Data\DeltaComponents.json", "Data")]
        public IEnumerable<DeltaComponent> GetDeltaComponents(string href, string status, string currentSnapshotId, string previousSnapshotId, string technology)
        {
            return TestUtility.GetSampleResult<DeltaComponent>(@".\Data\DeltaComponents.json").ToList();
        }
    }
}
