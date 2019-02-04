using System;
using System.Diagnostics.CodeAnalysis;

namespace CastReporting.Console.Argument
{
    /// <summary>
    /// ArgumentReader class
    /// </summary>
    internal sealed class CRArgumentReader : IDisposable
    {
        #region Properties

        /// <summary>
        /// Retur n Help String
        /// </summary>
        public string Help => @"
CAST REPORT GENERATOR HELP - APPLICATION LEVEL
-webservice <ws_name> : Webservice URI.
-username <ws_username> : Username of Webservice for authentication.
-password <ws_password> : Password of Webservice for authentication.
-application <app_name> : Application name containing data for 
     document generation.
-template <template_name> : Required template file name for document 
     generation by batch.
-file <output_file> : Output generated file name.
-snapshot_cur <current_snapshot> (optional) : Current snapshot name.
-snapshot_prev <prev_snapshot> (optional) : Previous snapshot name.
-snapshot_cur_id <current_snapshot_id> (optional) : Current snapshot id.
-snapshot_prev_id <prev_snapshot_id> (optional) : Previous snapshot id.
-domain <domain> : the name of the rest api domain to select the application
-culture <> : the culture when you do not want the one in the settings
<full_xml_path> (optional) : XML file path checked by CastReportSchema.xsd 
     and containing required arguments for document generation by batch.
     Replace console arguments « -webservice », « -application », 
     « -template », « -file » and « -snapshot ».

CAST REPORT GENERATOR HELP - PORTFOLIO LEVEL
-reporttype : Portfolio --Mandatory and must always be the first parameter
-category : Category Name --This is optional
-tag : Tage Name --This is optional
-webservice <ws_name> : Webservice URI.
-username <ws_username> : Username of Webservice for authentication.
-password <ws_password> : Password of Webservice for authentication.
-application <app_name> : Application name containing data for 
     document generation.
-template <template_name> : Required template file name for document 
     generation by batch.
-file <output_file> : Output generated file name.
<full_xml_path> (optional) : XML file path checked by CastReportSchema.xsd 
     and containing required arguments for document generation by batch.
     Replace console arguments « -webservice », « -application », 
     « -template », « -file » and « -snapshot ».



";

        #endregion

        #region Methods

        /// <summary>
        /// Load arguments from array
        /// </summary>
        /// <param name="pArgs">Argument array from Main()</param>
        /// <param name="pShowHelp">Show help indicator</param>
        /// <returns>Arguments</returns>
        public XmlCastReport Load(string[] pArgs, out bool pShowHelp)
        { 
            // Do not show help by default
            pShowHelp = false;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (pArgs == null || pArgs.Length == 0)
            {
                // No Arguments
                // unrecognized type -> show help
                pShowHelp = true;
                // return nothing
                return null;
            }

            if (pArgs.Length == 1)
            {
                try
                {
                    // Load from XML file
                    XmlCastReport cstRep = XmlCastReport.LoadXML(pArgs[0]);
                    if (!cstRep.Check())
                    {
                        // XSD do not check -> show help
                        pShowHelp = true;
                        // return nothing
                        return null;
                    }
                    return cstRep;
                }
                catch
                {
                    // not enough arguments -> show help
                    pShowHelp = true;
                    // return nothing
                    return null;
                }
            }
            if (pArgs.Length % 2 == 0)
            {
                XmlCastReport castReport = new XmlCastReport() { Snapshot = new XmlSnapshot() };

                for (int i = 1; i < pArgs.Length; i += 2)
                {
                    var type = LoadType(pArgs[i - 1]);
                    var value = pArgs[i];
                    if (string.IsNullOrEmpty(type))
                    {
                        // unrecognized type -> show help
                        pShowHelp = true;
                        // return nothing
                        return null;
                    }
                    // Set Current Argument
                    SetArgument(type, value, castReport);
                }
                // all right
                if (!castReport.Check())
                {
                    // XSD do not check -> show help
                    pShowHelp = true;
                    // return nothing
                    return null;
                }
                return castReport;
            }

            // not enough arguments -> show help
            pShowHelp = true;
            // return nothing
            return null;

        }

        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            // Here don't need SuppressFinalize because there is no Destructor
            //GC.SuppressFinalize(this);
        }

        #region Private

        /// <summary>
        /// Load Type from argument
        /// </summary>
        /// <param name="pArg">Argument</param>
        /// <returns></returns>
        static string LoadType(string pArg)
        {
            if (string.IsNullOrEmpty(pArg))
                // No Arguments
                return null;

            string type = string.Empty;
            if (pArg[0] == '-')
                // Special argument, get string type
                type = pArg.Substring(1, pArg.Length - 1);
            return type;
        }

        /// <summary>
        /// Set Argument
        /// </summary>
        /// <param name="pType">Type</param>
        /// <param name="pValue">Value</param>
        /// <param name="pCastReport">Cast Report</param>
        [SuppressMessage("ReSharper", "UseNameofExpression")]
        static void SetArgument(string pType, string pValue, XmlCastReport pCastReport)
        {
            if (string.IsNullOrEmpty(pType))
                throw new ArgumentNullException("pType");
            if (pCastReport == null)
                throw new ArgumentNullException("pCastReport");
            if (pCastReport.Snapshot == null)
                throw new ArgumentException("pCastReport misses Snapshot");

            switch (pType)
            {
                case "webservice": pCastReport.Webservice = new XmlTagName() { Name = pValue }; break;
                case "application": pCastReport.Application = new XmlTagName() { Name = pValue }; break;
                case "template": pCastReport.Template = new XmlTagName() { Name = pValue }; break;
                case "file": pCastReport.File = new XmlTagName() { Name = pValue }; break;
                case "database": pCastReport.Database = new XmlTagName() { Name = pValue }; break;
                case "domain": pCastReport.Domain = new XmlTagName() { Name = pValue }; break;
                case "snapshot_cur": pCastReport.Snapshot.Current = new XmlTagName() { Name = pValue }; break;
                case "snapshot_prev": pCastReport.Snapshot.Previous = new XmlTagName() { Name = pValue }; break;
                case "snapshot_cur_id": pCastReport.Snapshot.CurrentId = new XmlTagName() { Name = pValue }; break;
                case "snapshot_prev_id": pCastReport.Snapshot.PreviousId = new XmlTagName() { Name = pValue }; break;
                case "username": pCastReport.Username = new XmlTagName() { Name = pValue }; break;
                case "password": pCastReport.Password = new XmlTagName() { Name = pValue }; break;
                case "culture": pCastReport.Culture = new XmlTagName() {Name = pValue }; break;

                case "reporttype": pCastReport.ReportType = new XmlTagName() { Name = pValue }; break;
                case "category": pCastReport.Category = new XmlTagName() { Name = pValue }; break;
                case "tag": pCastReport.Tag = new XmlTagName() { Name = pValue }; break;
            }
        }
        #endregion

        #endregion
    }
}
