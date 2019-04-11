using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cast.Util.Version;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class VersionUtilTests
    {
        [TestMethod]
        public void Test82Compliant()
        {
            Assert.IsTrue(VersionUtil.IsAdgVersion82Compliant("8.2.0"));
            Assert.IsTrue(VersionUtil.IsAdgVersion82Compliant("9.0.0"));
            Assert.IsFalse(VersionUtil.IsAdgVersion82Compliant("8.1.0"));
            Assert.IsFalse(VersionUtil.IsAdgVersion82Compliant("7.3.0"));
        }

        [TestMethod]
        public void Test833Compliant()
        {
            Assert.IsTrue(VersionUtil.IsAdgVersion833Compliant("8.3.3"));
            Assert.IsTrue(VersionUtil.IsAdgVersion833Compliant("8.3.4"));
            Assert.IsTrue(VersionUtil.IsAdgVersion833Compliant("9.0.0"));
            Assert.IsFalse(VersionUtil.IsAdgVersion833Compliant("8.2.0"));
            Assert.IsFalse(VersionUtil.IsAdgVersion833Compliant("7.3.0"));
        }

        [TestMethod]
        public void Test19Compliant()
        {
            Assert.IsFalse(VersionUtil.Is19Compatible("1.8.0.355"));
            Assert.IsTrue(VersionUtil.Is19Compatible("1.9.0.457"));
            Assert.IsFalse(VersionUtil.Is19Compatible("1.7.0.355"));
            Assert.IsFalse(VersionUtil.Is17Compatible("1.6.0.355"));
        }

        [TestMethod]
        public void Test18Compliant()
        {
            Assert.IsTrue(VersionUtil.Is18Compatible("1.8.0.256"));
            Assert.IsTrue(VersionUtil.Is18Compatible("1.9.0.457"));
            Assert.IsFalse(VersionUtil.Is19Compatible("1.7.0.355"));
            Assert.IsFalse(VersionUtil.Is17Compatible("1.6.0.355"));
        }

        [TestMethod]
        public void Test17Compliant()
        {
            Assert.IsTrue(VersionUtil.Is17Compatible("1.8.0.256"));
            Assert.IsTrue(VersionUtil.Is17Compatible("1.9.0.457"));
            Assert.IsTrue(VersionUtil.Is17Compatible("1.7.0.355"));
            Assert.IsFalse(VersionUtil.Is17Compatible("1.6.0.355"));
        }
    }
}
