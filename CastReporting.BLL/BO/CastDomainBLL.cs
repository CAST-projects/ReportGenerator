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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CastReporting.Domain;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Cast.Util.Log;

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class CastDomainBLL : BaseBLL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public CastDomainBLL(WSConnection connection)
            : base(connection)
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CastDomain> GetDomains()
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetDomains();
            }
        }

           
        /// <summary>
        /// 
        /// </summary>
        public List<Application> GetApplications()
        {
            List<Application> applications = new List<Application>();

            var domains = GetDomains();

            using (var castRepsitory = GetRepository())
            {
                foreach (var domain in domains)
                {
					var domainApps = castRepsitory.GetApplicationsByDomain(domain.Href)?.ToList();

                    if (domainApps == null) continue;
                    foreach (var app in domainApps)
                    {
                        if (string.IsNullOrEmpty(app.Version)) {
                            app.Version = domain.Version;
                        }
                    }
					
                    applications.AddRange(domainApps);
                }
            }

           return applications.OrderBy(_ => _.Name).ToList();
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class CommonTags
        {
            public Application application { get; set; }
            public Tagg[] commonTags { get; set; }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class CommonCategoriess
        {
            public string key { get; set; }
            public string label { get; set; }
            public Tagg[] tags { get; set; }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class Tagg
        {
            public string key { get; set; }
            public string label { get; set; }
        }

        public List<Snapshot> GetAllSnapshots(Application[] applications)
        {
            List<Snapshot> _snapshots = new List<Snapshot>();
            foreach (Application _appl in applications)
            {
                int nbSnapshotsEachApp = _appl.Snapshots.Count();
                if (nbSnapshotsEachApp <= 0) continue;
                foreach (Snapshot snapshot in _appl.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                {
                    snapshot.AdgVersion = _appl.AdgVersion;
                    _snapshots.Add(snapshot);
                }
            }
            return _snapshots;
        }


        public List<Application> GetCommonTaggedApplications(string strSelectedTag)
        {
            List<Application> _commonTaggedApplications = new List<Application>();
            if (strSelectedTag == null)
            {
                using (var castRepository = GetRepository())
                {
                    string strCommonTagsJson = castRepository.GetCommonTagsJson();
                    if (strCommonTagsJson == null || strCommonTagsJson.Equals(string.Empty))
                    {
                        return _commonTaggedApplications;
                    }
                    var _commonTagsss3 = new DataContractJsonSerializer(typeof(CommonTags[]));
                    using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(strCommonTagsJson)))
                    {
                        var _commonTags = _commonTagsss3.ReadObject(ms) as CommonTags[];

                        if (_commonTags == null || !_commonTags.Any()) return _commonTaggedApplications;
                        _commonTaggedApplications.AddRange(_commonTags.Select(ct => GetApplications().First(_ => _.Href == ct.application.Href)));
                    }
                }
            }
            else
            {
                using (var castRepository = GetRepository())
                {
                    string strCommonTagsJson = castRepository.GetCommonTagsJson();
                    if (strCommonTagsJson == null) return _commonTaggedApplications;

                    var _commonTagsss3 = new DataContractJsonSerializer(typeof(CommonTags[]));
                    using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(strCommonTagsJson)))
                    {
                        var _commonTags = _commonTagsss3.ReadObject(ms) as CommonTags[];

                        if (_commonTags == null || !_commonTags.Any()) return _commonTaggedApplications;
                        foreach (var ct in _commonTags)
                        {
                            Application app = GetApplications().First(_ => _.Href == ct.application.Href);
                            Tagg[] tags = ct.commonTags;
                            _commonTaggedApplications.AddRange(from tag in tags select string.IsNullOrEmpty(tag.label) ? " " : tag.label into strTagLabel where strTagLabel == strSelectedTag select app);
                        }
                    }
                }
            }
            return _commonTaggedApplications;
        }

        public List<string> GetTags(string strCategory)
        { 
            List<string> _tags = new List<string>();
             
            using (var castRepository = GetRepository())
            {
                string _commonCategoriesJson = castRepository.GetCommonCategoriesJson();
                if (_commonCategoriesJson == "") return _tags;
                var _commonTagsss3 = new DataContractJsonSerializer(typeof(CommonCategoriess[]));
                using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(_commonCategoriesJson)))
                {
                    var _commonCategorys = _commonTagsss3.ReadObject(ms) as CommonCategoriess[];

                    if (_commonCategorys == null || !_commonCategorys.Any()) return _tags;
                    foreach (var _category in _commonCategorys)
                    {
                        string strLabelled = string.IsNullOrEmpty(_category.label) ? " " : _category.label;
                        if (strCategory != strLabelled) continue;
                        Tagg[] tags = _category.tags;
                        if (tags.Length > 0)
                        {
                            _tags.AddRange(tags.Select(tag => string.IsNullOrEmpty(tag.label) ? " " : tag.label));
                        }
                    }
                }
            }

            return _tags;
        }

        public List<string> GetCategories()
        {
            try
            {
                List<string> _categories = new List<string>();

                using (var castRepository = GetRepository())
                {
                    var _categoriess = castRepository.GetCommonCategories();

                    _categories.AddRange(_categoriess.Select(category => string.IsNullOrEmpty(category.Name) ? " " : category.Name));
                }

                return _categories;
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo(ex.Message);
                List<string> _categories = new List<string>();
                return _categories;
            }
        }

    }
}
