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
    [BlockType("TEXT")]
    public abstract class TextBlock
    {
        #region ABSTRACT - To be implemented by Inherited children
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract string Content(ReportData client, Dictionary<string, string> options);
        #endregion ABSTRACT - To be implemented by Inherited children

        #region PROPERTIES

        /// <summary>
        /// Block Type Name
        /// </summary>
        public static string BlockTypeName => "TEXT";

        #endregion PROPERTIES

        #region METHODS
        public static bool IsMatching(string blockType)
        {
            return BlockTypeName.Equals(blockType);
        }

        public string GetContent(ReportData client, Dictionary<string, string> options)
        {
            return Content(client, options);
        }

        public static void BuildContent(ReportData client, OpenXmlPartContainer container, BlockItem block, string blockName, Dictionary<string, string> options)
        {
            TextBlock instance = BlockHelper.GetAssociatedBlockInstance<TextBlock>(blockName);
            if (null == instance) return;
            LogHelper.LogDebugFormat("Start TextBlock generation : Type {0}", blockName);
            Stopwatch treatmentWatch = Stopwatch.StartNew();
            string content = instance.Content(client, options);
            ApplyContent(client, container, block, content);
            treatmentWatch.Stop();
            LogHelper.LogDebugFormat
            ("End TextBlock generation ({0}) in {1} ms"
                , blockName
                , treatmentWatch.ElapsedMilliseconds.ToString()
            );
        }
        public static void ApplyContent(ReportData client, OpenXmlPartContainer container, BlockItem block, string content)
        {
            var contentblock = GetTextContentBlock(client, block);
            if (null != contentblock)
            {
                UpdateBlock(client, container, contentblock, content);
            }
        }
        
        private static void UpdateBlock(ReportData client, OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            switch (client.ReportType)
            {
                case FormatType.Word: { UpdateWordBlock(container, block, content); } break;
                case FormatType.PowerPoint: { UpdatePowerPointBlock(container, block, content); } break;
                case FormatType.Excel: { UpdateExcelBlock(container, block, content); } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        // ReSharper disable once UnusedParameter.Local
        private static void UpdatePowerPointBlock(OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            OXP.Shape shape = (OXP.Shape)block.CloneNode(true);
            OXD.Run run = (OXD.Run)shape.TextBody.Descendants<OXD.Run>().First().CloneNode(true);
            run.Text = new OXD.Text(content);
            OXD.Paragraph paragraph = shape.TextBody.GetFirstChild<OXD.Paragraph>();
            paragraph.RemoveAllChildren<OXD.Run>();
            OXD.EndParagraphRunProperties endP = paragraph.GetFirstChild<OXD.EndParagraphRunProperties>();
            paragraph.InsertBefore(run, endP);
            block.Parent.ReplaceChild(shape, block);
        }
        private static void UpdateWordBlock(OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            OXW.Text new_text = new OXW.Text(content);
            if (!string.IsNullOrEmpty(content) && (char.IsWhiteSpace(content[0]) || char.IsWhiteSpace(content[content.Length-1]))) {
            	new_text.Space = SpaceProcessingModeValues.Preserve;
            }
            OXW.Run run = new OXW.Run(new_text);
            OXW.RunProperties originalRunProp = block.Descendants<OXW.RunProperties>().FirstOrDefault();
            if (originalRunProp != null)
            {
            	run.RunProperties = (OXW.RunProperties)originalRunProp.CloneNode(true);
            }
            OpenXmlElement finalBlock = run;
            if ("SdtRun" == block.Parent.GetType().Name)
            {
                // case text block in a content control
                var cbcontainer = block.Parent;
                cbcontainer?.Parent.ReplaceChild(finalBlock, cbcontainer);
            }
            else
            {
                // case text block is in a text box
                var oldTxt = block.Descendants<OXW.Run>().FirstOrDefault()?.Parent;
                oldTxt?.RemoveAllChildren();
                oldTxt?.AppendChild(finalBlock);
            }
            var docPart = container.GetPartsOfType<MainDocumentPart>().FirstOrDefault();
            if (docPart == null)
            {
            	var p = container as OpenXmlPart;
            	if (p != null)
            		docPart = p.GetParentParts().FirstOrDefault(_ => _ is MainDocumentPart) as MainDocumentPart;
            }
            docPart?.Document.Save();
        }
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void UpdateExcelBlock(OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            // TODO : Finalize Excel alimentation
            throw new NotImplementedException();
        }
        private static OpenXmlElement GetTextContentBlock(ReportData client, BlockItem block)
        {
            switch (client.ReportType)
            {
                case FormatType.Word:
                    var txtContent = block.OxpBlock.Descendants<OXW.SdtContentRun>().FirstOrDefault();
                    return txtContent ?? block.OxpBlock;
                    // case text is in a text box
                case FormatType.PowerPoint: return block.OxpBlock;
                case FormatType.Excel:
                    return null;
                default: return null;
            }
        }
        #endregion METHODS
    }
}
