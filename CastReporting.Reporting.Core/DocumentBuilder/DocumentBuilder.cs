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
using System.Linq;
using Cast.Util.Log;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml.Packaging;

namespace CastReporting.Reporting.Builder
{
    internal abstract class DocumentBuilderBase : IDocumentBuilder
    {
        #region CONSTRUCTORS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        protected DocumentBuilderBase(ReportData reportData)
        {
            ReportData = reportData;

            Package = GetPackage(reportData.FileName, ReportData.ReportType);
        }
        #endregion CONSTRUCTORS

        #region PROPERTIES
        
        /// <summary>
        /// Get/Set the client
        /// </summary>
        public ReportData ReportData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected OpenXmlPackage Package { get; set; }

        #endregion PROPERTIES

        #region METHODS

        #region Abstracts
        /// <summary>
        /// Construct the current document.
        /// </summary>
        public abstract void BuildDocument();
        /// <summary>
        /// Returns the block configuration of the block item given in parameter.
        /// </summary>
        /// <param name="block">Block where the block configuration parameters will be found.</param>
        /// <returns>The block configuration of the block item given in parameter.</returns>
        protected abstract BlockConfiguration GetBlockConfiguration(BlockItem block);
        /// <summary>
        /// Returns all block contained into the container given in argument.
        /// </summary>
        /// <param name="container">Container where the block items will be found.</param>
        /// <returns>All block contained into the container given in argument.</returns>
        protected abstract List<BlockItem> GetBlocks(OpenXmlPartContainer container);
        #endregion Abstracts

        #region Publics

        /// <summary>
        /// Parse the document in order to proceed to the alimentation of all blocks.
        /// </summary>
        public virtual void ParseDocument()
        {
            ParseDocument(Package);
        }

        /// <summary>
        /// Parse the document in order to proceed to the alimentation of all blocks into the given container.
        /// </summary>
        /// <param name="container">Container to build.</param>
        public virtual void ParseDocument(OpenXmlPartContainer container)
        {
            IList<BlockItem> blocks = GetBlocks(container);
            foreach (BlockItem block in blocks)
            {
                BlockConfiguration config = GetBlockConfiguration(block);
                try
                {
                    if (TextBlock.IsMatching(config.Type))
                    {
                        TextBlock.BuildContent(ReportData, container, block, config.Name, config.Options);
                    }
                    else if (TableBlock.IsMatching(config.Type))
                    {
                        TableBlock.BuildContent(ReportData, container, block, config.Name, config.Options);
                    }
                    else if (GraphBlock.IsMatching(config.Type))
                    {
                        GraphBlock.BuildContent(ReportData, Package,block, config.Name, config.Options);
                    }
                    else
                    {
                        LogHelper.Instance.LogWarnFormat("Block type '{0}' not found.", config.Type);
                    }
                }
                catch (Exception exception)
                {
                    string logMessage = $"Exception thrown during document parsing (BlockType : {(null != config ? config.Type : string.Empty)}, BlockName : {(null != config ? config.Name : string.Empty)})";
                    LogHelper.Instance.LogError(logMessage, exception);
                }
            }
        }
        #endregion Publics

        #region Locales
        /// <summary>
        /// Return the block configuration of the given description.
        /// </summary>
        /// <param name="description">Description of the block configuration.</param>
        /// <returns>The block configuration of the given description.</returns>
        protected BlockConfiguration GetBlockConfiguration(string description)
        {
            return GetBlockConfiguration(description, null);
        }
        /// <summary>
        /// Return the block configuration of the given alias and tag.
        /// </summary>
        /// <param name="alias">Alias text content.</param>
        /// <param name="tag">Tag text content.</param>
        /// <returns>The block configuration of the given alias and tag.</returns>
        protected BlockConfiguration GetBlockConfiguration(string alias, string tag)
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
            if (null != optionList && optionList.Length >= 2)
            {
                back.Type = optionList[0];
                back.Name = optionList[1];
                if (optionList.Length > 2 && string.IsNullOrWhiteSpace(blockOptionStr))
                {
                    blockOptionStr += $",{optionList.Skip(2).Aggregate((current, next) => $"{current},{next}")}";
                }
                back.Options = string.IsNullOrWhiteSpace(blockOptionStr) ? new Dictionary<string, string>() : ParseOptions(blockOptionStr);
            }
            return back;
        }
        /// <summary>
        /// Parse the options into the value given in argument.
        /// </summary>
        /// <param name="strOption">Value where to find options.</param>
        /// <returns>The options into the value given in argument.</returns>
        protected Dictionary<string, string> ParseOptions(string strOption)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            string[] allOpt = strOption.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string oneOpt in allOpt)
            {
                string[] defOpt = oneOpt.Split('=');
                if (defOpt.Length < 1) continue;
                string val = "";
                if (defOpt.Length >= 2) val = defOpt[1];
                options.Add(defOpt[0], val);
            }
            return options;
        }


        /// <summary>
        /// Get Package
        /// </summary>
        /// <param name="pPath"></param>
        /// <param name="reportType"></param>
        /// <returns></returns>
        private static OpenXmlPackage GetPackage(string pPath, FormatType reportType)
        {
            if (string.IsNullOrWhiteSpace(pPath)) return null;
            LogHelper.Instance.LogInfoFormat("Opening '{0}'...", pPath);
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (reportType)
            {
                case FormatType.Word: 
                { 
                    return WordprocessingDocument.Open(pPath, true); 
                }
                case FormatType.PowerPoint: 
                { 
                    return PresentationDocument.Open(pPath, true); 
                }
                case FormatType.Excel: 
                { 
                    return SpreadsheetDocument.Open(pPath, true); 
                }
            }
            return null;
        }

        #endregion Locales

        #region Inherited

        /// <summary>
        /// free memory and handle
        /// </summary>
        public virtual void Dispose()
        {
            if (Package != null)
            {
                Package.Close();
                Package.Dispose();
            }
        }
        
        #endregion Inherited

        #endregion METHODS
    }
}
