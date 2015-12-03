/*
 *   Copyright (c) 2015 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;

namespace CastReporting.BLL.Computing
{
    public class BusinessCriteriaDTO
    {
               
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double? TQI { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double? Robustness { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double? Performance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double? Security { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Double? Changeability { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public Double? Transferability { get; set; }


        /// <summary>
        /// /
        /// </summary>
        public Double? ProgrammingPractices { get; set; }

        
        /// <summary>
        /// /
        /// </summary>
        public Double? ArchitecturalDesign { get; set; }

        
        /// <summary>
        /// /
        /// </summary>
        public Double? Documentation { get; set; }

        
        /// <summary>
        /// /
        /// </summary>
        public Double? SEIMaintainability { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BusinessCriteriaDTO operator -(BusinessCriteriaDTO left, BusinessCriteriaDTO right)
        {
            BusinessCriteriaDTO GetBusinessCriteriaGradesVartiation = new BusinessCriteriaDTO();
            if (left != null && right != null)
            {
                GetBusinessCriteriaGradesVartiation.TQI = (left.TQI.HasValue &&  right.TQI.HasValue)?left.TQI - right.TQI:null;
                GetBusinessCriteriaGradesVartiation.Robustness = (left.Robustness.HasValue && right.Robustness.HasValue) ? left.Robustness - right.Robustness : null; ;
                GetBusinessCriteriaGradesVartiation.Performance = (left.Performance.HasValue && right.Performance.HasValue) ? left.Performance - right.Performance : null; ;
                GetBusinessCriteriaGradesVartiation.Security = (left.Security.HasValue && right.Security.HasValue) ? left.Security - right.Security : null; ;
                GetBusinessCriteriaGradesVartiation.Transferability = (left.Transferability.HasValue && right.Transferability.HasValue) ? left.Transferability - right.Transferability : null; ;
                GetBusinessCriteriaGradesVartiation.Changeability = (left.Changeability.HasValue && right.Changeability.HasValue) ? left.Changeability - right.Changeability : null; ;
            }
            return GetBusinessCriteriaGradesVartiation;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BusinessCriteriaDTO operator /(BusinessCriteriaDTO left, BusinessCriteriaDTO right)
        {
            BusinessCriteriaDTO GetBusinessCriteriaGradesVartiation = new BusinessCriteriaDTO();
            if (left != null && left != null)
            {
                GetBusinessCriteriaGradesVartiation.TQI = (0 == right.TQI ? 0 : left.TQI / right.TQI);
                GetBusinessCriteriaGradesVartiation.Robustness = (0 == right.Robustness ? 0 : left.Robustness / right.Robustness);
                GetBusinessCriteriaGradesVartiation.Performance = (0 == right.Performance ? 0 : left.Performance / right.Performance);
                GetBusinessCriteriaGradesVartiation.Security = (0 == right.Security ? 0 : left.Security / right.Security);
                GetBusinessCriteriaGradesVartiation.Transferability = (0 == right.Transferability ? 0 : left.Transferability / right.Transferability);
                GetBusinessCriteriaGradesVartiation.Changeability = (0 == right.Changeability ? 0 : left.Changeability / right.Changeability);
            }
            return GetBusinessCriteriaGradesVartiation;
        }
    }
}
