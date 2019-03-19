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

        [TestMethod]
        public void TestGetYear()
        {
            Assert.AreEqual(DateUtil.GetYear(new DateTime(2016,12,12)), 2016);
            Assert.AreEqual(DateUtil.GetYear(new DateTime(2009, 1, 1)), 2009);
        }

        [TestMethod]
        public void TestGetPreviousQuarter()
        {
            Assert.AreEqual(DateUtil.GetPreviousQuarter(new DateTime(2016, 12, 12)), 3);
            Assert.AreEqual(DateUtil.GetPreviousQuarter(new DateTime(2009, 1, 1)), 4);
            Assert.AreEqual(DateUtil.GetPreviousQuarter(new DateTime(2009, 4, 4)), 1);
            Assert.AreEqual(DateUtil.GetPreviousQuarter(new DateTime(2009, 7, 1)), 2);

            Assert.AreEqual(DateUtil.GetPreviousQuarter(4), 3);
            Assert.AreEqual(DateUtil.GetPreviousQuarter(3), 2);
            Assert.AreEqual(DateUtil.GetPreviousQuarter(2), 1);
            Assert.AreEqual(DateUtil.GetPreviousQuarter(1), 4);
        }

        [TestMethod]
        public void TestGetPreviousQuarterYear()
        {
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(new DateTime(2016, 12, 12)), 2016);
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(new DateTime(2009, 1, 1)), 2008);
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(new DateTime(2009, 4, 4)), 2009);
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(new DateTime(2009, 7, 1)), 2009);

            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(4, 2015), 2015);
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(3, 2010), 2010);
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(2, 1999), 1999);
            Assert.AreEqual(DateUtil.GetPreviousQuarterYear(1, 2017), 2016);
        }
    }
}
