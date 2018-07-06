using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "file")]
    public class CodeFile
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public string GetSiteId()
        {
            return Href.Split('/')[2];
        }
        public string GetFileId()
        {
            return Href.Split('/')[4];
        }

    }
}