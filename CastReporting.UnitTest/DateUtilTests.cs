using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cast.Util.Date;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class DateUtilTests
    {
        [TestMethod]
        public void TestGetQuarter()
        {
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 1, 1)), 1);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 2, 1)), 1);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 3, 1)), 1);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 4, 1)), 2);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 5, 1)), 2);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 6, 1)), 2);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 7, 1)), 3);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 8, 1)), 3);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 9, 1)), 3);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 10, 1)), 4);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 11, 1)), 4);
            Assert.AreEqual(DateUtil.GetQuarter(new DateTime(2016, 12, 1)), 4);
        }
    }
}
