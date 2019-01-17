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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using DocumentFormat.OpenXml;
using System.Globalization;
using System.Text.RegularExpressions;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Builder
{
    internal class ExcelDocumentBuilder : DocumentBuilderBase
    {
        public string StrFinalTempFile { get; set; }
        // ReSharper disable once InconsistentNaming
        public ReportData reportData { get; set; }

        #region CONSTRUCTORS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="tmpRepFlexi"></param>
        public ExcelDocumentBuilder(ReportData client, string tmpRepFlexi)
            : base(client)
        { 
            StrFinalTempFile = tmpRepFlexi;
            reportData = client;
        }
        #endregion CONSTRUCTORS

        #region METHODS


        ///// <summary>
        ///// Returns the block configuration of the block item given in parameter.
        ///// </summary>
        ///// <param name="block">Block where the block configuration parameters will be found.</param>
        ///// <returns>The block configuration of the block item given in parameter.</returns>
        protected override BlockConfiguration GetBlockConfiguration(BlockItem block)
        {
            // TODO : Finalize Excel alimentation
            return null;
        }

        protected new virtual BlockConfiguration GetBlockConfiguration(string description)
        {
            return GetBlockConfiguration(description, null);
        }


        protected new BlockConfiguration GetBlockConfiguration(string alias, string tag)
        {
            BlockConfiguration back = new BlockConfiguration();

            string[] optionList = null;
            string blockOptionStr = "";
            if (!string.IsNullOrWhiteSpace(alias))
            {
                optionList = alias.Replace(@"\r\n", string.Empty).Split(';');
                blockOptionStr = !string.IsNullOrWhiteSpace(tag) ? tag.Replace(@"\r\n", string.Empty) : string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(tag))
        {
                optionList = tag.Replace(@"\r\n", string.Empty).Split(';');
                if (optionList.Length >= 3)
            {
                    blockOptionStr = optionList[2];
            }
        }
            if (null == optionList || optionList.Length < 2) return back;
            back.Type = optionList[0];
            back.Name = optionList[1];
            if (optionList.Length > 2 && string.IsNullOrWhiteSpace(blockOptionStr))
            {
                blockOptionStr += $",{optionList.Skip(2).Aggregate((current, next) => $"{current},{next}")}";
            }
            back.Options = string.IsNullOrWhiteSpace(blockOptionStr) ? new Dictionary<string, string>() : ParseOptions(blockOptionStr);
            return back;
        }

        /// <summary>
        /// Returns all block contained into the container given in argument.
        /// </summary>
        /// <param name="container">Container where the block items will be found.</param>
        /// <returns>All block contained into the container given in argument.</returns>
        protected override List<BlockItem> GetBlocks(OpenXmlPartContainer container)
        {
            // TODO : Finalize Excel alimentation


            throw new NotImplementedException();

        }


        public override void BuildDocument()
        {
            string strTargetFile = ReportData.FileName;
            string fileName = StrFinalTempFile;

            if (strTargetFile != "")
            {
                BuildReportTemplateEFPFlexi(fileName);
            }
            else
            {
                throw new InvalidOperationException("Unable to file the Workbook");
            }
        }


        private static void SetCellValue(CellType cell, string value)
        {
            decimal dx;
            if (decimal.TryParse(value, out dx))
            {
                cell.CellValue = new CellValue(dx.ToString("G", NumberFormatInfo.InvariantInfo));
                cell.DataType = CellValues.Number;
            }
            else
            {
                cell.CellValue = new CellValue(value);
                cell.DataType = CellValues.String;
            }
        }

        // ReSharper disable once InconsistentNaming
        private const string FLEXI_PREFIX = "RepGen:";

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class TableInfo
        {
            public Cell cell { get; set; }
            public TableDefinition table { get; set; }
        }

        private void BuildReportTemplateEFPFlexi(string strTargetFile)
        {
            string fileName = strTargetFile;

            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string[] alph = new string[676]; // 676 = 26*26
            for (int i = 0; i < 26; i++)
            {
                alph[i] = alphabet.Substring(i, 1);
            }
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    alph[26 + i + j] = alphabet.Substring(i,1) + alphabet.Substring(j, 1);

                }
            }

            var tableTargets = new List<TableInfo>();

            using (SpreadsheetDocument workbook = SpreadsheetDocument.Open(fileName, true))
            {
                var workbookPart = workbook.WorkbookPart;
                var sharedStringPart = workbookPart.SharedStringTablePart;
                var values = sharedStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();

                foreach (WorksheetPart worksheetpart in workbookPart.WorksheetParts)
                {
                    foreach (var sheetData in worksheetpart.Worksheet.Elements<SheetData>())
                    {
                        // reset accross sheets
                        tableTargets.Clear();

                        #region TextPopulate
                        foreach (var cell in sheetData.Descendants<Cell>())
                        {
                            if (cell.CellValue != null)
                            {
                                if (cell.CellFormula != null)
                                {
                                    // force recompute
                                    cell.CellValue.Remove();
                                }
                                else if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                                {
                                    var index = int.Parse(cell.CellValue.Text);
                                    if (values[index].InnerText.StartsWith(FLEXI_PREFIX))
                                    {
                                        string strBlockTypeAndName = values[index].InnerText.Substring(FLEXI_PREFIX.Length);

                                        BlockConfiguration config = GetBlockConfiguration(strBlockTypeAndName);

                                        if (TextBlock.IsMatching(config.Type))
                                        {
                                            TextBlock instance = BlockHelper.GetAssociatedBlockInstance<TextBlock>(config.Name);
                                            if (instance != null)
                                            {
                                                SetCellValue(cell, instance.GetContent(reportData, config.Options));
                                            }
                                        }
                                        else if (TableBlock.IsMatching(config.Type))
                                        {
                                            TableBlock instance = BlockHelper.GetAssociatedBlockInstance<TableBlock>(config.Name);
                                            if (instance != null)
                                            {
                                                tableTargets.Add(new TableInfo
                                                {
                                                    cell = cell,
                                                    table = instance.GetContent(reportData, config.Options)
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion TextPopulate


                        #region TablePopulate
                        foreach (var tableInfo in tableTargets)
                        {
                            var _finaleCell = tableInfo.cell;
                            var _finaleTable = tableInfo.table;

                            int intColumns = _finaleTable.NbColumns;

                            // TODO: handle cell references after 'ZZ' (AA1, AB1...)
                            // TODO: current limitation: the generated cells must be in the range "A-ZZ"

                            string firstLetter = _finaleCell.CellReference.InnerText[0].ToString();
                            int firstColIdx = alph.ToList().IndexOf(firstLetter) + 1;
                            int lastColIdx = firstColIdx + intColumns - 1;
                            int curColIdx = firstColIdx;

                            uint firstRowIdx = uint.Parse(_finaleCell.CellReference.InnerText.Substring(1));
                            uint curRowIdx = firstRowIdx;

                            // create first row
                            Row curRow = new Row();

                            foreach (var result in _finaleTable.Data)
                            {
                                // append cell to current row
                                Cell c = new Cell();
                                SetCellValue(c, result);
                                // to avoid crash when too many columns generated
                                if (curColIdx > 676) continue;
                                c.CellReference = alph[curColIdx - 1] + curRowIdx.ToString();
                                c.StyleIndex = 0;
                                // ReSharper disable once PossiblyMistakenUseOfParamsMethod
                                curRow.Append(c);

                                if (curColIdx == lastColIdx)
                                {
                                    // add row to current worksheet
                                    InsertRow(curRowIdx, worksheetpart, curRow);
                                    // create new row for next data
                                    curRow = new Row();

                                    // first cell on next row
                                    curRowIdx++;
                                    curColIdx = firstColIdx;
                                }
                                else
                                {
                                    // next cell
                                    curColIdx++;
                                }
                            }
                            _finaleCell.Parent.RemoveChild(_finaleCell);
                        }

                        workbookPart.Workbook.Save();

                        #endregion TablePopulate
                    }
                }

            }
        }


        private static void UpdateRowIndexes(WorksheetPart worksheetPart, uint rowIndex, bool isDeletedRow)
        {
            // Get all the rows in the worksheet with equal or higher row index values than the one being inserted/deleted for reindexing.
            IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex.Value >= rowIndex);

            foreach (Row row in rows)
            {
                uint newIndex = (isDeletedRow ? row.RowIndex - 1 : row.RowIndex + 1);
                string curRowIndex = row.RowIndex.ToString();
                string newRowIndex = newIndex.ToString();

                foreach (Cell cell in row.Elements<Cell>())
                {
                    // Update the references for the rows cells.
                    cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curRowIndex, newRowIndex));
                }

                // Update the row index.
                row.RowIndex = newIndex;
            }
        }

        private static void UpdateMergedCellReferences(WorksheetPart worksheetPart, uint rowIndex, bool isDeletedRow)
        {
            if (!worksheetPart.Worksheet.Elements<MergeCells>().Any()) return;
            MergeCells mergeCells = worksheetPart.Worksheet.Elements<MergeCells>().FirstOrDefault();

            if (mergeCells == null) return;
            // Grab all the merged cells that have a merge cell row index reference equal to or greater than the row index passed in
            List<MergeCell> mergeCellsList = mergeCells.Elements<MergeCell>()
                .Where(r => r.Reference.HasValue &&
                            (GetRowIndex(r.Reference.Value.Split(':').ElementAt(0)) >= rowIndex ||
                             GetRowIndex(r.Reference.Value.Split(':').ElementAt(1)) >= rowIndex)).ToList();

            // Need to remove all merged cells that have a matching rowIndex when the row is deleted
            if (isDeletedRow)
            {
                List<MergeCell> mergeCellsToDelete = mergeCellsList.Where(r => GetRowIndex(r.Reference.Value.Split(':').ElementAt(0)) == rowIndex ||
                                                                               GetRowIndex(r.Reference.Value.Split(':').ElementAt(1)) == rowIndex).ToList();

                // Delete all the matching merged cells
                foreach (MergeCell cellToDelete in mergeCellsToDelete)
                {
                    cellToDelete.Remove();
                }

                // Update the list to contain all merged cells greater than the deleted row index
                mergeCellsList = mergeCells.Elements<MergeCell>()
                    .Where(r => r.Reference.HasValue &&
                                (GetRowIndex(r.Reference.Value.Split(':').ElementAt(0)) > rowIndex ||
                                 GetRowIndex(r.Reference.Value.Split(':').ElementAt(1)) > rowIndex)).ToList();
            }

            // Either increment or decrement the row index on the merged cell reference
            foreach (MergeCell mergeCell in mergeCellsList)
            {
                string[] cellReference = mergeCell.Reference.Value.Split(':');

                if (GetRowIndex(cellReference.ElementAt(0)) >= rowIndex)
                {
                    string columnName = GetColumnName(cellReference.ElementAt(0));
                    cellReference[0] = isDeletedRow ? columnName + (GetRowIndex(cellReference.ElementAt(0)) - 1).ToString() : IncrementCellReference(cellReference.ElementAt(0), CellReferencePartEnum.Row);
                }

                if (GetRowIndex(cellReference.ElementAt(1)) >= rowIndex)
                {
                    string columnName = GetColumnName(cellReference.ElementAt(1));
                    cellReference[1] = isDeletedRow ? columnName + (GetRowIndex(cellReference.ElementAt(1)) - 1).ToString() : IncrementCellReference(cellReference.ElementAt(1), CellReferencePartEnum.Row);
                }

                mergeCell.Reference = new StringValue(cellReference[0] + ":" + cellReference[1]);
            }
        }

        private static void UpdateHyperlinkReferences(WorksheetPart worksheetPart, uint rowIndex, bool isDeletedRow)
        {
            Hyperlinks hyperlinks = worksheetPart.Worksheet.Elements<Hyperlinks>().FirstOrDefault();

            if (hyperlinks == null) return;
            foreach (Hyperlink hyperlink in hyperlinks.Elements<Hyperlink>())
            {
                var hyperlinkRowIndexMatch = Regex.Match(hyperlink.Reference.Value, "[0-9]+");
                uint hyperlinkRowIndex;
                if (!hyperlinkRowIndexMatch.Success || !uint.TryParse(hyperlinkRowIndexMatch.Value, out hyperlinkRowIndex) || hyperlinkRowIndex < rowIndex) continue;
                // if being deleted, hyperlink needs to be removed or moved up
                if (isDeletedRow)
                {
                    // if hyperlink is on the row being removed, remove it
                    if (hyperlinkRowIndex == rowIndex)
                    {
                        hyperlink.Remove();
                    }
                    // else hyperlink needs to be moved up a row
                    else
                    {
                        hyperlink.Reference.Value = hyperlink.Reference.Value.Replace(hyperlinkRowIndexMatch.Value, (hyperlinkRowIndex - 1).ToString());

                    }
                }
                // else row is being inserted, move hyperlink down
                else
                {
                    hyperlink.Reference.Value = hyperlink.Reference.Value.Replace(hyperlinkRowIndexMatch.Value, (hyperlinkRowIndex + 1).ToString());
                }
            }

            // Remove the hyperlinks collection if none remain
            if (!hyperlinks.Elements<Hyperlink>().Any())
            {
                hyperlinks.Remove();
            }
        }

        public static string IncrementCellReference(string reference, CellReferencePartEnum cellRefPart)
        {
            string newReference = reference;

            if (cellRefPart != CellReferencePartEnum.None && !String.IsNullOrEmpty(reference))
            {
                string[] parts = Regex.Split(reference, "([A-Z]+)");

                if (cellRefPart == CellReferencePartEnum.Column || cellRefPart == CellReferencePartEnum.Both)
                {
                    List<char> col = parts[1].ToCharArray().ToList();
                    bool needsIncrement = true;
                    int index = col.Count - 1;

                    do
                    {
                        // increment the last letter
                        col[index] = Letters[Letters.IndexOf(col[index]) + 1];

                        // if it is the last letter, then we need to roll it over to 'A'
                        if (col[index] == Letters[Letters.Count - 1])
                        {
                            col[index] = Letters[0];
                        }
                        else
                        {
                            needsIncrement = false;
                        }

                    } while (needsIncrement && --index >= 0);

                    // If true, then we need to add another letter to the mix. Initial value was something like "ZZ"
                    if (needsIncrement)
                    {
                        col.Add(Letters[0]);
                    }

                    parts[1] = new String(col.ToArray());
                }

                if (cellRefPart == CellReferencePartEnum.Row || cellRefPart == CellReferencePartEnum.Both)
                {
                    // Increment the row number. A reference is invalid without this componenet, so we assume it will always be present.
                    parts[2] = (int.Parse(parts[2]) + 1).ToString();
                }

                newReference = parts[1] + parts[2];
            }

            return newReference;
        }



        private static string GetColumnName(string cellName)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }

        public enum CellReferencePartEnum
        {
            None,
            Column,
            Row,
            Both
        }

        protected static List<char> Letters = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

        public static uint GetRowIndex(string cellReference)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellReference);

            return uint.Parse(match.Value);
        }


        public static Row InsertRow(uint rowIndex, WorksheetPart worksheetPart, Row insertRow, bool isNewLastRow = false)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row retRow = !isNewLastRow ? sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex) : null;

            // If the worksheet does not contain a row with the specified row index, insert one.
            if (retRow != null)
            {
                // if retRow is not null and we are inserting a new row, then move all existing rows down.
                if (insertRow != null)
                {
                    UpdateRowIndexes(worksheetPart, rowIndex, false);
                    UpdateMergedCellReferences(worksheetPart, rowIndex, false);
                    UpdateHyperlinkReferences(worksheetPart, rowIndex, false);

                    // actually insert the new row into the sheet
                    retRow = sheetData.InsertBefore(insertRow, retRow);  // at this point, retRow still points to the row that had the insert rowIndex

                    string curIndex = rowIndex.ToString();
                    string newIndex = rowIndex.ToString();

                    foreach (Cell cell in retRow.Elements<Cell>())
                    {
                        // Update the references for the rows cells.
                        cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curIndex, newIndex));
                    }

                    // Update the row index.
                    retRow.RowIndex = rowIndex;
                }
            }
            else
            {
                // Row doesn't exist yet, shifting not needed.
                // Rows must be in sequential order according to RowIndex. Determine where to insert the new row.
                Row refRow = !isNewLastRow ? sheetData.Elements<Row>().FirstOrDefault(row => row.RowIndex > rowIndex) : null;

                // use the insert row if it exists
                retRow = insertRow ?? new Row() { RowIndex = rowIndex };

                IEnumerable<Cell> _cellsInRow = retRow.Elements<Cell>().ToList();
                if (_cellsInRow.Any())
                {
                    string curIndex = retRow.RowIndex.ToString();
                    string newIndex = rowIndex.ToString();

                    foreach (Cell cell in _cellsInRow)
                    {
                        // Update the references for the rows cells.
                        cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curIndex, newIndex));
                    }

                    // Update the row index.
                    retRow.RowIndex = rowIndex;
                }

                sheetData.InsertBefore(retRow, refRow);
            }

            return retRow;
        }

        #endregion METHODS
    }
}
