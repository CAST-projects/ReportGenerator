using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastReporting.BLL.Computing
{
    static public class MeasureUtility
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetSizingMeasure(Snapshot snapshot, Constants.SizingInformations measureId)
        {

            if (null != snapshot && snapshot.SizingMeasuresResults != null)
            {
                var measure = snapshot.SizingMeasuresResults.Where(_ => _.Reference.Key == measureId.GetHashCode())
                                                              .FirstOrDefault();


                if (null != measure && measure.DetailResult != null)
                    return MathUtility.GetRound(measure.DetailResult.Value);
            }

            return null;
        }
        public static Double? GetSizingMeasure(Snapshot snapshot, int measureId)
        {

            if (null != snapshot && snapshot.SizingMeasuresResults != null)
            {
                var measure = snapshot.SizingMeasuresResults.Where(_ => _.Reference.Key == measureId)
                                                              .FirstOrDefault();


                if (null != measure && measure.DetailResult != null)
                    return measure.DetailResult.Value;
            }

            return null;
        }


        public static Double? GetAddedFunctionPoint(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.AddedFunctionPoints);
        }


        public static Double? GetDeletedFunctionPoint(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.DeletedFunctionPoints);
        }


        public static Double? GetModifiedFunctionPoint(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.ModifiedFunctionPoints);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetAfpMetricDF(Snapshot snapshot)
        {         
            return GetSizingMeasure(snapshot, Constants.SizingInformations.UnadjustedDataFunctions);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetAfpMetricTF(Snapshot snapshot)
        {          
            return  GetSizingMeasure(snapshot, Constants.SizingInformations.UnadjustedTransactionalFunctions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetTechnicalDebtMetric(Snapshot snapshot)
        {            
            return GetSizingMeasure(snapshot, Constants.SizingInformations.TechnicalDebt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetRemovedTechDebtMetric(Snapshot snapshot)
        {           
            return GetSizingMeasure(snapshot, Constants.SizingInformations.RemovedViolationsTechnicalDebt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetAddedTechDebtMetric(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.AddedViolationsTechnicalDebt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetFileNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.FileNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetClassNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.ClassNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetSqlArtifactNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.SQLArtifactNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetTableNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.TableNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetAutomatedIFPUGFunction(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.AutomatedIFPUGFunctionPointsEstimation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetDecisionPointsNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.DecisionPointsNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetBackfiredIFPUGFunction(Snapshot snapshot)
        {           
            return GetSizingMeasure(snapshot, Constants.SizingInformations.BackfiredIFPUGFunctionPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Double? GetCodeLineNumber(Snapshot snapshot)
        {           
            return GetSizingMeasure(snapshot, Constants.SizingInformations.CodeLineNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static Int32? GetAddedCriticalViolations(Snapshot snapshot)
        {          
            return (Int32?)GetSizingMeasure(snapshot, Constants.SizingInformations.CriticalQualityRulesWithViolationsNewModifiedCodeNumber);
        }
        public static Int32? GetCriticalViolations(Snapshot snapshot)
        {//CriticalQualityRulesWithViolationsNumber
            return (Int32?)GetSizingMeasure(snapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static double? GetSumMetric(Snapshot snapshot, int index)
        {
            if (null != snapshot && snapshot.SizingMeasuresResults != null)
            {

                var result = snapshot.SizingMeasuresResults.Where(_ => _.Reference.Key == index && _.DetailResult != null)
                                                           .Sum(_ => _.DetailResult.Value);

                return result;

            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrentSnapshot"></param>
        /// <param name="PreviousSnapshot"></param>
        /// <param name="application"></param>
        /// <param name="indicatorId"></param>
        /// <returns></returns>
        static public Double? SumDeltaIndicator(Snapshot CurrentSnapshot, Snapshot PreviousSnapshot, Application application, Constants.SizingInformations measureId)
        {        
            double? result = null;
            if(application != null && CurrentSnapshot != null && CurrentSnapshot.SizingMeasuresResults != null)
            {             
                result = GetSizingMeasure(CurrentSnapshot, measureId);
                                    
                if (PreviousSnapshot != null && PreviousSnapshot.Annotation.Date.DateSnapShot.HasValue && PreviousSnapshot.SizingMeasuresResults != null )
                {
                    DateTime dtPrevoiusSnapshot = PreviousSnapshot.Annotation.Date.DateSnapShot.Value;
                    DateTime dtCurrentSnapshot = CurrentSnapshot.Annotation.Date.DateSnapShot.Value;

                    var quryPreviusIndicators = from s in application.Snapshots
                                                where s.Annotation.Date.DateSnapShot > dtPrevoiusSnapshot 
                                                &&    s.Annotation.Date.DateSnapShot < dtCurrentSnapshot
                                                from i in s.SizingMeasuresResults
                                                where i.Reference.Key == measureId.GetHashCode()
                                                select i;
             
                    result += quryPreviusIndicators.Sum(s => s.DetailResult.Value);
                }
            }
            return result;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static List<TechnologyResultDTO> GetSizingMeasureTechnologies(Snapshot snapshot, string hrefModule, Int32 measureId)
        {
            List<TechnologyResultDTO> result = new List<TechnologyResultDTO>();

            if (snapshot != null && snapshot.SizingMeasuresResults != null)
            {
                ApplicationResult applicationResult = snapshot.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == measureId && _.ModulesResult != null);

                ModuleResult modulesResult = null;

                if (applicationResult != null)
                    modulesResult = applicationResult.ModulesResult.FirstOrDefault(_ => _.Module.Href == hrefModule);

                if (modulesResult != null)
                    result = modulesResult.TechnologyResults.Select(_ => new TechnologyResultDTO { Name = _.Technology, Value = _.DetailResult.Value }).ToList();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="nbResult"></param>
        /// <returns></returns>
        public static List<TechnologyResultDTO> GetTechnoLoc(Snapshot snapshot, int nbResult)
        {
            if (null != snapshot && null != snapshot.Technologies && snapshot.SizingMeasuresResults != null)
            {

                List<TechnologyResultDTO> technologyInfos = (from techno in snapshot.Technologies
                                                                       .Where(_ => !(_.StartsWith("APM") && _.EndsWith("Module")))
                                                             from codeLineNumber in snapshot.SizingMeasuresResults
                                                             where codeLineNumber.Reference.Key == Constants.SizingInformations.CodeLineNumber.GetHashCode() &&
                                                                   codeLineNumber.DetailResult?.Value > 0 &&
                                                                   codeLineNumber.TechnologyResult.Where(_ => _.Technology.Equals(techno)).Count() != 0
                                                             select new TechnologyResultDTO
                                                             {
                                                                 Name = techno,
                                                                 Value = (null == codeLineNumber) ? -1 : codeLineNumber.TechnologyResult.Where(_ => _.Technology.Equals(techno)).FirstOrDefault().DetailResult.Value

                                                             }).OrderByDescending(_ => _.Value).Take(nbResult).ToList();
                return technologyInfos;

            }
            return null;
        }


        public static List<TechnologyResultDTO> GetTechnoClasses(Snapshot snapshot, int nbResult)
        {
            if (null != snapshot && null != snapshot.Technologies && snapshot.SizingMeasuresResults != null)
            {

                List<TechnologyResultDTO> technologyInfos = (from techno in snapshot.Technologies
                                                                       .Where(_ => !(_.StartsWith("APM") && _.EndsWith("Module")))
                                                             from codeLineNumber in snapshot.SizingMeasuresResults
                                                             where codeLineNumber.Reference.Key == Constants.SizingInformations.ClassNumber.GetHashCode() &&
                                                                   codeLineNumber.DetailResult.Value > 0
                                                             select new TechnologyResultDTO
                                                             {
                                                                 Name = techno,
                                                                 Value = null == codeLineNumber ? -1 : codeLineNumber.TechnologyResult.Where(_ => _.Technology.Equals(techno)).FirstOrDefault().DetailResult.Value

                                                             }).OrderByDescending(_ => _.Value).Take(nbResult).ToList();
                return technologyInfos;

            }
            return null;
        }


        public static List<TechnologyResultDTO> GetTechnoComplexity(Snapshot snapshot, int nbResult)
        {
            if (null != snapshot && null != snapshot.Technologies && snapshot.SizingMeasuresResults != null)
            {

                List<TechnologyResultDTO> technologyInfos = (from techno in snapshot.Technologies
                                                                       .Where(_ => !(_.StartsWith("APM") && _.EndsWith("Module")))
                                                             from codeLineNumber in snapshot.SizingMeasuresResults
                                                             where codeLineNumber.Reference.Key == Constants.SizingInformations.DecisionPointsNumber.GetHashCode() &&
                                                                   codeLineNumber.DetailResult.Value > 0
                                                             select new TechnologyResultDTO
                                                             {
                                                                 Name = techno,
                                                                 Value = null == codeLineNumber ? -1 : codeLineNumber.TechnologyResult.Where(_ => _.Technology.Equals(techno)).FirstOrDefault().DetailResult.Value

                                                             }).OrderByDescending(_ => _.Value).Take(nbResult).ToList();
                return technologyInfos;

            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="moduleHRef"></param>
        /// <param name="bcId"></param>
        /// <returns></returns>
        public static Double GetModuleMeasureGrade(Snapshot snapshot, Int32 moduleId, Constants.SizingInformations measureId)
        {
            return snapshot.SizingMeasuresResults
                           .Where(_ => _.Reference.Key == (Int32)measureId && _.ModulesResult != null)
                           .SelectMany(_ => _.ModulesResult)
                           .Where(_ => _.Module.Id == moduleId && _.DetailResult != null)
                           .Select(_ => MathUtility.GetRound(_.DetailResult.Grade).Value)
                           .FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="nbResult"></param>
        /// <returns></returns>
        public static List<ModuleResultDTO> GetModulesMeasure(Snapshot snapshot, int nbResult, Constants.SizingInformations measureId)
        {
            if(snapshot !=null && snapshot.SizingMeasuresResults != null)
            {
                return snapshot.SizingMeasuresResults
                               .Where(_ => _.Reference.Key == measureId.GetHashCode())
                               .SelectMany(_ => _.ModulesResult)                           
                               .Where(m => m.DetailResult != null)
                               .Select( _ => new ModuleResultDTO { Name = _.Module.Name, Value= _.DetailResult.Value})
                               .OrderByDescending(_ => _.Value)
                               .Take(nbResult)                           
                               .ToList();
            }

            return null;
        }
    }
}
