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
    /// Represents the definition of a graphic options in order to set correctly
    /// the graphic container and its content.
    /// </summary>
    public class GraphOptions
    {
        #region PROPERTIES
        
        /// <summary>
        /// Get/Set the axis configuration of the graphic.
        /// </summary>
        public AxisDefinition AxisConfiguration { get; set; }
        
        /// <summary>
        /// Get the indication of at once one definition is present in this GraphOptions.
        /// </summary>
        public bool HasConfiguration => null != AxisConfiguration && AxisConfiguration.HasDefinitions;

        #endregion PROPERTIES

    }
}
