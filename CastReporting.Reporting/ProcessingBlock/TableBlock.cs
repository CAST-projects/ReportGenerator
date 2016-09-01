/*
 *   Copyright (c) 2016 CAST
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Cast.Util.Log;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using OXD = DocumentFormat.OpenXml.Drawing;
using OXP = DocumentFormat.OpenXml.Presentation;
using OXW = DocumentFormat.OpenXml.Wordprocessing;

namespace CastReporting.Reporting.Builder.BlockProcessing
{
    [BlockType("TABLE")]
    public abstract class TableBlock
    {
        #region ABSTRACTS - To be implemented by Inherited children
        protected abstract TableDefinition Content(ReportData client, Dictionary<string, string> options);
        #endregion ABSTRACTS - To be implemented by Inherited children

        #region PROPERTIES
        /// <summary>
        /// Block Type Name
        /// </summary>
        public static string BlockTypeName { get { return "TABLE"; } }

      

        #endregion PROPERTIES

        #region METHODS

        public static bool IsMatching(string blockType)
        {
            return (BlockTypeName.Equals(blockType));
        }

        public TableDefinition GetContent(ReportData client, Dictionary<string, string> options)
        {
            return Content(client, options);
        }
        public static TableDefinition GetContent(string blockName, ReportData client, Dictionary<string, string> options)
        {
            TableBlock instance = BlockHelper.GetAssociatedBlockInstance<TableBlock>(blockName);
            return (null == instance ? (TableDefinition)null : instance.Content(client, options));
        }
        public static void BuildContent(ReportData client, OpenXmlPartContainer container, BlockItem block, string blockName, Dictionary<string, string> options)
        {
            TableBlock instance = BlockHelper.GetAssociatedBlockInstance<TableBlock>(blockName);
            if (null != instance)
            {
                LogHelper.Instance.LogDebugFormat("Start TableBlock generation : Type {0}", blockName);
                Stopwatch treatmentWatch = Stopwatch.StartNew();
                TableDefinition content = instance.Content(client, options);
                if (null != content)
                {
                    ApplyContent(client, container, block, content, options);
                }
                treatmentWatch.Stop();
                LogHelper.Instance.LogDebugFormat
                    ("End TableBlock generation ({0}) in {1} millisecond{2}"
                    , blockName
                    , treatmentWatch.ElapsedMilliseconds.ToString()
                    , treatmentWatch.ElapsedMilliseconds > 1 ? "s" : string.Empty
                    );
            }
        }
        public static void ApplyContent(ReportData client, OpenXmlPartContainer container, BlockItem block, TableDefinition content, Dictionary<string, string> options)
        {
            var contentblock = GetTableContentBlock(client, block);
            if (null != contentblock)
            {
                UpdateBlock(client, container, contentblock, content, options);
            }
        }

        /// <summary>
        /// Format the display of percent value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        protected static string FormatPercent(Double? pValue, bool pWidthPostiveSign)
        {           
            if (!pValue.HasValue)
                return String.Empty;

            var roundedValue = Math.Round(pValue.Value, 4);
            string sign = (roundedValue > 0 && pWidthPostiveSign) ? "+" : "";

            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            var tmp = Math.Abs(roundedValue * 100);
            nfi.PercentDecimalDigits = (tmp % 1 == 0 || tmp >= 100) ? 0 : (tmp < 10) ? 2 : 1;



            return sign + roundedValue.ToString("P", nfi);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        protected static string FormatPercent(Double? pValue)
        {
            return FormatPercent(pValue, true);
        }

        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        public static string FormatEvolution(long pValue)
        {
            string sign = (pValue > 0)?"+":"";

            return sign + pValue.ToString("N0");
        }

        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65 %" or "10.4 %" or "243 %" or "10 052 %"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        protected static string FormatEvolution(decimal pValue)
        {
            return FormatEvolution((double)pValue);
        }
        /// <summary>
        /// Format the display of evolution value into 3 digits if we can
        /// <para>Example : "3.65" or "10.4" or "243" or "10 052"</para>
        /// </summary>
        /// <param name="pValue">Numeric value to display</param>
        /// <returns>Displayed text</returns>
        protected static string FormatEvolution(double? pValue)
        {
            if(!pValue.HasValue)
				return CastReporting.Domain.Constants.No_Data;

            string ret;
            if (pValue.Value > 99)
                ret = string.Format( "{0}{1:F0}", (((int)pValue.Value) > 0) ? "+" : string.Empty, pValue);
            else if (pValue.Value > 9)
                ret = string.Format( "{0}{1:F1}", (((int)((double)pValue.Value * 10)) > 0) ? "+" : string.Empty, pValue);
            else
                ret = string.Format( "{0}{1:F2}", (((int)((double)pValue.Value * 100)) > 0) ? "+" : string.Empty, pValue);
            return ret;
        }

        private static void UpdateBlock(ReportData client, OpenXmlPartContainer container, OpenXmlElement block, TableDefinition content, Dictionary<string, string> options)
        {
            switch (client.ReportType)
            {
                case FormatType.Word: { UpdateWordBlock(client, container, block, content, options); } break;
                case FormatType.PowerPoint: { UpdatePowerPointBlock(client, container, block, content, options); } break;
                case FormatType.Excel: { UpdateExcelBlock(client, container, block, content, options); } break;
                default: break;
            }
        }
        private static OpenXmlElement GetTableContentBlock(ReportData client, BlockItem block)
        {
            switch (client.ReportType)
            {
                case FormatType.Word: return block.OxpBlock.Descendants<OXW.Table>().FirstOrDefault();
                case FormatType.PowerPoint: return block.OxpBlock;
                case FormatType.Excel: // TODO : Finalize Excel alimentation
                default: break;
            }
            throw new NotImplementedException();
        }

        #region Word methods
        private static void UpdateWordBlock(ReportData client, OpenXmlPartContainer container, OpenXmlElement block, TableDefinition content, Dictionary<string, string> options)
        {
            if (null != content && block is OXW.Table)
            {
                OXW.Table table = ((OXW.Table)block).CloneNode(true) as OXW.Table;

                OXW.TableRow headerRowTemplate = table.Descendants<OXW.TableRow>().First().CloneNode(true) as OXW.TableRow;
                OXW.TableRow contentRowTemplate = table.Descendants<OXW.TableRow>().Skip(1).First().CloneNode(true) as OXW.TableRow;

                #region Column number management
                OXW.TableGrid tablegrid = table.Descendants<OXW.TableGrid>().FirstOrDefault();
                if (null != tablegrid)
                {
                    List<OXW.GridColumn> columns = tablegrid.Descendants<OXW.GridColumn>().ToList();
                    if (null != columns && content.NbColumns != columns.Count)
                    {
                        if (content.NbColumns < columns.Count)
                        {
                            for (int i = columns.Count - 1, lim = content.NbColumns - 1; i > lim; i--)
                            {
                                tablegrid.RemoveChild<OXW.GridColumn>(columns[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0, lim = content.NbColumns - columns.Count; i < lim; i++)
                            {
                                tablegrid.AppendChild<OXW.GridColumn>(new OXW.GridColumn() { Width = "1000" });
                            }
                        }
                    }
                }
                #endregion Column number management

                ModifyWordRowTextContent(headerRowTemplate, string.Empty);
                ModifyWordRowTextContent(contentRowTemplate, string.Empty);

                int idx = 0;
                int nbrow = 0;
                List<OXW.TableCell> headerCells = headerRowTemplate.Descendants<OXW.TableCell>().Select(_ => _.CloneNode(true) as OXW.TableCell).ToList();
                List<OXW.TableCell> contentCells = contentRowTemplate.Descendants<OXW.TableCell>().Select(_ => _.CloneNode(true) as OXW.TableCell).ToList();
                headerRowTemplate.RemoveAllChildren<OXW.TableCell>();
                OXW.TableRow row = headerRowTemplate;
                int headerCellsCount = headerCells.Count;
                int contentCellsCount = headerCells.Count;

                table.RemoveAllChildren<OXW.TableRow>();
                foreach (var item in content.Data)
                {
                    if (null != item)
                    {
                        OXW.TableCell cell = null;
                        if (content.HasColumnHeaders && 0 == nbrow)
                        {
                            cell = headerCells[idx % headerCellsCount].CloneNode(true) as OXW.TableCell;
                        }
                        else
                        {
                            cell = contentCells[idx % contentCellsCount].CloneNode(true) as OXW.TableCell;
                        }
                        ModifyWordCellTextContent(cell, item);
                        row.Append(cell);
                    }

                    idx = ++idx % content.NbColumns;
                    if (0 == idx)
                    {
                        if (null != row)
                        {
                            table.Append(row);
                            nbrow++;
                        }
                        row = contentRowTemplate.CloneNode(true) as OXW.TableRow;
                        row.RemoveAllChildren<OXW.TableCell>();
                    }
                }
                var blockSdt = block.Ancestors<OXW.SdtBlock>().First();
                blockSdt.Parent.ReplaceChild(table, blockSdt);
            }
            else
            {
                LogHelper.Instance.LogErrorFormat("Impossible to load data in Table block with a block source of type \"{0}\"", null != block ? block.GetType().ToString() : "null");
            }
        }
        private static void ModifyWordRowTextContent(OXW.TableRow headerRowTemplate, string txt)
        {
            if (null != headerRowTemplate)
            {
                var cells = headerRowTemplate.Descendants<OXW.TableCell>();
                if (null != cells)
                {
                    foreach (var cell in cells)
                    {
                        ModifyWordCellTextContent(cell, txt);
                    }
                }
            }
        }
        private static void ModifyWordCellTextContent(OXW.TableCell cell, string txt)
        {
            if (null != cell)
            {
                OXW.Paragraph paragraph = cell.Descendants<OXW.Paragraph>().FirstOrDefault();
                if (null != paragraph)
                {
                    paragraph = paragraph.CloneNode(true) as OXW.Paragraph;
                    ModifyWordParagraphTextContent(paragraph, txt);
                    cell.RemoveAllChildren<OXW.Paragraph>();
                    cell.Append(paragraph);
                }
            }
        }
        private static void ModifyWordParagraphTextContent(OXW.Paragraph paragraph, string txt)
        {
            if (null != paragraph)
            {
                OXW.Run run = paragraph.Descendants<OXW.Run>().FirstOrDefault();
                if (null != run)
                {
                    run = run.CloneNode(true) as OXW.Run;
                    paragraph.RemoveAllChildren<OXW.Run>();
                }
                else
                {
					run = new OXW.Run();
				}
                    OXW.Text text = run.Descendants<OXW.Text>().FirstOrDefault();
                    text = (null == text ? new OXW.Text() : text.CloneNode(true) as OXW.Text);
                    run.RemoveAllChildren<OXW.Text>();
                    text.Text = txt;
				if (!string.IsNullOrEmpty(txt) && (char.IsWhiteSpace(txt[0]) || char.IsWhiteSpace(txt[txt.Length-1]))) {
					text.Space = SpaceProcessingModeValues.Preserve;
				}
                    run.Append(text);
                    paragraph.Append(run);
                }
            }

        private static void ReplaceWordRun(OXD.Paragraph paragraph, OXD.Run initRun, OXD.Run finalRun)
        {
            if (null != paragraph.Descendants<OXD.Run>())
            {
                List<OXD.Run> runs = paragraph.Descendants<OXD.Run>().ToList();
                foreach (var run in runs)
                {
                    if (initRun != run)
                    {
                        paragraph.RemoveChild<OXD.Run>(run);
                    }
                }
                paragraph.ReplaceChild<OXD.Run>(finalRun, initRun);
            }
        }
        #endregion Word methods

        #region Powerpoint methods
        private static void UpdatePowerPointBlock(ReportData client, OpenXmlPartContainer container, OpenXmlElement block, TableDefinition content, Dictionary<string, string> options)
        {
            if (null != content && block is OXP.GraphicFrame)
            {
                OXD.Table initTable = (OXD.Table)block.Descendants<OXD.Table>().FirstOrDefault();
                if (null != initTable)
                {
                    try
                    {
                        OXD.Table table = initTable.CloneNode(true) as OXD.Table;
                        OXD.TableRow headerRowTemplate = table.Descendants<OXD.TableRow>().First().CloneNode(true) as OXD.TableRow;
                        OXD.TableRow contentRowTemplate = table.Descendants<OXD.TableRow>().Skip(1).First().CloneNode(true) as OXD.TableRow;

                        ModifyPowerPointRowTextContent(headerRowTemplate, string.Empty);
                        ModifyPowerPointRowTextContent(contentRowTemplate, string.Empty);

                        #region Column Number Management
                        List<OXD.GridColumn> columns = table.TableGrid.Descendants<OXD.GridColumn>().ToList();
                        if (columns.Count < content.NbColumns)
                        {
                            int nbNewColumn = content.NbColumns - columns.Count;
                            for (int i = 0, lim = nbNewColumn; i < lim; i++)
                            {
                                AddNewGridColumn(table.TableGrid, headerRowTemplate, contentRowTemplate);
                            }
                        } else if (columns.Count > content.NbColumns) {
                        	for (int i = content.NbColumns, lim = columns.Count; i < lim; i++) {
                        		RemoveLastGridColumn(table.TableGrid);
                        	}
                        }
                        #endregion Column Number Management

                        int idx = 0;
                        int nbrow = 0;
                        List<OXD.TableCell> headerCells = headerRowTemplate.Descendants<OXD.TableCell>().Select(_ => _.CloneNode(true) as OXD.TableCell).ToList();
                        List<OXD.TableCell> contentCells = contentRowTemplate.Descendants<OXD.TableCell>().Select(_ => _.CloneNode(true) as OXD.TableCell).ToList();
                        headerRowTemplate.RemoveAllChildren<OXD.TableCell>();
                        OXD.TableRow row = headerRowTemplate;

                        table.RemoveAllChildren<OXD.TableRow>();
                        foreach (var item in content.Data)
                        {
                            OXD.TableCell cell = null;
                            if (content.HasColumnHeaders && 0 == nbrow)
                            {
                                cell = headerCells[idx].CloneNode(true) as OXD.TableCell;
                            }
                            else
                            {
                                cell = contentCells[idx].CloneNode(true) as OXD.TableCell;
                            }
                            ModifyPowerPointCellTextContent(cell, item);
                            row.Append(cell);
                            idx = ++idx % content.NbColumns;
                            if (0 == idx)
                            {
                                if (null != row)
                                {
                                    table.Append(row);
                                    nbrow++;
                                }
                                row = contentRowTemplate.CloneNode(true) as OXD.TableRow;
                                row.RemoveAllChildren<OXD.TableCell>();
                            }
                        }
                        initTable.Parent.ReplaceChild(table, initTable);
                    }
                    catch (Exception exception)
                    {
                        LogHelper.Instance.LogErrorFormat("An unhandled exception was thrown during table block content generation : '{0}'", exception.ToString());
                        if (null != initTable)
                        {
                            if (null != initTable.Descendants<OXD.TableRow>() && 1 > initTable.Descendants<OXD.TableRow>().Count())
                            {
                                foreach (var row in initTable.Descendants<OXD.TableRow>().Skip(1))
                                {
                                    ModifyPowerPointRowTextContent(row, string.Empty);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                LogHelper.Instance.LogErrorFormat("Impossible to load data in table block with a block source of type \"{0}\"", null != block ? block.GetType().ToString() : "null");
            }
        }

        private static void AddNewGridColumn(OXD.TableGrid tableGrid, OXD.TableRow headerRow, OXD.TableRow contentRow)
        {
            var columns = tableGrid.Descendants<OXD.GridColumn>();
            if (null != columns && columns.Any())
            {
                var headerLastCell = headerRow.Descendants<OXD.TableCell>().Last();
                var contentLastCell = contentRow.Descendants<OXD.TableCell>().Last();
                double tableWidth = columns.Sum(_ => Convert.ToInt32(_.Width.Value));
                var newColWidth = Math.Floor(tableWidth / columns.Count());
                foreach (var col in columns)
                {
                    col.Width = col.Width > 0 ? Convert.ToInt64(Math.Floor((tableWidth - newColWidth) / (tableWidth / col.Width))) : 0;
                }
                tableGrid.InsertAfter<OXD.GridColumn>(new OXD.GridColumn() { Width = Convert.ToInt64(newColWidth) }, columns.Last());
                headerRow.InsertAfter<OXD.TableCell>((OXD.TableCell)headerLastCell.CloneNode(true), headerLastCell);
                contentRow.InsertAfter<OXD.TableCell>((OXD.TableCell)contentLastCell.CloneNode(true), contentLastCell);
            }
        }

		private static void RemoveLastGridColumn(OXD.TableGrid tableGrid)
		{
			var lastColumn = tableGrid.Descendants<OXD.GridColumn>().Last();
			tableGrid.RemoveChild<OXD.GridColumn>(lastColumn);
		}
		
        private static int GetWidth(OpenXmlAttribute openXmlAttribute)
        {
            int back = 0;
            if (null != openXmlAttribute && !int.TryParse(openXmlAttribute.Value, out back))
            {
                back = 0;
            }
            return back;
        }
        private static void ModifyPowerPointRowTextContent(OXD.TableRow headerRowTemplate, string txt)
        {
            if (null != headerRowTemplate)
            {
                var cells = headerRowTemplate.Descendants<OXD.TableCell>();
                if (null != cells)
                {
                    foreach (var cell in cells)
                    {
                        ModifyPowerPointCellTextContent(cell, txt);
                    }
                }
            }
        }
        private static void ModifyPowerPointCellTextContent(OXD.TableCell cell, string txt)
        {
            if (null != cell)
            {
                OXD.TextBody textbody = cell.Descendants<OXD.TextBody>().FirstOrDefault();
                if (null != textbody)
                {
                    OXD.TextBody final_textbody = textbody.CloneNode(true) as OXD.TextBody;
                    OXD.Paragraph paragraph = final_textbody.Descendants<OXD.Paragraph>().FirstOrDefault();
                    if (null != paragraph)
                    {
                        OXD.Paragraph final_paragraph = paragraph.CloneNode(true) as OXD.Paragraph;
                        ModifyPowerPointParagraphTextContent(final_paragraph, txt);
                        final_textbody.ReplaceChild<OXD.Paragraph>(final_paragraph, paragraph);
                    }
                    cell.ReplaceChild<OXD.TextBody>(final_textbody, textbody);
                }
            }
        }
        private static void ModifyPowerPointParagraphTextContent(OXD.Paragraph paragraph, string txt)
        {
            if (null != paragraph)
            {
                OXD.Run run = paragraph.Descendants<OXD.Run>().FirstOrDefault();
                if (null != run)
                {
                    OXD.Run final_run = run.CloneNode(true) as OXD.Run;
                    OXD.Text text = final_run.Descendants<OXD.Text>().FirstOrDefault();
                    OXD.Text final_text = (null == text ? new OXD.Text() : text.CloneNode(true) as OXD.Text);
                    final_text.Text = txt;
                    try
                    { final_run.ReplaceChild<OXD.Text>(final_text, text); }
                    catch
                    { throw; }
                    ReplaceWordRun(paragraph, run, final_run);
                }
                else
                {
                    run = new OXD.Run();
                }
            }
        }
        #endregion Powerpoint methods

        #region Excel methods
        private static void UpdateExcelBlock(ReportData client, OpenXmlPartContainer container, OpenXmlElement block, TableDefinition content, Dictionary<string, string> options)
        {
            // TODO : Finalize Excel alimentation
            throw new NotImplementedException();
        }
        #endregion Excel methods

        #endregion METHODS
    }
}
