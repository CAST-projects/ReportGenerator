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
            using (ExtendRepository extendRepository = new ExtendRepository(extendUrl, string.Empty, string.Empty, false))
            {
                string latestVersion = extendRepository.PostForLatestVersion("com.castsoftware.qualitystandards", extendUrl);
                Assert.IsNotNull(latestVersion);
            }
        }
    }

}
