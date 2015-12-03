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
        /// <param name="index"></param>
        /// <returns></returns>
        public static double? GetCostComplexityGrade(Snapshot snapshot, int categorieId, int metricId)
        {
            double? value = null;
            if (null != snapshot && snapshot.CostComplexityResults != null)
            {
                var result = snapshot.CostComplexityResults.Where(_ => _.Reference.Key == categorieId).FirstOrDefault();

                if (result != null && result.DetailResult.Categories != null)
                {
                    var category = result.DetailResult.Categories
                                                      .Where(_ => _.key == metricId)
                                                      .FirstOrDefault();

                    return (category != null) ? MathUtility.GetRound(category.Value) : 0;
                }
            }


            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="CategorieIndex"></param>
        /// <param name="Metricindex"></param>
        /// <returns></returns>
        public static string GetCostComplexityName(Snapshot snapshot, int categorieId)
        {
            var result = snapshot.CostComplexityResults.Where(_ => _.Reference.Key == categorieId)
                                                       .FirstOrDefault();

            return (result != null && result.Reference!=null) ? result.Reference.Name : null;
        }
       
    }
}
