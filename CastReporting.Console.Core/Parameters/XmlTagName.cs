using System;
using System.Xml.Serialization;

namespace CastReporting.Console.Argument
{
    /// <summary>
    /// XmlTagName Class
    /// </summary>
    [Serializable]
    public class XmlTagName
    {
        #region Properties

        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// Debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
        
        #endregion
    }
}
