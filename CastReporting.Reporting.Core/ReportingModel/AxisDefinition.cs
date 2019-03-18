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
namespace CastReporting.Reporting.ReportingModel
{
    /// <summary>
    /// Represents the definition of a graph axis properties in order to set
    /// correctly the content table.
    /// </summary>
    public class AxisDefinition
    {
        #region PROPERTIES
        /// <summary>
        /// Get/Set the minimal vertical axis value.
        /// </summary>
        public double? VerticalAxisMinimal { get; set; }
        /// <summary>
        /// Get/Set the maximal vertical axis value.
        /// </summary>
        public double? VerticalAxisMaximal { get; set; }
        /// <summary>
        /// Get/Set the minimal horizontal axis value.
        /// </summary>
        public double? HorizontalAxisMinimal { get; set; }
        /// <summary>
        /// Get/Set the maximal horizontal axis value.
        /// </summary>
        public double? HorizontalAxisMaximal { get; set; }
        /// <summary>
        /// Get the indication that at once one definition is present in this AxisDefinition.
        /// </summary>
        public bool HasDefinitions => VerticalAxisMinimal.HasValue
                                      || VerticalAxisMaximal.HasValue
                                      || HorizontalAxisMinimal.HasValue
                                      || HorizontalAxisMaximal.HasValue;

        #endregion PROPERTIES

    }
}
