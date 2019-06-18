/*
 *   Copyright (c) 2019 CAST
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
        public double? TQI { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? Robustness { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? Performance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? Security { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? Changeability { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public double? Transferability { get; set; }


        /// <summary>
        /// /
        /// </summary>
        public double? ProgrammingPractices { get; set; }

        
        /// <summary>
        /// /
        /// </summary>
        public double? ArchitecturalDesign { get; set; }

        
        /// <summary>
        /// /
        /// </summary>
        public double? Documentation { get; set; }

        
        /// <summary>
        /// /
        /// </summary>
        public double? SEIMaintainability { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BusinessCriteriaDTO operator -(BusinessCriteriaDTO left, BusinessCriteriaDTO right)
        {
            BusinessCriteriaDTO _getBusinessCriteriaGradesVartiation = new BusinessCriteriaDTO();
            if (left == null || right == null) return _getBusinessCriteriaGradesVartiation;

            _getBusinessCriteriaGradesVartiation.TQI = left.TQI.HasValue && right.TQI.HasValue ? left.TQI - right.TQI : null;
            _getBusinessCriteriaGradesVartiation.Robustness = left.Robustness.HasValue && right.Robustness.HasValue ? left.Robustness - right.Robustness : null;
            _getBusinessCriteriaGradesVartiation.Performance = left.Performance.HasValue && right.Performance.HasValue ? left.Performance - right.Performance : null;
            _getBusinessCriteriaGradesVartiation.Security = left.Security.HasValue && right.Security.HasValue ? left.Security - right.Security : null;
            _getBusinessCriteriaGradesVartiation.Transferability = left.Transferability.HasValue && right.Transferability.HasValue ? left.Transferability - right.Transferability : null;
            _getBusinessCriteriaGradesVartiation.Changeability = left.Changeability.HasValue && right.Changeability.HasValue ? left.Changeability - right.Changeability : null;
            return _getBusinessCriteriaGradesVartiation;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BusinessCriteriaDTO operator /(BusinessCriteriaDTO left, BusinessCriteriaDTO right)
        {
            BusinessCriteriaDTO _getBusinessCriteriaGradesVartiation = new BusinessCriteriaDTO();
            if (left == null || right == null) return _getBusinessCriteriaGradesVartiation;

            _getBusinessCriteriaGradesVartiation.TQI = right.TQI.Equals(0.00) ? 0 : left.TQI / right.TQI;
            _getBusinessCriteriaGradesVartiation.Robustness = right.Robustness.Equals(0.00) ? 0 : left.Robustness / right.Robustness;
            _getBusinessCriteriaGradesVartiation.Performance = right.Performance.Equals(0.00) ? 0 : left.Performance / right.Performance;
            _getBusinessCriteriaGradesVartiation.Security = right.Security.Equals(0.00) ? 0 : left.Security / right.Security;
            _getBusinessCriteriaGradesVartiation.Transferability = right.Transferability.Equals(0.00) ? 0 : left.Transferability / right.Transferability;
            _getBusinessCriteriaGradesVartiation.Changeability = right.Changeability.Equals(0.00) ? 0 : left.Changeability / right.Changeability;
            return _getBusinessCriteriaGradesVartiation;
        }
    }
}
