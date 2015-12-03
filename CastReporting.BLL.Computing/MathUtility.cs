using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastReporting.BLL.Computing
{
    public static class MathUtility
    {
        /// <summary>
        /// /
        /// </summary>
        /// <param name="baseval"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static double GetVerticalMinValue(double baseval, double step)
        {
            return Math.Floor((baseval - step) * 10) / 10;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseval"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static double GetVerticalMaxValue(double baseval, double step)
        {
            return Math.Ceiling((baseval + step) * 10) / 10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currMesure"></param>
        /// <param name="prevMesure"></param>
        /// <returns></returns>
        public static Double? GetEvolution(double? currMesure, double? prevMesure)
        {
            return (currMesure.HasValue && prevMesure.HasValue) ? (currMesure.Value - prevMesure.Value) : (Double?)null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currMesure"></param>
        /// <param name="prevMesure"></param>
        /// <returns></returns>
        public static Double? GetSum(double? currMesure, double? prevMesure)
        {
            return (currMesure.HasValue && prevMesure.HasValue) ? (currMesure.Value + prevMesure.Value) : (Double?)null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Double? GetPercent(double? value, double? reference)
        {
            return (value.HasValue && reference.HasValue && reference.Value != 0)?(value.Value / reference.Value): (Double?)null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Double? GetVariationPercent(double? currMesure, double? prevMesure)
        {
            var variaton = GetEvolution(currMesure, prevMesure);
            return (variaton.HasValue && prevMesure.HasValue && prevMesure.Value > 0) ? (variaton / prevMesure) : (Double?)null;
        }

        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double? GetRound(double? number)
        {
            double? value = null;

            if (number.HasValue)
                value = Math.Round(number.Value, 2);

            return value;
        }
    }
}
