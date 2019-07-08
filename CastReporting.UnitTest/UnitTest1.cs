using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;
using Cast.Util;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {

            const double pValue = 1.1401;
            // ReSharper disable once UnreachableCode
            const string sign = pValue > 0 ? "+" : "";
            var roundedValue = Math.Round(pValue, 4);
            NumberFormatInfo nfi = (NumberFormatInfo) CultureInfo.CurrentCulture.NumberFormat.Clone();
            var tmp = roundedValue*100;
            nfi.PercentDecimalDigits = Math.Abs(tmp%1) < 0 || tmp >= 100 ? 0 : tmp >= 0.1 ? 2 : 1;
            // ReSharper disable once UnusedVariable
            var r = sign + roundedValue.ToString("P", nfi);

        }

        [TestMethod]
        public void TestMatrice()
        {
            var key = Tuple.Create(1234, "JEE", 60017);
            var values = new Dictionary<Tuple<int, string, int>, double> {[key] = 2.35};

            var key2 = Tuple.Create(1234, "JEE", 60017);
            Console.WriteLine(values[key]);
            Console.WriteLine(values[key2]);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Templates.zip", "Data")]
        public void TestUnzipAndCopy()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "RGtests_" + DateTime.Today.ToString("yyyyMMdd"));
            if (Directory.Exists(tempDirectory))
            {
                File.SetAttributes(tempDirectory, FileAttributes.Normal);
                Directory.Delete(tempDirectory, true);
            }
            Directory.CreateDirectory(tempDirectory);
            File.SetAttributes(tempDirectory, FileAttributes.Normal);
            PathUtil.UnzipAndCopy(@".\Data\Templates.zip",tempDirectory);
            Assert.IsTrue(Directory.Exists(Path.Combine(tempDirectory, "Templates","Portfolio")));
        }

    }
}
