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
        /// Snapshots
        /// </summary>
        [XmlElement("snapshot")]
        public XmlSnapshot Snapshot { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        [XmlElement("file")]
        public XmlTagName File { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Save XML Cast Report
        /// </summary>
        /// <param name="pOutputPath"></param>
        public void SaveXML(string pOutputPath)
        {
            if (string.IsNullOrEmpty(pOutputPath))
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
        /// <param name="pOutputPath"></param>
        public static XmlCastReport LoadXML(string pInputPath)
        {
            if (string.IsNullOrEmpty(pInputPath))
                throw new ArgumentNullException("pInputPath");

            XmlCastReport report = null;
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
            //XmlTextReader schemaReader = new XmlTextReader("relativeSchemaPath");
            //XmlSchemaSet schemaSet = new XmlSchemaSet();
            // TO DO !!
            if (this.Application == null || string.IsNullOrEmpty(this.Application.Name))
                return false;
            if (this.Webservice == null || string.IsNullOrEmpty(this.Webservice.Name))
                return false;
            if (this.Template == null || string.IsNullOrEmpty(this.Template.Name))
                return false;
            if (this.Username == null || string.IsNullOrEmpty(this.Username.Name))
                return false;
            if (this.Password == null || string.IsNullOrEmpty(this.Password.Name))
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
                return string.Format
                    ("Cast Report '{0}' to DB '{1}' and template '{2}'"
                    , (Application != null && !string.IsNullOrEmpty(Application.Name)) ? Application.Name : "?"
                    , (Database != null && !string.IsNullOrEmpty(Database.Name)) ? Database.Name : "?"
                    , (Template != null && !string.IsNullOrEmpty(Template.Name)) ? Template.Name : "?"
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
