using CastReporting.Domain;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    public static class CastComplexityUtility
    {

        /// <summary>
        /// return Fack Value --> Mock should be deleted on S06
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="categorieId"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        public static double? GetCostComplexityGrade(Snapshot snapshot, int categorieId, int metricId)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            var category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.key == metricId);
            return (category != null) ? MathUtility.GetRound(category.Value) : 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="categorieId"></param>
        /// <returns></returns>
        public static string GetCostComplexityName(Snapshot snapshot, int categorieId)
        {
            var result = snapshot.CostComplexityResults.FirstOrDefault(_ => _.Reference.Key == categorieId);
            return result?.Reference?.Name;
        }
       
    }
}
