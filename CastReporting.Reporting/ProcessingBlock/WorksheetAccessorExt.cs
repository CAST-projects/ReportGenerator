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
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;

namespace CastReporting.Reporting.Builder.BlockProcessing
{
    public class WorksheetAccessorExt
    {
        private WorksheetAccessorExt()
        {
            // Avoid instanciation of the class
        }
        public static string GetFormulaCoord(string formula, out int startRow, out int startColumn, out int endRow, out int endColumn)
        {
            string sheetName = null;
            int nextIndex = formula.IndexOf('!');
            if (nextIndex != -1)
                sheetName = formula.Substring(0, nextIndex++);  // ++ is for passing the !
            else
                nextIndex = 0;
            string range = formula.Substring(nextIndex).Replace("$", "");
            int colonIndex = range.IndexOf(':');
            string startCell;
            string endCell;
            if (colonIndex != -1)
            {
                startCell = range.Substring(0, colonIndex);
                endCell = range.Substring(colonIndex + 1);
            }
            else
            {
                startCell = range;
                endCell = range;
            }
            GetRowColumn(startCell, out startRow, out startColumn);
            GetRowColumn(endCell, out endRow, out endColumn);
            return sheetName;
        }

        private static void GetRowColumn(string cellReference, out int row, out int column)
        {
            row = 0;
            column = 0;
            foreach (char c in cellReference)
            {
                if (char.IsLetter(c))
                    column = column * 26 + Convert.ToInt32(c) - Convert.ToInt32('A') + 1;
                else
                    row = row * 10 + Convert.ToInt32(c) - Convert.ToInt32('0');
            }
        }

        // Returns the row and column numbers and worksheet part for the named range
        public static WorksheetPart GetFormula(SpreadsheetDocument doc, string formula, out string sheetName, out int startRow, out int startColumn, out int endRow, out int endColumn)
        {
            sheetName = GetFormulaCoord(formula, out startRow, out startColumn, out endRow, out endColumn);
            return WorksheetAccessor.GetWorksheet(doc, sheetName);
        }

        public static string SetFormula(string sheetName, int startRow, int startColumn, int endRow, int endColumn, bool fixedFormula = true)
        {
            var formula = new StringBuilder();
            if (string.IsNullOrEmpty(sheetName) == false)
            {
                formula.Append(sheetName);
                formula.Append('!');
            }
            if (fixedFormula) formula.Append('$');
            formula.Append(WorksheetAccessor.GetColumnId(startColumn));
            if (fixedFormula) formula.Append('$');
            formula.Append(startRow);
            formula.Append(':');
            if (fixedFormula) formula.Append('$');
            formula.Append(WorksheetAccessor.GetColumnId(endColumn));
            if (fixedFormula) formula.Append('$');
            formula.Append(endRow);
            return formula.ToString();
        }

        public static void GetRowColumnValue(string cellReference, out int row, out int column)
        {
            GetRowColumn(cellReference, out row, out column);
        }

        public static int? AddSharedStringValue(SpreadsheetDocument document, string value)
        {
            XDocument sharedStringsXDocument = document.WorkbookPart.SharedStringTablePart.GetXDocument();
            if (sharedStringsXDocument.Root == null) return null;
            var newIndex = sharedStringsXDocument.Root.Elements().Count();
            var siElement = new XElement(S.si);
            var tElement = new XElement(S.t) {Value = value};

            siElement.Add(tElement);
            sharedStringsXDocument.Root.Add(siElement);

            document.WorkbookPart.SharedStringTablePart.PutXDocument(sharedStringsXDocument);
            return newIndex;
        }

        public static bool SetSharedStringValue(SpreadsheetDocument document, int index, string value)
        {
            XDocument sharedStringsXDocument = document.WorkbookPart.SharedStringTablePart.GetXDocument();
            if (sharedStringsXDocument.Root != null)
            {
                var siElement = sharedStringsXDocument.Root.Elements().ElementAt(index);
                if (siElement == null) return false;

                var hasTextElement = siElement.Descendants(S.t).FirstOrDefault();
                if (hasTextElement == null)
                {
                    hasTextElement = new XElement(S.t);
                    siElement.Add(hasTextElement);
                }
                hasTextElement.Value = value;
            }
            document.WorkbookPart.SharedStringTablePart.PutXDocument(sharedStringsXDocument);
            return true;
        }
    }
}
