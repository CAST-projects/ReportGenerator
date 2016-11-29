using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Repositories;
using CastReporting.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTests
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
            Url = "http://demo-eu-aed.castsoftware.com/AED/rest/",
            Login = "admin",
            Password = "cast",
            IsActive = true,
            Name = "Default"
        };

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void IsServiceValidTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
           
            bool result = context.IsServiceValid();
           
            Assert.IsTrue( result);           
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetDomainsTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            IEnumerable<CastDomain> result = context.GetDomains();
            
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(),0);
        }
       
        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetApplicationsByDomainTest()
        {
            string domainHref = "AED1";

            ICastRepsitory context = new CastRepository(_connection);
            IEnumerable<Application> result = context.GetApplicationsByDomain(domainHref);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            string appilcationId = "AED1/applications/3";
            Application result = context.GetApplication(appilcationId);
            Assert.IsNotNull(result);
            
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetSnapshotsByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            string appilcationId = "AED1/applications/3";
            var result = context.GetSnapshotsByApplication(appilcationId);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetSnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            string snapshotId = "AED1/applications/3/snapshots/3";
            Snapshot result = context.GetSnapshot(snapshotId);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetModulesByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            string appilcationId = "AED1/applications/3";
            var result = context.GetModules(appilcationId);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetModulesBySnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            string appilcationId = "AED1/applications/3/snapshots/3";
            var result = context.GetModules(appilcationId);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count(), 0);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetModuleTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            string moduleId = "AED1/modules/4";
            Module result = context.GetModule(moduleId);
            Assert.IsNotNull(result);
        }


        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetQualityIndicatorsByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            const string appilcationRef = "AED1/applications/3";

            int[] businessCriterias = (int[])Enum.GetValues(typeof(Constants.BusinessCriteria));
            string strBusinessCriterias = string.Join(",", businessCriterias);

            var result = context.GetResultsQualityIndicators(appilcationRef, strBusinessCriterias, "$all", string.Empty, string.Empty, string.Empty);
            
            
            Assert.IsNotNull(result);
            var _enumerable = result as IList<Result> ?? result.ToList();
            Assert.AreNotEqual(_enumerable.Count, 0);
            Assert.AreNotEqual(_enumerable.First().ApplicationResults.Length, 0);            
        }

        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetQualityIndicatorsBySnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            const string snapshotRef = "AED1/applications/3/snapshots/3";

            int[] businessCriterias = (int[])Enum.GetValues(typeof(Constants.BusinessCriteria));
            string strBusinessCriterias = string.Join(",", businessCriterias);

            var result = context.GetResultsQualityIndicators(snapshotRef, strBusinessCriterias, string.Empty, "$all", "$all", "$all");


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
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetQualityIndicatorsByModuleTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            const string moduleRef = "AED1/modules/4";

            int[] businessCriterias = (int[])Enum.GetValues(typeof(Constants.BusinessCriteria));
            string strBusinessCriterias = string.Join(",", businessCriterias);

            var result = context.GetResultsQualityIndicators(moduleRef, strBusinessCriterias, string.Empty, string.Empty, string.Empty, string.Empty);


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
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetSizingMeasuresByApplicationTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
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
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetSizingMeasuresBySnapshotTest()
        {
            ICastRepsitory context = new CastRepository(_connection);
            const string snapshotRef = "AED1/applications/3/snapshots/3";

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
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetConfBusinessCriteriaBySnapshot()
        {

            ICastRepsitory context = new CastRepository(_connection);
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
        [DeploymentItem("CastReporting.DAL.dll")]
        public void GetConfBusinessCriteria()
        {

            ICastRepsitory context = new CastRepository(_connection);
            const string domainHref = "AED1/quality-indicators/61001/snapshots/3";

            var result = context.GetConfBusinessCriteria(domainHref);

            Assert.IsNotNull(result);
        }
    }
}
