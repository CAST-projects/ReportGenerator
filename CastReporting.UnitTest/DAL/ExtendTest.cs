using CastReporting.Repositories.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.DAL
{


    /// <summary>
    ///This is a test class for CRContextTest and is intended
    ///to contain all CRContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExtendTest
    {
        private static string extendUrl = "https://extendng.castsoftware.com";

        [TestMethod()]
        public void Testlatestversionid()
        {
            // https://extendng.castsoftware.com/api/search/packages/EXTENSION-ID/latest
            using (ExtendRepository extendRepository = new ExtendRepository(extendUrl, "123456789"))
            {
                string latestVersion = extendRepository.PostForLatestVersion("com.castsoftware.qualitystandards", extendUrl);
                Assert.IsNotNull(latestVersion);
            }
        }

        [TestMethod()]
        public void TestDownloadlatestversionid()
        {
            // https://extendng.castsoftware.com/api/search/packages/EXTENSION-ID/latest
            using (ExtendRepository extendRepository = new ExtendRepository(extendUrl, "ca2cc61f-d0c7-4056-9c44-3142e0bb5bfc"))
            {
                extendRepository.GetLatestVersion("com.castsoftware.qualitystandards", extendUrl, "20190708.0.0-funcrel");
            }
        }
    }

}
