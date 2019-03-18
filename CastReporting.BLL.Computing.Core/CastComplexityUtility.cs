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
            return (category != null) ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetCostComplexityGrade(Snapshot snapshot, int categorieId, string position)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            Category category = null;
            switch (position)
            {
                case "low":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("low"))
                               ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("small"));
                    break;
                case "average":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("average"))
                        ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("moderate"));
                    break;
                case "high":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && !(_.Name.ToLower().Contains("very")))
                               ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && !(_.Name.ToLower().Contains("very")));
                    break;
                case "very_high":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && _.Name.ToLower().Contains("very"))
                               ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && _.Name.ToLower().Contains("very"));
                    break;
            }

            return (category != null) ? MathUtility.GetRound(category.Value) : null;
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
