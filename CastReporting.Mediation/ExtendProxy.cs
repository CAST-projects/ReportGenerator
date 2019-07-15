using System;
using System.Net;
using System.Text;
using CastReporting.Mediation.Interfaces;

namespace CastReporting.Mediation
{
    public class ExtendProxy : WebClient, IExtendProxy
    {
        // https://extendng.castsoftware.com/api/search/packages/EXTENSION-ID/latest 
        // https://extendng.castsoftware.com/api/package/download/EXTENSION_ID/VERSION

        public ExtendProxy(string nugetApiKey)
        {
            // To connect to cast extend to download latest version of reports
            Headers.Add("x-nuget-apikey", nugetApiKey);
        }

        public string UploadJsonString(string query)
        {
            Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return UploadString(query, "POST");
        }

        public void DownloadExtension(string query, string filename)
        {
            DownloadFile(query, filename);
        }

    }
}
