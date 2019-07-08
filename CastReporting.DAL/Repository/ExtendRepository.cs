using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Cast.Util;
using Cast.Util.Log;
using CastReporting.Domain.WSObjects;
using CastReporting.Mediation;
using CastReporting.Mediation.Interfaces;

namespace CastReporting.Repositories.Repository
{
    public class ExtendRepository : IDisposable
    {
        protected IExtendProxy Proxy;
        private readonly string _url;
        private const string QUERY_POST_LATEST_VERSION = "{0}/api/search/packages/{1}/latest";
        private const string QUERY_GET_LATEST_VERSION = "{0}/api/package/download/{1}/{2}";

        public ExtendRepository(string url, string login, string password, bool nugetKey)
        {
            Proxy = new ExtendProxy(login, password, nugetKey);
            _url = url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
        }

        public void GetLatestTemplateVersion(string packageId, string templatePath)
        {
            // get the latest version id for the extension
            string latestverion = PostForLatestVersion(packageId, _url);

            // next download this extension
            string archive = GetLatestVersion(packageId, _url, latestverion);

            // then install it
            PathUtil.UnzipAndCopy(archive, templatePath);
        }

        public string PostForLatestVersion(string packageId, string extendUrl)
        {

            string query = string.Format(QUERY_POST_LATEST_VERSION, extendUrl, packageId);
            try
            {
                var jsonString = Proxy.UploadJsonString(query);
                var serializer = new DataContractJsonSerializer(typeof(ExtendPackage));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
                try
                {
                    ExtendPackage res = serializer.ReadObject(ms) as ExtendPackage;
                    return res?.Version;
                }
                finally
                {
                    ms.Close();
                }

            }
            catch (WebException e)
            {
                LogHelper.Instance.LogError(e.Message);
                return null;
            }
        }

        public string GetLatestVersion(string packageId, string extendUrl, string version)
        {
            string query = string.Format(QUERY_GET_LATEST_VERSION, extendUrl, packageId, version);
            string filename = packageId + "." + version;
            try
            {
                Proxy.DownloadExtension(query, filename);
            }
            catch (WebException e)
            {
                LogHelper.Instance.LogError(e.Message);
            }
            return filename;
        }


        public void Dispose()
        {
            Proxy.Dispose();
        }
    }

}
