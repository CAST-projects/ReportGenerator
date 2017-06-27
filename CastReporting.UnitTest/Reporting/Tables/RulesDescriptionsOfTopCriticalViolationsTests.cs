using System.Collections.Generic;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesDescriptionsOfTopCriticalViolationsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern4592.json", "Data")]
        [DeploymentItem(@".\Data\cc60011.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @".\Data\cc60011.json", null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesDescriptionsOfTopCriticalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" },
                {"IDX","0" },
                {"COUNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rules Descriptions for Top Critical Violation Rules For Business Criterion Transferability", null});
            expectedData.AddRange(new List<string> { "Rule Name", "Avoid hiding static Methods" });
            expectedData.AddRange(new List<string> { "Rationale", "Hiding is all about polymorphism. This means that the OO designer expects to override methods and use polymorphism so that code calling methods through a base class will end up executing different methods depending on the instance being used. This is not the case with static methods. When static methods are called, there is no polymorphism in play. It is always the static method of the type used to reference the object used that is called. Hiding static methods is a misuse of OO practices that results in misunderstanding of what is going to be executed at runtime and thus leads to unexpected behavior, jeopardizing the stability of the application." });
            expectedData.AddRange(new List<string> { "Description", "Hiding Static Methods is not allowed. This Quality Rule retrieves all static methods that are redefined in subclasses i.e. \"implicitly hidden\". A Static Method MyMethod of Class MySuperClass is \"implicitly hidden\" in Subclass MySubClass if MySubClass contains a similar declaration of MyMethod (i.e. same signature)." });
            expectedData.AddRange(new List<string> { "Remediation", "Review the design of the Method" });
            expectedData.AddRange(new List<string> { "# Violations", "63" });
            expectedData.AddRange(new List<string> { " ", " " });
            TestUtility.AssertTableContent(table, expectedData, 2, 15);
        }

    }
}

