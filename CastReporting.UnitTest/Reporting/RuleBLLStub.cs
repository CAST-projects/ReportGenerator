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
        [DeploymentItem(@".\Data\BaseQI60017.json", "Data")]
        public IEnumerable<RuleDetails> GetRulesDetails(string domain, int businessCriteria, long snapshotId)
        {
            IEnumerable<RuleDetails> res = businessCriteria == 60011 ? TestUtility.GetSampleResult<RuleDetails>(@".\Data\BaseQI60011.json").ToList() 
                : TestUtility.GetSampleResult<RuleDetails>(@".\Data\BaseQI60017.json").ToList();

            return res;
        }
    }
}
