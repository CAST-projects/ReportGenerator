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
using System.Collections.Generic;
using System.Xml.Linq;

namespace CastReporting.Reporting.ReportingModel
{
  
    /// <summary>
    /// Represents the definition of a table options in order to set correctly
    /// the table container and its content.
    /// </summary>
    public class TableDefinition
    {
        #region ATTRIBUTES
        public delegate void FormatCellEventHandler(int row, int col, XElement cell, TableDefinition table);
        #endregion ATTRIBUTES


        #region PROPERTIES
        /// <summary>
        /// Get/Set the table content items.
        /// </summary>
        public IEnumerable<string> Data { get; set; }
        /// <summary>
        /// Get/Set the table content labels.
        /// </summary>
        public IEnumerable<string> DataLabel { get; set; }
        /// <summary>
        /// Get/Set the indication of the presence of a graph data label text.
        /// </summary>
        public bool GraphDataLabelText { get; set; }
        /// <summary>
        /// Get/Set the indication of the presence of a column header in table.
        /// </summary>
        public bool HasColumnHeaders { get; set; }
        /// <summary>
        /// Get/Set the indication of the presence of a row header in table.
        /// </summary>
        public bool HasRowHeaders { get; set; }
        /// <summary>
        /// Get/Set the column count in the target table.
        /// </summary>
        public int NbColumns { get; set; }
        /// <summary>
        /// Get/Set the row count in the target table.
        /// </summary>
        public int NbRows { get; set; }
        /// <summary>
        /// Get/Set the graph options.
        /// </summary>
        public GraphOptions GraphOptions { get; set; }

        public List<CellAttributes> CellsAttributes
        {
            get;
            set;
        }

        public bool HasCellsAttributes()
        {
            return CellsAttributes?.Count > 0;
        }

        #endregion PROPERTIES


    }

}
