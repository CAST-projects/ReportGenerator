using System;
using System.Drawing;
using System.Globalization;
using CastReporting.Domain;

namespace CastReporting.Reporting.Helper
{
    public static class FormatHelper
    {
        public static string NAIfEmpty(this object data)
        {
            var s = data?.ToString();
            return (string.IsNullOrWhiteSpace(s)) ? Constants.No_Value : s;
        }

        public static string NAIfEmpty(this int? data, string format)
        {
            var s = data?.ToString(format) ?? Constants.No_Value;
            return (string.IsNullOrWhiteSpace(s)) ? Constants.No_Value : s;
        }

        /// <summary>
        /// Format the display of percent value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <param name="pWidthPostiveSign"></param>
        /// <returns>Displayed text</returns>
        public static string FormatPercent(this double? pValue, bool pWidthPostiveSign) {
            if (!pValue.HasValue)
                return string.Empty;

            var roundedValue = Math.Round(pValue.Value, 4);
            var sign = (roundedValue > 0 && pWidthPostiveSign) ? "+" : string.Empty;

            var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            var tmp = Math.Abs(roundedValue * 100);
            nfi.PercentDecimalDigits = (Math.Abs(tmp % 1) < 0.00001 || tmp >= 99.945) ? 0 : (tmp >= 9.9945) ? 1 : 2;
            return sign + roundedValue.ToString("P", nfi);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public static string FormatPercent(this double? pValue) {
            return pValue.FormatPercent(true);
        }

        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        public static string FormatEvolution(this long pValue) {
            var sign = (pValue > 0) ? "+" : "";
            return sign + pValue.ToString("N0");
        }

        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        public static string FormatEvolution(this decimal pValue) {
            return FormatEvolution((double?)pValue);
        }

        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65" or "10.4" or "243" or "10 052"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        public static string FormatEvolution(this double? pValue) {
            // this is an inconsistent behaviour compared to other "FormatXxxx" APIs
            // original code was in TableBlock.FormatEvolution() before refactoring
            // the behaviour has not been changed for compatibility with previous versions
            return pValue.HasValue ? pValue.Value.FormatEvolution() : Constants.No_Value; 
        }

        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65" or "10.4" or "243" or "10 052"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        public static string FormatEvolution(this double pValue) {
            var sign = (pValue > 0) ? "+" : string.Empty;
            if (pValue >= 99.945)
                return sign + pValue.ToString("F0");
            else if (pValue >= 9.9945)
                return sign + pValue.ToString("F1");
            else
                return sign + pValue.ToString("F2");
        }

        public static string FormatStringDoubleIntoString(this string pValue)
        {
            double var;
            try
            {
                var = double.Parse(pValue, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                var = double.Parse(pValue, new CultureInfo("en-US"));
            }
            return var.ToString("N2");
        }

        public static Color FormatColor(string myColor)
        {
            switch (myColor)
            {
                case "Gainsboro":
                    return Color.Gainsboro;
                case "White":
                    return Color.White;
                case "Lavender":
                    return Color.Lavender;
                case "LightYellow":
                    return Color.LightYellow;
                case "Beige":
                    return Color.Beige;
                case "Gray":
                    return Color.Gray;
                case "LightGray":
                    return Color.LightGray;
            }
            return Color.White;
        }
    }
}
