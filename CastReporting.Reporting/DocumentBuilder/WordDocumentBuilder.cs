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
using System.Linq;
using System.Xml.Linq;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CastReporting.Reporting.Builder
{
    internal class WordDocumentBuilder : DocumentBuilderBase
    {
      

        #region CONSTRUCTORS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public WordDocumentBuilder(ReportData client) : base(client) 
        { 
        }
        #endregion CONSTRUCTORS

        #region METHODS
        /// <summary>
        /// Construct the current document.
        /// </summary>
        public override void BuildDocument()
        {
            var wordDoc = (WordprocessingDocument)Package;
            if (null == wordDoc) { return; }
            foreach (var onePart in wordDoc.MainDocumentPart.HeaderParts)
            {
                ParseDocument(onePart);
            }
            ParseDocument();
            foreach (FooterPart onePart in wordDoc.MainDocumentPart.FooterParts)
            {
                ParseDocument(onePart);
            }

            /* To make the word document read only
            var dp = new DocumentProtection
            {
                Edit = DocumentProtectionValues.ReadOnly,
                Enforcement = OnOffValue.FromBoolean(true)
            };
            if (Equals(wordDoc.MainDocumentPart.DocumentSettingsPart.Settings, null))
            {
                wordDoc.MainDocumentPart.DocumentSettingsPart.Settings = new Settings();
            }
            wordDoc.MainDocumentPart.DocumentSettingsPart.Settings.AppendChild(dp);
            wordDoc.MainDocumentPart.DocumentSettingsPart.Settings.Save();
            */
        }
        /// <summary>
        /// Returns the block configuration of the block item given in parameter.
        /// </summary>
        /// <param name="block">Block where the block configuration parameters will be found.</param>
        /// <returns>The block configuration of the block item given in parameter.</returns>
        protected override BlockConfiguration GetBlockConfiguration(BlockItem block)
        {
            string alias = null;
            string tag = null;
            var xElement = block.OxpBlock.Descendants<SdtProperties>().FirstOrDefault();
            if (xElement != null)
            {
                var aliasElement = xElement.Descendants<SdtAlias>().FirstOrDefault();
                if (aliasElement != null) { alias = aliasElement.Val.Value; }
                var tagElement = xElement.Descendants<Tag>().FirstOrDefault();
                if (tagElement != null) { tag = tagElement.Val.Value; }
            }
            else
            {
                var elt = block.OxpBlock
                               .Descendants<DocProperties>()
                               .FirstOrDefault(_ => !string.IsNullOrWhiteSpace(_.Description));
                if (null != elt)
                {
                    tag = elt.Description;
                }
                else
                {
                    // case table is not in a content control
                    var tblElt = block.OxpBlock
                        .Descendants<TableProperties>()
                        .FirstOrDefault(_ => null != _.TableDescription);
                    if (null != tblElt)
                    {
                        tag = tblElt.TableDescription.Val;
                    }
                }
            }
            return GetBlockConfiguration(alias, tag);
        }
        /// <summary>
        /// Returns all block contained into the container given in argument.
        /// </summary>
        /// <param name="container">Container where the block items will be found.</param>
        /// <returns>All block contained into the container given in argument.</returns>
        protected override List<BlockItem> GetBlocks(OpenXmlPartContainer container)
        {
            OpenXmlPartRootElement rootContainer = GetRootContainer(container);
            var blocks = rootContainer.Descendants<SdtElement>()
                                      .Select(_ => new BlockItem
                                                   {
                                                       OxpBlock = _,
                                                       XBlock = XElement.Parse(_.OuterXml),
                                                       Container = container
                                                   })
                                      .ToList();

            // Find text and graph that are not in content control
            var addblocks = rootContainer.Descendants<DocProperties>()
                                         .Where(_ => null != _.Description
                                                     && _.Description.HasValue
                                                     && !string.IsNullOrWhiteSpace(_.Description.Value)
                                                     && (_.Parent.Parent is Drawing || _.Parent.Parent is Table))
                                         .Select(_ => new BlockItem
                                         {
                                             OxpBlock = _.Parent.Parent,
                                             XBlock = XElement.Parse(_.Parent.Parent.OuterXml),
                                             Container = container
                                         })
                                         .ToList();
            blocks.AddRange(addblocks);
            
            // Find tables that are not in content control
            var addblocks2 = rootContainer.Descendants<TableProperties>()
                                         .Where(_ => null != _.TableDescription
                                                && _.Parent is Table)
                                         .Select(_ => new BlockItem
                                         {
                                             OxpBlock = _.Parent,
                                             XBlock = XElement.Parse(_.Parent.OuterXml),
                                             Container = container
                                         })
                                         .ToList();
            blocks.AddRange(addblocks2);
            return blocks;
        }

        private static OpenXmlPartRootElement GetRootContainer(OpenXmlPartContainer container)
        {
            var _part = container as HeaderPart;
            if (_part != null)
            {
                return _part.Header;
            }
            else if (container is FooterPart)
            {
                return ((FooterPart)container).Footer;
            }
            else
            {
                return ((WordprocessingDocument)container).MainDocumentPart.Document;
            }
        }
        #endregion METHODS
    }
}

