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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Cast.Util.Log;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using OXS = DocumentFormat.OpenXml.Spreadsheet;
using OXW = DocumentFormat.OpenXml.Wordprocessing;
// ReSharper disable UnusedParameter.Local

namespace CastReporting.Reporting.Builder.BlockProcessing
{
    [BlockType("GRAPH")]
    public abstract class GraphBlock
    {
        #region PROPERTIES
        private static string BlockTypeName => "GRAPH";

        #endregion PROPERTIES

        #region METHODS
        public abstract TableDefinition Content(ReportData client, Dictionary<string, string> options);

        public static bool IsMatching(string blockType)
        {
            return (BlockTypeName.Equals(blockType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pPackage"></param>
        /// <param name="block"></param>
        /// <param name="blockName"></param>
        /// <param name="options"></param>
        public static void BuildContent(ReportData client, OpenXmlPackage pPackage, BlockItem block, string blockName, Dictionary<string, string> options)
        {
            var previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            GraphBlock instance = BlockHelper.GetAssociatedBlockInstance<GraphBlock>(blockName);
            if (null == instance) return;
            LogHelper.Instance.LogDebugFormat("Start GraphBlock generation : Type {0}", blockName);
            Stopwatch treatmentWatch = Stopwatch.StartNew();
            TableDefinition content = instance.Content(client, options);
            try
            {
                if (null != content)
                {                       
                    ApplyContent(client, pPackage, block, content, options);                        
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previousCulture;

                treatmentWatch.Stop();
                LogHelper.Instance.LogDebugFormat
                ("End GraphBlock generation ({0}) in {1} millisecond{2}"
                    , blockName
                    , treatmentWatch.ElapsedMilliseconds
                    , treatmentWatch.ElapsedMilliseconds > 1 ? "s" : string.Empty
                );
            }
        }
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void ApplyContent(ReportData pClient, OpenXmlPackage pPackage, BlockItem pBlock, TableDefinition pContent, Dictionary<string, string> pOptions)
        {
           
            try
            {
                string chartId = null;

                var phElem = GetElements(pBlock.OxpBlock, pClient.ReportType)?.ToList();
                if (phElem == null || phElem.Count == 0)
                {
                    LogHelper.Instance.LogError("No placeholder content found.");
                    return;
                }

                #region Get the block Id in document
                var allElementInPlaceHolder = GetElementsInPlaceHolder(pClient, phElem);
                XElement graphicElement = phElem.Descendants(A.graphic).FirstOrDefault() ?? phElem.FirstOrDefault(_ => A.graphic.Equals(_.Name));
                if (null != graphicElement)
                {
                    XElement chartElem = graphicElement.Descendants(C.chart).FirstOrDefault();
                    if (null != chartElem)
                    {
                        chartId = chartElem.Attribute(R.id)?.Value;
                    }
                    else
                    {
                        LogHelper.Instance.LogFatal("Graphic object present but not a graph.");
                    }
                }
                else
                {
                    LogHelper.Instance.LogFatal("No graphic / chart object inside the block.");
                }
                if (null == chartId) { pBlock.XBlock.ReplaceWith(allElementInPlaceHolder); return; }
                #endregion Get the block Id in document

                var chartPart = GetChartPart(pPackage, pBlock, chartId);
                if (null != chartPart)
                {
                    string spreadsheetId = GetSpreadsheetId(chartPart);
                    if (!string.IsNullOrWhiteSpace(spreadsheetId))
                    {
                        #region Associated content management
                        var embedPackage = (EmbeddedPackagePart)chartPart.GetPartById(spreadsheetId);
                        if (null != embedPackage)
                        {
                            using (var ms = new MemoryStream())
                            {
                                #region Set content in memory to work with
                                using (Stream str = embedPackage.GetStream())
                                {
                                    StreamHelper.CopyStream(str, ms);
                                    str.Close();
                                }
                                #endregion Set content in memory to work with

                                using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(ms, true))
                                {
                                    #region Associated Data File content Management
                                    var ws = (OXS.Sheet)spreadsheetDoc.WorkbookPart.Workbook.Sheets.FirstOrDefault();
                                    if (ws != null)
                                    {
                                        string sheetId = ws.Id;
                                        var wsp = (WorksheetPart)spreadsheetDoc.WorkbookPart.GetPartById(sheetId);
                                        XDocument shPart = wsp.GetXDocument();
                                        XElement shData = shPart.Descendants(S.sheetData).FirstOrDefault();

                                        #region Use of the data content (Only data, no titles)
                                        if (null != shData)
                                        {
                                            IEnumerable<XElement> allRows = shData.Descendants(S.row); // => if ToList() cause some graph to fail to generate
                                            int ctRow = 0;
                                            int nbRows = allRows.Count();

                                            // Cleaning Cells  ======================================================================
                                            // We clean row not having Cell information
                                            int idxRow = 1;
                                            var nbCorrectSeries = allRows.Where(x => x.Descendants(S.c).Any(y => y.Attribute(NoNamespace.r) != null &&
                                                                                                                 y.Attribute(NoNamespace.r)?.Value != ""))
                                                .Max(x => x.Descendants(S.c).Count());
                                            for (int ctn = 0; ctn < nbRows; ctn += 1)
                                            {
                                                var oneRow = allRows.ElementAt(ctn);//[ctn];
                                                // DCO - 9/21/2012 - I added the condition that the count of CELL must be indeed equal of numCorrectSeries OR to the number of column
                                                // It happens for graphs with 3 cells defined per row, but only two used (so no Value for third cell), when NbColumns was == 2
                                                var isRowValid = ((oneRow.Descendants(S.c).Count() == nbCorrectSeries || oneRow.Descendants(S.c).Count() >= pContent.NbColumns) &&
                                                                  (oneRow.Descendants(S.c).Descendants(S.v).Count() == oneRow.Descendants(S.c).Count() ||
                                                                   oneRow.Descendants(S.c).Descendants(S.v).Count() >= pContent.NbColumns));

                                                // We remove rows that are not defined in content
                                                if (isRowValid == false || ctRow >= pContent.NbRows)
                                                {
                                                    oneRow.Remove();
                                                    ctn -= 1;
                                                    nbRows -= 1;
                                                    continue;
                                                }
                                                var _xAttribute = oneRow.Attribute(NoNamespace.r);
                                                if (_xAttribute != null) _xAttribute.Value = idxRow.ToString();
                                                idxRow += 1;
                                            }
                                            // ====================================================================================


                                            // Copying new row  ========================================================================
                                            // We copied new row if needed and extrapolate formula
                                            // Take cell with no t SharedString
                                            nbRows = allRows.Count();
                                            if (pContent.NbRows > nbRows)
                                            {
                                                // We need to detect the best ROW to copy
                                                //    Usually the first row is the header so it contains two cells with SharedString 
                                                //    Case 1: For others rows, it will be two cells with value.
                                                //    Case 2: For other rows, it can be one cell with SharedString and one cell with value (in case of App Nane + value)
                                                var oneSerieRow =
                                                    // Case 1
                                                    allRows.FirstOrDefault(x => x.Attribute(NoNamespace.r) != null &&
                                                                                x.Attribute(NoNamespace.r)?.Value != "" && x.Descendants(S.c).Any() &&
                                                                                x.Descendants(S.c).Attributes(NoNamespace.t).Any() == false)
                                                    ?? // Case 2: One or several SharedString, but at least one Value (by using < content.NbColumns)
                                                    allRows.FirstOrDefault(x => x.Attribute(NoNamespace.r) != null &&
                                                                                x.Attribute(NoNamespace.r)?.Value != "" && x.Descendants(S.c).Any() &&
                                                                                x.Descendants(S.c).Attributes(NoNamespace.t).Count() < pContent.NbColumns)
                                                    ?? // Case 3: Any row but the first (avoiding Header)
                                                    allRows.FirstOrDefault(x => x.Attribute(NoNamespace.r) != null &&
                                                                                x.Attribute(NoNamespace.r)?.Value != "" &&
                                                                                x.Attribute(NoNamespace.r)?.Value != "1" && x.Descendants(S.c).Any());

                                                if (oneSerieRow != null)
                                                {
                                                    var previousRowValue = Convert.ToInt32(oneSerieRow.Attribute(NoNamespace.r)?.Value);
                                                    while (nbRows < pContent.NbRows)
                                                    {
                                                        var newRow = new XElement(oneSerieRow);
                                                        var _xAttribute = newRow.Attribute(NoNamespace.r);
                                                        if (_xAttribute != null) _xAttribute.Value = (nbRows + 1).ToString(); // DCO Correction, ROW ID starts at 1
                                                        var serieCells = newRow.Descendants(S.c);
                                                        foreach (var oneCell in serieCells)
                                                        {
                                                            if (oneCell.Attribute(NoNamespace.r) == null) continue;


                                                            var previousFormula = oneCell.Attribute(NoNamespace.r)?.Value;
                                                            // We extrapolate
                                                            int indexRow;
                                                            int indexCol;
                                                            WorksheetAccessorExt.GetRowColumnValue(previousFormula, out indexRow, out indexCol);
                                                            int newRowValue = nbRows + 1 + (indexRow - previousRowValue);
                                                            var _attribute = oneCell.Attribute(NoNamespace.r);
                                                            if (_attribute != null) _attribute.Value = string.Concat(WorksheetAccessor.GetColumnId(indexCol), newRowValue.ToString());

                                                            if (oneCell.Attributes(NoNamespace.t).Any() != true) continue;
                                                            var vElement = oneCell.Descendants(S.v).FirstOrDefault();
                                                            if (vElement != null)
                                                                vElement.Value = WorksheetAccessorExt.AddSharedStringValue(spreadsheetDoc, "").ToString();
                                                            // ---
                                                        }
                                                        shData.Add(newRow);
                                                        nbRows += 1;
                                                    }
                                                }
                                                else { LogHelper.Instance.LogWarn("Adding Rows: Could not find a row without a SharedString element."); }
                                            }
                                            //-----

                                            // Define Sheet Dimension ================================================================
                                            int minStartRow = -1;
                                            int minStartCol = -1;
                                            int maxEndRow = -1;
                                            int maxEndCol = -1;
                                            var entireScope = allRows.SelectMany(x => x.Descendants(S.c).Attributes(NoNamespace.r).Select(y => y.Value));
                                            foreach (var oneFormula in entireScope)
                                            {
                                                var startRow = -1;
                                                var endRow = -1;
                                                var startCol = -1;
                                                var endCol = -1;
                                                WorksheetAccessorExt.GetFormulaCoord(oneFormula, out startRow, out startCol,
                                                    out endRow, out endCol);
                                                if (minStartRow == -1 || startRow < minStartRow) { minStartRow = startRow; }
                                                if (minStartCol == -1 || startCol < minStartCol) { minStartCol = startCol; }
                                                if (maxEndRow == -1 || endRow > maxEndRow) { maxEndRow = endRow; }
                                                if (maxEndCol == -1 || endCol > maxEndCol) { maxEndCol = endCol; }
                                            }
                                            XElement sheetDimension = shPart.Descendants(S.s + "dimension").FirstOrDefault();
                                            if (sheetDimension?.Attribute(NoNamespace._ref) != null)
                                                sheetDimension.Attribute(NoNamespace._ref)?.SetValue(WorksheetAccessorExt.SetFormula("", minStartRow, minStartCol, maxEndRow, maxEndCol, false));
                                            // ====================================================================================

                                            int contentEltCount = pContent.Data?.Count() ?? 0;
                                            // Apply values =======================================================================
                                            for (int ctn = 0; ctn < nbRows; ctn++)
                                            {
                                                var oneRow = allRows.ElementAt(ctn);
                                                // TODO: We may have to correct the "spans" in:  <row r="1" spans="1:3" 
                                                List<XElement> allCells = oneRow.Descendants(S.c).ToList();
                                                var ctCell = 0;
                                                int nbCells = allCells.Count;
                                                for (int ctc = 0; ctc < nbCells; ctc++)
                                                {
                                                    var oneCell = allCells[ctc];

                                                    // We remove cell if they are not defined as content columns
                                                    if (ctCell >= pContent.NbColumns)
                                                    {
                                                        LogHelper.Instance.LogWarn("More cells that defined content ");
                                                        if (null != oneCell.Parent) { oneCell.Remove(); }

                                                        ctc -= 1;
                                                        nbCells -= 1;
                                                        continue;
                                                    }

                                                    // We inject text
                                                    var targetText = ((ctRow * pContent.NbColumns + ctCell) < contentEltCount ?
                                                        pContent.Data?.ElementAt(ctRow * pContent.NbColumns + ctCell) :
                                                        string.Empty);
                                                    if (null != targetText && !"<KEEP>".Equals(targetText)) // Keep for managing UniversalGraph
                                                    {
                                                        var isSharedString = oneCell.Attribute(NoNamespace.t);
                                                        if (null != isSharedString && "s".Equals(isSharedString.Value))
                                                        {
                                                            if ("".Equals(targetText)) { LogHelper.Instance.LogWarn("Target Text empty for Shared String, this can create abnormal behavior."); }
                                                            var idx = Convert.ToInt32(oneCell.Value);
                                                            WorksheetAccessorExt.SetSharedStringValue(spreadsheetDoc, idx, targetText);
                                                        }
                                                        else
                                                        {
                                                            XElement cell = oneCell.Descendants(S.v).FirstOrDefault();
                                                            if (null != cell) { cell.Value = targetText; }
                                                            else { LogHelper.Instance.LogWarn("No correct cell value found"); }
                                                        }
                                                    }
                                                    ctCell += 1;
                                                }
                                                ctRow += 1;
                                            }
                                        }
                                        else
                                        {
                                            LogHelper.Instance.LogWarn("Embedded spreadsheet is not formatted correctly");
                                        }
                                        // ====================================================================================
                                        #endregion Get and use of the data content (Only data, no titles)

                                        // We modify Table Definition (defining scope of Graph)
                                        foreach (TableDefinitionPart t in wsp.TableDefinitionParts)
                                        {
                                            t.Table.Reference = String.Concat(WorksheetAccessor.GetColumnId(1), 1, ":",
                                                WorksheetAccessor.GetColumnId(pContent.NbColumns), pContent.NbRows);

                                            // We reduce the scope TableColumn if needed
                                            var columnCount = t.Table.TableColumns.Count;
                                            for (int ctCol = 0; ctCol < columnCount; ctCol += 1)
                                            {
                                                var tabColumn = t.Table.TableColumns.ElementAt(ctCol);
                                                if (ctCol >= pContent.NbColumns)
                                                {
                                                    tabColumn.Remove();
                                                    ctCol -= 1;
                                                    columnCount -= 1; // DCO - 10/23/2012 - Correction when reducing scope to a column (count is corrected afterwards).
                                                    continue;
                                                }

                                                // We align column name with the correct Shared String
                                                if (!string.IsNullOrEmpty(pContent.Data?.ElementAt(ctCol)) && ctCol < pContent.NbColumns && "<KEEP>" != pContent.Data.ElementAt(ctCol))
                                                {
                                                    tabColumn.SetAttribute(new OpenXmlAttribute("", "name", "", pContent.Data.ElementAt(ctCol)));
                                                }
                                            }

                                            // The Count attribute is not updated correctly, so we do the work for them :)
                                            if (pContent.NbColumns < t.Table.TableColumns.Count)
                                            {
                                                t.Table.TableColumns.SetAttribute(new OpenXmlAttribute("", "count", "", pContent.NbColumns.ToString()));
                                            }
                                        }
                                        // We save the XML content
                                        wsp.PutXDocument(shPart);
                                    }
                                    // We update cached data in Word document
                                    UpdateCachedValues(pClient, chartPart, spreadsheetDoc, pContent);
                                    #endregion Associated Data File content Management
                                }
                                // Write the modified memory stream back
                                // into the embedded package part.
                                using (Stream s = embedPackage.GetStream())
                                {
                                    ms.WriteTo(s);
                                    s.SetLength(ms.Length);
                                }
                            }
                        }
                        else
                        {
                            LogHelper.Instance.LogWarn("No embedded excel file found.");
                        }
                        #endregion Associated content management
                    }
                    else
                    {
                        LogHelper.Instance.LogWarn("No spreadsheet identifier found.");
                    }

                    #region Additionnal parameters

                    if (null == pContent.GraphOptions || !pContent.GraphOptions.HasConfiguration) return;
                    Chart chart = chartPart.ChartSpace.Descendants<Chart>().FirstOrDefault();
                    PlotArea p_c = chart?.PlotArea;
                    var primaryVerticalAxis = p_c?.Descendants<ValueAxis>().FirstOrDefault(_ => "valAx".Equals(_.LocalName));
                    if (pContent.GraphOptions.AxisConfiguration.VerticalAxisMinimal.HasValue)
                    {
                        if (primaryVerticalAxis != null) primaryVerticalAxis.Scaling.MinAxisValue.Val = DoubleValue.FromDouble(pContent.GraphOptions.AxisConfiguration.VerticalAxisMinimal.Value);
                    }
                    if (!pContent.GraphOptions.AxisConfiguration.VerticalAxisMaximal.HasValue) return;
                    // ReSharper disable once PossibleInvalidOperationException
                    if (primaryVerticalAxis != null) primaryVerticalAxis.Scaling.MaxAxisValue.Val = DoubleValue.FromDouble(pContent.GraphOptions.AxisConfiguration.VerticalAxisMaximal.Value);

                    #endregion Additionnal parameters
                }
            }
            catch (Exception exception)
            {
                LogHelper.Instance.LogError("Unexpected exception thrown.", exception);
                throw;
            }
           
        }

        private static IEnumerable<XElement> GetElementsInPlaceHolder(ReportData pClient, IEnumerable<XElement> phElem)
        {
            switch (pClient.ReportType)
            {
                case FormatType.Word: { return phElem.Elements(); }
                case FormatType.PowerPoint: { return phElem; }
                case FormatType.Excel: // TODO : Finalize Excel alimentation
                    { return null; }
                default: { return null; }
            }
        }
        private static IEnumerable<XElement> GetElements(OpenXmlElement block, FormatType formatType)
        {
            switch (formatType)
            {
                case FormatType.Word:
                {
                    if (!(block is OXW.SdtBlock)) return block.Select(_ => XElement.Parse(_.OuterXml));
                    List<XElement> back = new List<XElement>();
                    back.AddRange(block.Descendants<OXW.SdtContentBlock>().Select(_ => XElement.Parse(_.OuterXml)));
                    back.AddRange(block.Descendants<OXW.SdtContentRun>().Select(_ => XElement.Parse(_.OuterXml)));
                    return back;
                }
                case FormatType.PowerPoint: { return block.Select(_ => XElement.Parse(_.OuterXml)); }
                case FormatType.Excel: // TODO : Finalize Excel alimentation
                    { return null; }
                default: { return null; }
            }
        }
        private static ChartPart GetChartPart(OpenXmlPackage package, BlockItem block, string chartId)
        {
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (package is WordprocessingDocument) { return (ChartPart)((WordprocessingDocument)package).MainDocumentPart.GetPartById(chartId); }
            if (package is PresentationDocument) { return (ChartPart)((SlidePart)block.Container).GetPartById(chartId); }
            if (package is SpreadsheetDocument) { return (ChartPart)((WorksheetPart)block.Container).GetPartById(chartId); }
            return null;
        }
        private static string GetSpreadsheetId(OpenXmlPart chartPart)
        {
            string id = null;
            XElement extData = chartPart.GetXDocument().Descendants(C.externalData).FirstOrDefault();
            if (extData?.Attribute(R.id) != null)
            {
                id = extData.Attribute(R.id)?.Value;
            }
            return id;
        }
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static void UpdateCachedValues(ReportData conf, OpenXmlPart chartPart, SpreadsheetDocument spreadsheetDoc, TableDefinition content)
        {
            try
            {
                var chartDoc = chartPart.GetXDocument();

                // We update the cached value now
                XElement chartCached = chartDoc.Descendants(C.chart).FirstOrDefault();
                if (null != chartCached)
                {
                    // We look for Chart Title -----------------------------------------------
                    var titleElement = chartCached.Descendants(C.title).FirstOrDefault();
                    if (titleElement == null) // No title so we need to enforce no will be displayed when regenerating Cached Values
                    {
                        var autoTitleDeleted = chartCached.Descendants(C.autoTitleDeleted).FirstOrDefault();
                        if (autoTitleDeleted == null)
                        {
                            autoTitleDeleted = new XElement(C.autoTitleDeleted);
                            chartCached.Add(autoTitleDeleted);
                        }
                        autoTitleDeleted.SetAttributeValue(NoNamespace.val, "1");
                    }
                    // ----------------------------------------------------------------------


                    // Series in Graph ------------------------------------------------------
                    List<XElement> cSerie = chartCached.Descendants(C.ser).ToList();
                    var formulaToCheck = new List<XElement>();
                    int cSerieCount = cSerie.Count;
                    for (int ctSer = 0; ctSer < cSerieCount; ctSer += 1)
                    {
                        #region Serie Treatment
                        var oneSerie = cSerie[ctSer];

                        // We check if the content as been given for this serie
                        formulaToCheck.Clear();
                        formulaToCheck.AddRange(oneSerie.Descendants(C.numRef).Descendants(C.f).Union(oneSerie.Descendants(C.strRef).Descendants(C.f)));

                        bool isSerieDeleted = false;
                        foreach (var oneFormula in formulaToCheck)
                        {
                            int startRow;
                            int startCol;
                            int endRow;
                            int endCol;
                            string sheetName = WorksheetAccessorExt.GetFormulaCoord(oneFormula.Value, out startRow, out startCol, out endRow, out endCol);
                            if ((startRow > content.NbRows && endRow > content.NbRows) ||
                                (startCol > content.NbColumns && endCol > content.NbColumns))
                            {
                                // We need to remove the whole serie as it is out of scope
                                isSerieDeleted = true;
                                oneSerie.Remove();
                                break;
                            }
                            // ignore if mapped to a single cell of first row or first column
                            if ((endCol - startCol == 0 && endRow - startRow == 0) && (startCol == 1 || startRow == 1))
                            	continue;
                            // otherwise this is mapped to a range: update
                            if (startCol == endCol && endRow != content.NbRows && startRow != endRow)
                            {
                                endRow = content.NbRows; // -startRow;
                                oneFormula.Value = WorksheetAccessorExt.SetFormula(sheetName, startRow, startCol, endRow, endCol);
                            }
                            if (startRow == endRow && endCol != content.NbColumns && startCol != endCol)
                            {
                                endCol = content.NbColumns; // -startRow;
                                oneFormula.Value = WorksheetAccessorExt.SetFormula(sheetName, startRow, startCol, endRow, endCol);
                            }
                        }
                        // ------------

                        // No need to go further, the serie is deleted (or will generate error "parent is missing")
                        if (isSerieDeleted) continue;

                        // We now update Cached Value
                        IEnumerable<XElement> cachedBlock = oneSerie.Descendants(C.strRef).Union(oneSerie.Descendants(C.numRef));

                        foreach (XElement oneCache in cachedBlock)
                        {
                            var formula = oneCache.Descendants(C.f).FirstOrDefault();
                            var allCells = oneCache.Descendants(C.pt); //.ToList()

                            if (formula != null)
                            {
                                int startRow;
                                int startCol;
                                int endRow;
                                int endCol;
                                string sheetName;
                                WorksheetPart sheet = WorksheetAccessorExt.GetFormula
                                    (spreadsheetDoc, formula.Value
                                    ,out sheetName, out startRow
                                    ,out startCol, out endRow
                                    ,out endCol
                                    );
                                if (sheet != null)
                                {
                                    int indexCell = 0;
                                    XElement previousCell = null;
                                    for (int ctRow = startRow; ctRow <= endRow; ctRow += 1)
                                    {
                                        for (int ctCol = startCol; ctCol <= endCol; ctCol += 1)
                                        {
                                            var cachedCell = allCells.FirstOrDefault(x => x.Attribute(NoNamespace.idx) != null &&
                                                                                          x.Attribute(NoNamespace.idx)?.Value == indexCell.ToString());
                                            if (cachedCell == null && previousCell != null)
                                            {
                                                cachedCell = new XElement(previousCell);
                                                cachedCell.Attribute(NoNamespace.idx).Value = indexCell.ToString();
                                                previousCell.Parent?.Add(cachedCell);
                                            }

                                            if (ctRow > content.NbRows || ctCol > content.NbColumns)
                                            {
                                                indexCell += 1;
                                                cachedCell.Remove();
                                                continue;
                                            }

                                            var cellValue = WorksheetAccessor.GetCellValue(spreadsheetDoc, sheet, ctCol, ctRow);

                                            var cachedCellValue = cachedCell.Descendants(C.v).FirstOrDefault();
                                            if (null != cachedCellValue && null != cellValue)
                                            {
                                                cachedCellValue.Value = cellValue.ToString();
                                            }
                                            else
                                            {
                                                LogHelper.Instance.LogWarn("No cell value in cached cell or no cell value in attached spreadsheet.");
                                            }

                                            indexCell += 1;
                                            previousCell = cachedCell;
                                        }
                                    }

                                    // --------------------
                                    // We clean other IDX
                                    var maxId = (endCol - startCol + 1) * (endRow - startRow + 1);
                                    var outOfRangeCells =
                                               allCells.Where(x => x.Attribute(NoNamespace.idx) != null &&
                                                                            Convert.ToInt32(x.Attribute(NoNamespace.idx)?.Value) >= maxId).ToList();
                                    var nbOutOfRangeCells = outOfRangeCells.Count;
                                    for (int ctCellOut = 0; ctCellOut < nbOutOfRangeCells; ctCellOut += 1)
                                        outOfRangeCells[ctCellOut].Remove();
                                    // ---------------------

                                    // Update number of elements ptCount
                                    var ptCountCell = oneCache.Descendants(C.ptCount).FirstOrDefault();
                                    if (ptCountCell?.Attribute(NoNamespace.val) != null)
                                        ptCountCell.Attribute(NoNamespace.val).Value = allCells.Count().ToString();
                                }
                                else
                                {
                                    LogHelper.Instance.LogWarn("Invalid sheet for cached data.");
                                }
                            }
                            else
                            {
                                LogHelper.Instance.LogWarn("No formula defining range of cached data.");
                            }
                        }

                        // Data Label Modification
                        if (content.GraphDataLabelText)
                        {
                            // TODO: Create elements if it does not exist
                            // As of today, the user needs to display Axis value, that will be changed by text by the following code
                            IEnumerable<XElement> dataLabels = oneSerie.Descendants(C.dLbl);
                            var tempLbl = dataLabels.FirstOrDefault();
                            if (tempLbl != null)
                            {
                                XElement parentNode = tempLbl.Parent;

                                // We copied new row if needed
                                int nbDataLabels = dataLabels.Count();
                                if (content.DataLabel.Count() > nbDataLabels)
                                {
                                    var idx = nbDataLabels - 1;
                                    var lastElement = dataLabels.ElementAt(idx);
                                    while (content.DataLabel.Count() > nbDataLabels)
                                    {
                                        XElement newElement = new XElement(lastElement);
                                        var idxValue = newElement.Descendants(C.idx).FirstOrDefault();
                                        // We need to assign IDX at 0 so we're sure it is in the correct range, we change the value afterwards
                                        if (idxValue?.Attribute(NoNamespace.val) != null)
                                        {
                                            idxValue.Attribute(NoNamespace.val).SetValue(0);
                                        }
                                        parentNode?.AddFirst(newElement);
                                        nbDataLabels += 1;
                                    }
                                }
                                // We remove DataLabel is there are too many
                                nbDataLabels = dataLabels.Count();
                                if (content.DataLabel.Count() < nbDataLabels)
                                {
                                    while (content.DataLabel.Count() < nbDataLabels)
                                    {
                                        dataLabels.ElementAt(nbDataLabels - 1).Remove();
                                        nbDataLabels -= 1;
                                    }
                                }
                            }
                            // ----

                            int nbDlbl = dataLabels.Count();
                            for (int ctDlbl = 0; ctDlbl < nbDlbl; ctDlbl += 1)
                            {
                                var oneDataLbl = dataLabels.ElementAt(ctDlbl);
                                var idxTag = oneDataLbl.Descendants(C.idx).FirstOrDefault();
                                if (idxTag?.Attribute(NoNamespace.val) != null)
                                {
                                    idxTag.Attribute(NoNamespace.val).SetValue(ctDlbl);
                                }

                                var indexVal = oneDataLbl.Descendants(C.idx).FirstOrDefault();
                                if (indexVal?.Attribute(NoNamespace.val) == null || "".Equals(indexVal.Attribute(NoNamespace.val).Value))
                                {
                                    continue;
                                }
                                int idxDataLbl = Convert.ToInt32(indexVal.Attribute(NoNamespace.val).Value);
                                var textTag = oneDataLbl.Descendants(C.tx).FirstOrDefault();
                                var textValue = textTag?.Descendants(A.t).FirstOrDefault();
                                textValue?.SetValue(content.DataLabel.ElementAt(idxDataLbl));
                            }
                        }
                        #endregion Serie Treatment
                    }
                }
                else
                {
                    LogHelper.Instance.LogWarn("No cached chart data found.");
                }

                chartPart.PutXDocument(chartDoc);
            }
            catch (Exception exception)
            {
                LogHelper.Instance.LogError("Unexpected exception thrown.", exception);
                throw;
            }
        }
        #endregion METHODS
    }
}
