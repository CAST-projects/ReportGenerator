using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CastReporting.Console.Argument
{
    /// <summary>
    /// XmlCastReport Class
    /// </summary>
    [Serializable] 
    [XmlRoot("castReport", Namespace = "http://tempuri.org/CastReportSchema.xsd", IsNullable = false)]
    public sealed class XmlCastReport
    {
        #region Properties

        /// <summary>
        /// ReportType
        /// </summary>
        [XmlElement("reporttype")]
        public XmlTagName ReportType { get; set; }

        /// <summary>
        /// Webservice
        /// </summary>
        [XmlElement("webservice")]
        public XmlTagName Webservice { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [XmlElement("username")]
        public XmlTagName Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [XmlElement("password")]
        public XmlTagName Password { get; set; }

        /// <summary>
        /// API Key
        /// </summary>
        [XmlElement("apikey")]
        public XmlTagName ApiKey { get; set; }

        /// <summary>
        /// Application
        /// </summary>
        [XmlElement("application")]
        public XmlTagName Application { get; set; }

        /// <summary>
        /// Template"
        /// </summary>
        [XmlElement("template")]
        public XmlTagName Template { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        [XmlElement("database")]
        public XmlTagName Database { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        [XmlElement("domain")]
        public XmlTagName Domain { get; set; }

        /// <summary>
        /// Snapshots
        /// </summary>
        [XmlElement("snapshot")]
        public XmlSnapshot Snapshot { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        [XmlElement("file")]
        public XmlTagName File { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        [XmlElement("category")]
        public XmlTagName Category { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        [XmlElement("tag")]
        public XmlTagName Tag { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        [XmlElement("culture")]
        public XmlTagName Culture { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Save XML Cast Report
        /// </summary>
        /// <param name="pOutputPath"></param>
        public void SaveXML(string pOutputPath)
        {
            if (string.IsNullOrEmpty(pOutputPath))
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("pOutputPath");
            using (TextWriter tr = new StreamWriter(pOutputPath, false, Encoding.UTF8))
            {
                XmlSerializer sr = new XmlSerializer(typeof(XmlCastReport));
                sr.Serialize(tr, this);
            }
        }

        /// <summary>
        /// Load XML Cast Report
        /// </summary>
        /// <param name="pInputPath"></param>
        public static XmlCastReport LoadXML(string pInputPath)
        {
            if (string.IsNullOrEmpty(pInputPath))
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("pInputPath");

            XmlCastReport report;
            using (TextReader tr = new StreamReader(pInputPath, Encoding.UTF8))
            {
                XmlSerializer sr = new XmlSerializer(typeof(XmlCastReport));
                report = sr.Deserialize(tr) as XmlCastReport;
            }
            return report;
        }

        /// <summary>
        /// Check 
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            if (string.IsNullOrEmpty(Webservice?.Name))
                return false;
            if (string.IsNullOrEmpty(Template?.Name))
                return false;
            if (string.IsNullOrEmpty(Username?.Name))
                return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrEmpty(Password?.Name))
                return false;
            return true;
        }

        /// <summary>
        /// Debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                // ReSharper disable once UseStringInterpolation
                return string.Format
                    ("Cast Report '{0}' to DB '{1}' and template '{2}'"
                    , !string.IsNullOrEmpty(Application?.Name) ? Application.Name : "?"
                    , !string.IsNullOrEmpty(Database?.Name) ? Database.Name : "?"
                    , !string.IsNullOrEmpty(Template?.Name) ? Template.Name : "?"
                    );
            }
            catch
            {
                return base.ToString();
            }
        }
        #endregion
    }
}
