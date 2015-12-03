/*
 *   Copyright (c) 2015 CAST
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
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml.Packaging;

namespace CastReporting.Reporting.Builder
{
    internal class ExcelDocumentBuilder : DocumentBuilderBase
    {
       

        #region CONSTRUCTORS
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public ExcelDocumentBuilder(ReportData client) : base(client) 
        { 
        }
        #endregion CONSTRUCTORS

        #region METHODS
        /// <summary>
        /// Construct the current document.
        /// </summary>
        public override void BuildDocument()
        {
            var excelDoc = (SpreadsheetDocument)base.Package;
            if (null == excelDoc) { return; }
            foreach (WorksheetPart part in excelDoc.WorkbookPart.WorksheetParts)
            {
                this.ParseDocument(part);
            }
        }
        /// <summary>
        /// Returns the block configuration of the block item given in parameter.
        /// </summary>
        /// <param name="block">Block where the block configuration parameters will be found.</param>
        /// <returns>The block configuration of the block item given in parameter.</returns>
        protected override BlockConfiguration GetBlockConfiguration(BlockItem block)
        {
            // TODO : Finalize Excel alimentation
            throw new NotImplementedException();
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
        #endregion METHODS
    }
}
