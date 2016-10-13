using System;
using System.Globalization;
using CastReporting.Domain;

namespace CastReporting.Reporting.Helper
{
    public static class FormatHelper
    {
        public static string Empty(this object data) {
            var s = data?.ToString();
            return string.IsNullOrWhiteSpace(s) ? string.Empty : s;
        }

        public static string NaIfEmpty(this object data) {
            var s = data?.ToString();
            return string.IsNullOrWhiteSpace(s) ? Constants.No_Value : s;
        }

        public static string DashIfEmpty(this object data) {
            var s = data?.ToString();
            return (string.IsNullOrWhiteSpace(s) || s == Constants.No_Value) ? Constants.No_Data : s;
        }

        /// <summary>
        /// Format the display of percent value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        public static string FormatPercent(this double? pValue, bool pWidthPostiveSign) {
            if (!pValue.HasValue)
                return string.Empty;

            var roundedValue = Math.Round(pValue.Value, 4);
            var sign = (roundedValue > 0 && pWidthPostiveSign) ? "+" : string.Empty;

            var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            var tmp = Math.Abs(roundedValue * 100);
            nfi.PercentDecimalDigits = (tmp % 1 == 0 || tmp >= 99.945) ? 0 : (tmp >= 9.9945) ? 1 : 2;
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
        public static string FormatEvolution(this long? pValue) {
            return pValue.HasValue ? pValue.Value.FormatEvolution() : string.Empty;
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
        public static string FormatEvolution(this decimal? pValue) {
            return FormatEvolution((double?)pValue);
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
            return pValue.HasValue ? pValue.Value.FormatEvolution() : Constants.No_Data; 
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
    }
}
