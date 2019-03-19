using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cast.Util.Version;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class VersionUtilTests
    {
        [TestMethod]
        public void TestExtract()
        {
            int expected = 7;
            int actual = VersionUtil.ExtractVersionNumber("7.2.0",0);
            Assert.AreEqual(expected, actual);

            expected = 2;
            actual = VersionUtil.ExtractVersionNumber("7.2.0", 1);
            Assert.AreEqual(expected, actual);

            expected = 0;
            actual = VersionUtil.ExtractVersionNumber("7.2.0", 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test82Compliant()
        {
            Assert.IsTrue(VersionUtil.IsAdgVersion82Compliant("8.2.0"));
            Assert.IsTrue(VersionUtil.IsAdgVersion82Compliant("9.0.0"));
            Assert.IsFalse(VersionUtil.IsAdgVersion82Compliant("8.1.0"));
            Assert.IsFalse(VersionUtil.IsAdgVersion82Compliant("7.3.0"));
        }
    }
}
