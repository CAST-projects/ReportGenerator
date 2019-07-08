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

        public ExtendProxy(string login, string password, bool nugetApiKey)
        {
            // To connect to cast extend to download latest version of reports
            if (nugetApiKey)
            {
                Headers.Add("x-nuget-apikey", password);
            }
            else
            {
                string credentials = CreateBasicAuthenticationCredentials(login, password);
                Headers.Add(HttpRequestHeader.Authorization, credentials);
            }
        }

        private static string CreateBasicAuthenticationCredentials(string userName, string password)
        {
            string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
            var returnValue = $"Basic {base64UsernamePassword}";
            return returnValue;
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
