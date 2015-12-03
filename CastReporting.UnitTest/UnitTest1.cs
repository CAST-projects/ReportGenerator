using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            double pValue = 1.1401;
            
            string sign = (pValue > 0) ? "+" : "";


            var roundedValue = Math.Round(pValue, 4);

            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();

            var tmp = roundedValue * 100;
            nfi.PercentDecimalDigits = (tmp % 1 == 0 || tmp >= 100) ? 0 : (tmp >= 0.1) ? 2 : 1;



            var r =  sign + roundedValue.ToString("P", nfi);
        }
    }
}
