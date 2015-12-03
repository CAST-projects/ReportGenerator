using System;
using System.Xml.Serialization;

namespace CastReporting.Console.Argument
{
    /// <summary>
    /// XmlSnapshot class
    /// </summary>
    [Serializable]
    public class XmlSnapshot
    {
        #region Properties

        /// <summary>
        /// Current Snapshot
        /// </summary>
        [XmlElement("current")]
        public XmlTagName Current { get; set; }

        /// <summary>
        /// Previous Snapshot
        /// </summary>
        [XmlElement(Namespace="previous", IsNullable=true)]
        public XmlTagName Previous { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// Debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format
                ("Current '{0}' - Previous '{1}'"
                , (Current != null) ? Current.Name ?? "?" : "?"
                , (Previous != null) ? Previous.Name ?? "?" : "?"
                );
        }

        #endregion
    }
}
