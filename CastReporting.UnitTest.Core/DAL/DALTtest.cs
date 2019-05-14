using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Repositories;
using CastReporting.Repositories.Interfaces;
using CastReporting.UnitTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Repositories
{


    /// <summary>
    ///This is a test class for CRContextTest and is intended
    ///to contain all CRContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CRContextTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        readonly WSConnection _connection = new WSConnection()
        {
            // if using demo-eu-aed, use domain AED1, if using localhost, domain is AED
            Url = "https://demo-eu.castsoftware.com/Engineering/rest/",
            // Url = "http://localhost:8585/CAST-AAD-AED/rest/",
            Login = "cio",
            Password = "cast",
            IsActive = true,
            Name = "Default"
        };

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void IsServiceValidTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
           
            bool result = context.IsServiceValid();
           
            Assert.IsTrue( result);           
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetDomainsTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            IEnumerable<CastDomain> result = context.GetDomains();
            
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(),0);
        }
       
        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetApplicationsByDomainTest()
        {
            string domainHref = "AED1";

            ICastRepsitory context = new CastRepository(_connection, null);
            IEnumerable<Application> result = context.GetApplicationsByDomain(domainHref);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            string appilcationId = "AED1/applications/3";
            Application result = context.GetApplication(appilcationId);
            Assert.IsNotNull(result);
            
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetSnapshotsByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            string appilcationId = "AED1/applications/3";
            var result = context.GetSnapshotsByApplication(appilcationId);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetSnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            string snapshotId = "AED1/applications/3/snapshots/2";
            Snapshot result = context.GetSnapshot(snapshotId);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetModulesByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            string appilcationId = "AED1/applications/3";
            var result = context.GetModules(appilcationId);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetModulesBySnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            string appilcationId = "AED1/applications/3/snapshots/2";
            var result = context.GetModules(appilcationId);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetModuleTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            string moduleId = "AED1/modules/4";
            Module result = context.GetModule(moduleId);
            Assert.IsNotNull(result);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetQualityIndicatorsByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            const string appilcationRef = "AED1/applications/3";

            int[] businessCriterias = (int[])Enum.GetValues(typeof(Constants.BusinessCriteria));
            string strBusinessCriterias = string.Join(",", businessCriterias);

            var result = context.GetResultsQualityIndicators(appilcationRef, strBusinessCriterias, "$all", string.Empty, string.Empty);
            
            
            Assert.IsNotNull(result);
            var _enumerable = result as IList<Result> ?? result.ToList();
            Assert.AreNotEqual(_enumerable.Count, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults.Length, 0);            
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetQualityIndicatorsBySnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            const string snapshotRef = "AED1/applications/3/snapshots/2";

            int[] businessCriterias = (int[])Enum.GetValues(typeof(Constants.BusinessCriteria));
            string strBusinessCriterias = string.Join(",", businessCriterias);

            var result = context.GetResultsQualityIndicators(snapshotRef, strBusinessCriterias, string.Empty, "$all", "$all");


            Assert.IsNotNull(result);
            var _enumerable = result as IList<Result> ?? result.ToList();
            Assert.AreNotEqual(_enumerable.Count, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults.Length, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults[0].ModulesResult.Length, 0);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetQualityIndicatorsByModuleTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            const string moduleRef = "AED1/modules/4";

            int[] businessCriterias = (int[])Enum.GetValues(typeof(Constants.BusinessCriteria));
            string strBusinessCriterias = string.Join(",", businessCriterias);

            var result = context.GetResultsQualityIndicators(moduleRef, strBusinessCriterias, string.Empty, string.Empty, string.Empty);


            Assert.IsNotNull(result);
            var _enumerable = result as IList<Result> ?? result.ToList();
            Assert.AreNotEqual(_enumerable.Count, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults.Length, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults[0].ModulesResult.Length, 0);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetSizingMeasuresByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            const string snapshotRef = "AED1/applications/3";

            int[] sizingMeasures = (int[])Enum.GetValues(typeof(Constants.SizingInformations));
            string strSizingMeasures = string.Join(",", sizingMeasures);

            var result = context.GetResultsSizingMeasures(snapshotRef, strSizingMeasures, "$all", "$all", "$all");

            Assert.IsNotNull(result);
            var _enumerable = result as IList<Result> ?? result.ToList();
            Assert.AreNotEqual(_enumerable.Count, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults.Length, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults[0].TechnologyResult.Length, 0);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetSizingMeasuresBySnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection, null);
            const string snapshotRef = "AED1/applications/3/snapshots/2";

            int[] sizingMeasures = (int[])Enum.GetValues(typeof(Constants.SizingInformations));
            string strSizingMeasures = string.Join(",", sizingMeasures);

            var result = context.GetResultsSizingMeasures(snapshotRef, strSizingMeasures, string.Empty, "$all", "$all");


            Assert.IsNotNull(result);
            var _enumerable = result as IList<Result> ?? result.ToList();
            Assert.AreNotEqual(_enumerable.Count, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults.Length, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults[0].TechnologyResult.Length, 0);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetConfBusinessCriteriaBySnapshot()
        {

            ICastRepsitory context = new CastRepository(_connection, null);
            const string domainHref = "AED1";
            const int snapshotId = 1;

            var result = context.GetConfBusinessCriteriaBySnapshot(domainHref, snapshotId);
            
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        public void GetConfBusinessCriteria()
        {

            ICastRepsitory context = new CastRepository(_connection, null);
            const string domainHref = "AED1/quality-indicators/61001/snapshots/2";

            var result = context.GetConfBusinessCriteria(domainHref);

            Assert.IsNotNull(result);
        }


        [TestMethod()]
        public void GetConfQualityRuleChinese()
        {
            WSConnection _connection3 = new WSConnection()
            {
                Url = "http://dash-aed-tomcat:8585/CAST-Health-Engineering/rest/",
                Login = "cio",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };

            TestUtility.SetCulture("zh-CN");
            ICastRepsitory ccontext = new CastRepository(_connection3, null);
            if ("ABD".Equals(Environment.UserName))
            {
                const string cdomainHref = "AAD/quality-indicators/7126/snapshots/1";
                var result = ccontext.GetConfBusinessCriteria(cdomainHref);
                Assert.AreEqual("避免工件的已注释掉代码行/代码行的比率过高", result.Name);

                TestUtility.SetCulture("en-US");
                ICastRepsitory ccontext2 = new CastRepository(_connection3, null);
                const string cdomainHref2 = "AAD/quality-indicators/7126/snapshots/1";
                var result2 = ccontext2.GetConfBusinessCriteria(cdomainHref2);
                Assert.AreEqual("Avoid Artifacts with high Commented-out Code Lines/Code Lines ratio", result2.Name);
                return;
            }

            bool valid = ccontext.IsServiceValid();
            Assert.IsTrue(valid);
        }

    }
}
