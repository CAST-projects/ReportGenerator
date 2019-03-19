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

using System.Drawing;

namespace CastReporting.Reporting.ReportingModel
{
    /// <summary>
    /// Represents the definition of a graphic options in order to set correctly
    /// the graphic container and its content.
    /// </summary>
    public class CellAttributes
    {
        #region PROPERTIES
        /// <summary>
        /// Give the index of the data in the content which need a special attribute in its cell
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Get/Set the background color of the cell
        /// </summary>
        public string BackgroundColor { get; set; }

        public Color FontColor { get; set; }

        public string Effect { get; set; }


        public CellAttributes(int idx, string back, Color font, string effect)
        {
            Index = idx;
            BackgroundColor = back;
            FontColor = font;
            Effect = effect;
        }
        public CellAttributes(int idx, string back, Color font)
        {
            Index = idx;
            BackgroundColor = back;
            FontColor = font;
            Effect = string.Empty;
        }

        public CellAttributes(int idx, string back, string effect)
        {
            Index = idx;
            BackgroundColor = back;
            FontColor = Color.Black;
            Effect = effect;
        }

        public CellAttributes(int idx, string back)
        {
            Index = idx;
            BackgroundColor = back;
            FontColor = Color.Black;
            Effect = string.Empty;
        }

    }

    #endregion PROPERTIES
}
