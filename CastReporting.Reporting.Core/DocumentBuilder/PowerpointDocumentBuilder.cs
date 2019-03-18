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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using OXD = DocumentFormat.OpenXml.Drawing;

namespace CastReporting.Reporting.Builder
{
    internal class PowerpointDocumentBuilder : DocumentBuilderBase
    {
       

        #region CONSTRUCTORS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public PowerpointDocumentBuilder(ReportData client) : base(client) 
        { 
        }
        #endregion CONSTRUCTORS

        #region METHODS
        /// <summary>
        /// Construct the current document.
        /// </summary>
        public override void BuildDocument()
        {
            var pptDoc = (PresentationDocument)Package;
            if (pptDoc?.PresentationPart == null) { return; }
            foreach (var slidepart in pptDoc.PresentationPart.SlideParts)
            {
                ApplyCorrectionOnSlide(slidepart.Slide);
                ParseDocument(slidepart);
            }
        }
        /// <summary>
        /// Returns the block configuration of the block item given in parameter.
        /// </summary>
        /// <param name="block">Block where the block configuration parameters will be found.</param>
        /// <returns>The block configuration of the block item given in parameter.</returns>
        protected override BlockConfiguration GetBlockConfiguration(BlockItem block)
        {
            string desc = string.Empty;
            var elt = block.OxpBlock
                           .Descendants<NonVisualDrawingProperties>()
                           .FirstOrDefault(_ => !string.IsNullOrWhiteSpace(_.Description));
            if (null != elt) { desc = elt.Description; }
            return GetBlockConfiguration(desc);
        }
        /// <summary>
        /// Returns all block contained into the container given in argument.
        /// </summary>
        /// <param name="container">Container where the block items will be found.</param>
        /// <returns>All block contained into the container given in argument.</returns>
        protected override List<BlockItem> GetBlocks(OpenXmlPartContainer container)
        {
            return ((SlidePart)container).Slide
                                         .Descendants<NonVisualDrawingProperties>()
                                         .Where(_ => !string.IsNullOrWhiteSpace(_.Description) && (_.Parent.Parent is Shape || _.Parent.Parent is GraphicFrame))
                                         .Select(_ => new BlockItem
                                                      {
                                                          OxpBlock = _.Parent.Parent,
                                                          XBlock = XElement.Parse(_.Parent.Parent.OuterXml),
                                                          Container = (SlidePart)container
                                                      })
                                         .ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="slide"></param>
        private static void ApplyCorrectionOnSlide(OpenXmlElement slide)
        {
            foreach (var run in slide.Descendants<OXD.Run>())
            {
                if (string.IsNullOrEmpty(run.Text.Text))
                {
                    run.Text.Text = " ";
                }
            }
        }
        #endregion METHODS
    }
}
