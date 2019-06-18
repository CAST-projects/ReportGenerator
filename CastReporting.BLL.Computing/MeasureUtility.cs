using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    public static class MeasureUtility
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="measureId"></param>
        /// <returns></returns>
        public static double? GetSizingMeasure(Snapshot snapshot, Constants.SizingInformations measureId)
        {
            var measure = snapshot?.SizingMeasuresResults?.FirstOrDefault(_ => _.Reference.Key == measureId.GetHashCode());
            return measure?.DetailResult != null ? MathUtility.GetRound(measure.DetailResult.Value) : null;
        }

        public static double? GetSizingMeasure(Snapshot snapshot, int measureId)
        {
            var measure =snapshot?.SizingMeasuresResults?.FirstOrDefault(_ => _.Reference.Key == measureId);
            return measure?.DetailResult?.Value;
        }

        public static string GetSizingMeasureName(Snapshot snapshot, int measureId, bool shortname = false)
        {
            var measure = snapshot?.SizingMeasuresResults?.FirstOrDefault(_ => _.Reference.Key == measureId);
            var name = (shortname ? measure?.Reference.ShortName : measure?.Reference.Name) ?? measure?.Reference.Name;
            return name;
        }

        public static double? GetSizingMeasureModule(Snapshot snapshot, int moduleId, int measureId)
        {
            var measure = snapshot?.SizingMeasuresResults?.FirstOrDefault(_ => _.Reference.Key == measureId);
            return measure?.ModulesResult.FirstOrDefault(_=>_.Module.Id == moduleId)?.DetailResult?.Value;
        }

        public static double? GetAddedFunctionPoint(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.AddedFunctionPoints);
        }


        public static double? GetDeletedFunctionPoint(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.DeletedFunctionPoints);
        }


        public static double? GetModifiedFunctionPoint(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.ModifiedFunctionPoints);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static double? GetAfpMetricDF(Snapshot snapshot)
        {         
            return GetSizingMeasure(snapshot, Constants.SizingInformations.UnadjustedDataFunctions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static double? GetAfpMetricTF(Snapshot snapshot)
        {          
            return  GetSizingMeasure(snapshot, Constants.SizingInformations.UnadjustedTransactionalFunctions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetTechnicalDebtMetric(Snapshot snapshot)
        {            
            return GetSizingMeasure(snapshot, Constants.SizingInformations.TechnicalDebt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetRemovedTechDebtMetric(Snapshot snapshot)
        {           
            return GetSizingMeasure(snapshot, Constants.SizingInformations.RemovedViolationsTechnicalDebt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetAddedTechDebtMetric(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.AddedViolationsTechnicalDebt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetFileNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.FileNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetClassNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.ClassNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetSqlArtifactNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.SQLArtifactNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetTableNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.TableNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static double? GetAutomatedIFPUGFunction(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.AutomatedIFPUGFunctionPointsEstimation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetDecisionPointsNumber(Snapshot snapshot)
        {
            return GetSizingMeasure(snapshot, Constants.SizingInformations.DecisionPointsNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static double? GetBackfiredIFPUGFunction(Snapshot snapshot)
        {           
            return GetSizingMeasure(snapshot, Constants.SizingInformations.BackfiredIFPUGFunctionPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static double? GetCodeLineNumber(Snapshot snapshot)
        {           
            return GetSizingMeasure(snapshot, Constants.SizingInformations.CodeLineNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static int? GetAddedCriticalViolations(Snapshot snapshot)
        {          
            return (int?)GetSizingMeasure(snapshot, Constants.SizingInformations.CriticalQualityRulesWithViolationsNewModifiedCodeNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSnapshot"></param>
        /// <param name="previousSnapshot"></param>
        /// <param name="application"></param>
        /// <param name="measureId"></param>
        /// <returns></returns>
        public static double? SumDeltaIndicator(Snapshot currentSnapshot, Snapshot previousSnapshot, Application application, Constants.SizingInformations measureId)
        {        
            if (application == null || currentSnapshot?.SizingMeasuresResults == null) return null;
            double? result = GetSizingMeasure(currentSnapshot, measureId);

            if (previousSnapshot?.Annotation.Date.DateSnapShot == null || previousSnapshot.SizingMeasuresResults == null || currentSnapshot.Annotation.Date.DateSnapShot == null)
                return result;
            DateTime dtPrevoiusSnapshot = previousSnapshot.Annotation.Date.DateSnapShot.Value;
            DateTime dtCurrentSnapshot = currentSnapshot.Annotation.Date.DateSnapShot.Value;

            var quryPreviusIndicators = from s in application.Snapshots
                where s.Annotation.Date.DateSnapShot > dtPrevoiusSnapshot 
                      &&    s.Annotation.Date.DateSnapShot < dtCurrentSnapshot
                from i in s.SizingMeasuresResults
                where i.Reference.Key == measureId.GetHashCode()
                select i;
             
            result += quryPreviusIndicators.Sum(s => s.DetailResult.Value);
            return result;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="hrefModule"></param>
        /// <param name="measureId"></param>
        /// <returns></returns>
        public static List<TechnologyResultDTO> GetSizingMeasureTechnologies(Snapshot snapshot, string hrefModule, int measureId)
        {
            List<TechnologyResultDTO> result = new List<TechnologyResultDTO>();

            if (snapshot?.SizingMeasuresResults == null) return result;
            ApplicationResult applicationResult = snapshot.SizingMeasuresResults.FirstOrDefault(_ => _.Reference.Key == measureId && _.ModulesResult != null);

            ModuleResult modulesResult = null;

            if (applicationResult != null)
                modulesResult = applicationResult.ModulesResult.FirstOrDefault(_ => _.Module.Href == hrefModule);

            if (modulesResult != null)
                result = modulesResult.TechnologyResults.Select(_ => new TechnologyResultDTO { Name = _.Technology, Value = _.DetailResult.Value }).ToList();

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
            if (snapshot?.Technologies == null || snapshot.SizingMeasuresResults == null) return null;
            List<TechnologyResultDTO> technologyInfos = (from techno in snapshot.Technologies
                    .Where(_ => !(_.StartsWith("APM") && _.EndsWith("Module")))
                from codeLineNumber in snapshot.SizingMeasuresResults
                where codeLineNumber.Reference.Key == Constants.SizingInformations.CodeLineNumber.GetHashCode() &&
                      codeLineNumber.DetailResult?.Value > 0 &&
                      codeLineNumber.TechnologyResult.Count(_ => _.Technology.Equals(techno)) != 0
                select new TechnologyResultDTO
                {
                    Name = techno,
                    Value = codeLineNumber?.TechnologyResult?.FirstOrDefault(_ => _.Technology.Equals(techno))?.DetailResult.Value ?? -1

                }).OrderByDescending(_ => _.Value).Take(nbResult).ToList();
            return technologyInfos;
        }


        public static List<TechnologyResultDTO> GetTechnoClasses(Snapshot snapshot, int nbResult)
        {
            if (snapshot?.Technologies == null || snapshot.SizingMeasuresResults == null) return null;
            List<TechnologyResultDTO> technologyInfos = (from techno in snapshot.Technologies
                    .Where(_ => !(_.StartsWith("APM") && _.EndsWith("Module")))
                from codeLineNumber in snapshot.SizingMeasuresResults
                where codeLineNumber.Reference.Key == Constants.SizingInformations.ClassNumber.GetHashCode() &&
                      codeLineNumber.DetailResult.Value > 0
                select new TechnologyResultDTO
                {
                    Name = techno,
                    Value = codeLineNumber?.TechnologyResult?.FirstOrDefault(_ => _.Technology.Equals(techno))?.DetailResult.Value ?? -1
                }).OrderByDescending(_ => _.Value).Take(nbResult).ToList();
            return technologyInfos;
        }


        public static List<TechnologyResultDTO> GetTechnoComplexity(Snapshot snapshot, int nbResult)
        {
            if (snapshot?.Technologies == null || snapshot.SizingMeasuresResults == null) return null;
            List<TechnologyResultDTO> technologyInfos = (from techno in snapshot.Technologies
                    .Where(_ => !(_.StartsWith("APM") && _.EndsWith("Module")))
                from codeLineNumber in snapshot.SizingMeasuresResults
                where codeLineNumber.Reference.Key == Constants.SizingInformations.DecisionPointsNumber.GetHashCode() &&
                      codeLineNumber.DetailResult.Value > 0
                select new TechnologyResultDTO
                {
                    Name = techno,
                    Value = codeLineNumber?.TechnologyResult?.FirstOrDefault(_ => _.Technology.Equals(techno))?.DetailResult.Value ?? -1

                }).OrderByDescending(_ => _.Value).Take(nbResult).ToList();
            return technologyInfos;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="nbResult"></param>
        /// <param name="measureId"></param>
        /// <returns></returns>
        public static List<ModuleResultDTO> GetModulesMeasure(Snapshot snapshot, int nbResult, Constants.SizingInformations measureId)
        {
            if(snapshot?.SizingMeasuresResults != null)
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
