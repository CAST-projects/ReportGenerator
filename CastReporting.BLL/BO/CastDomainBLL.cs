/*
 *   Copyright (c) 2016 CAST
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
using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Repositories;
using CastReporting.Repositories.Interfaces;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

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
        /// <param name="currentSnapshot"></param>
        /// <param name="previousSnapshot"></param>
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
					var domainApps = castRepsitory.GetApplicationsByDomain(domain.Href);

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

        public class CommonTags
        {
            public Application application { get; set; }
            public Tagg[] commonTags { get; set; }
        }

        public class CommonCategoriess
        {
            public string key { get; set; }
            public string label { get; set; }
            public Tagg[] tags { get; set; }
        }

        public class Tagg
        {
            public string key { get; set; }
            public string label { get; set; }
        }

        //public List<string> GetCommonTaggedApplications()
        //{
        //    List<string> CommonTaggedApplications = new List<string>();

        //    using (var castRepository = GetRepository())
        //    {
        //        string strCommonTagsJson = castRepository.GetCommonTagsJson();
        //        if (strCommonTagsJson != null)
        //        {
        //        }
        //    }

        //    return CommonTaggedApplications;
        //}

        public List<Snapshot> GetAllSnapshots(Application[] Applications)
        {
            List<Snapshot> Snapshots = new List<Snapshot>();
            using (var castRepository = GetRepository())
            {
                for (int j = 0; j < Applications.Count(); j++)
                {
                    Application Appl = Applications[j];

                    int nbSnapshotsEachApp = Appl.Snapshots.Count();
                    if (nbSnapshotsEachApp > 0)
                    {
                        foreach (Snapshot snapshot in Appl.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                        {
                            Snapshots.Add(snapshot);
                        }
                    }
                }
            }
            return Snapshots;
        }


        public List<Application> GetCommonTaggedApplications(string strSelectedTag)
        {
            List<Application> CommonTaggedApplications = new List<Application>();
            if (strSelectedTag == null)
            {
                using (var castRepository = GetRepository())
                {
                    string strCommonTagsJson = castRepository.GetCommonTagsJson();
                    if (strCommonTagsJson != null)
                    {
                        //var CommonTags = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonTags[]>(strCommonTagsJson);
                        var CommonTagsss3 = new DataContractJsonSerializer(typeof(CommonTags[]));
                        MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(strCommonTagsJson));
                        var CommonTags = CommonTagsss3.ReadObject(ms) as CommonTags[]; 

                        if (CommonTags != null && CommonTags.Any())
                        {
                            foreach (var ct in CommonTags)
                            {
                                Application app = ct.application;
                                CommonTaggedApplications.Add(app); 
                            }
                        }
                    }
                }
            }
            else
            {
                using (var castRepository = GetRepository())
                {
                    string strCommonTagsJson = castRepository.GetCommonTagsJson();
                    if (strCommonTagsJson != null)
                    {
                        //var CommonTags = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonTags[]>(strCommonTagsJson);

                        var CommonTagsss3 = new DataContractJsonSerializer(typeof(CommonTags[]));
                        MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(strCommonTagsJson));
                        var CommonTags = CommonTagsss3.ReadObject(ms) as CommonTags[];

                        if (CommonTags != null && CommonTags.Any())
                        {
                            foreach (var ct in CommonTags)
                            {
                                Application app = ct.application;
                                Tagg[] tags = ct.commonTags;
                                foreach (Tagg tag in tags)
                                {
                                    string strTagLabel = string.IsNullOrEmpty(tag.label) ? " " : tag.label;
                                    if (strTagLabel == strSelectedTag)
                                    {
                                        CommonTaggedApplications.Add(app);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return CommonTaggedApplications;
        }

        public List<string> GetTags(string strCategory)
        { 
            List<string> Tags = new List<string>();
             
            using (var castRepository = GetRepository())
            {
                string CommonCategoriesJson = castRepository.GetCommonCategoriesJson();
                if (CommonCategoriesJson != "")
                {
                    //var CommonCategorys = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonCategoriess[]>(CommonCategoriesJson);
                    var CommonTagsss3 = new DataContractJsonSerializer(typeof(CommonCategoriess[]));
                    MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(CommonCategoriesJson));
                    var CommonCategorys = CommonTagsss3.ReadObject(ms) as CommonCategoriess[];

                    if (CommonCategorys != null && CommonCategorys.Any())
                    {
                        foreach (var Category in CommonCategorys)
                        {
                            string strKey = string.IsNullOrEmpty(Category.key) ? " " : Category.key;
                            string strLabelled = string.IsNullOrEmpty(Category.label) ? " " : Category.label;
                            if (strCategory == strLabelled)
                            {
                                Tagg[] tags = (Tagg[])Category.tags;
                                if (tags.Count() > 0)
                                {
                                    foreach (Tagg tag in tags)
                                    {
                                        string strTagLabel = string.IsNullOrEmpty(tag.label) ? " " : tag.label;
                                        Tags.Add(strTagLabel);
                                    }


                                }
                            }

                        }
                    }
                }
            }

            return Tags;
        }

        public List<string> GetCategories()
        {
            try
            {
                List<string> Categories = new List<string>();

                using (var castRepository = GetRepository())
                {
                    var Categoriess = castRepository.GetCommonCategories();

                    foreach (var Category in Categoriess)
                    {
                        string strName = string.IsNullOrEmpty(Category.Name) ? " " : Category.Name;

                        Categories.Add(strName);
                    }
                }

                return Categories;
            }
            catch ( System.Exception ex)
            {
                List<string> Categories = new List<string>();
                return Categories;
            }
        }

    }
}
