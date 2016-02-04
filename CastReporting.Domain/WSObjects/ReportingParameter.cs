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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastReporting.Domain
{
     [Serializable]
    public class ReportingParameter
    {
         /// <summary>
         /// 
         /// </summary>
         public Int32 ApplicationSizeLimitSupSmall { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public Int32 ApplicationSizeLimitSupMedium { get; set; }


         /// <summary>
         /// 
         /// </summary>
         public Int32 ApplicationSizeLimitSupLarge { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public Double ApplicationQualityVeryLow { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public Double ApplicationQualityLow { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public Double ApplicationQualityMedium { get; set; }


         /// <summary>
         /// 
         /// </summary>
         public Double ApplicationQualityGood { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public Int32 NbResultDefault { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public string GeneratedFilePath { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public string TemplatePath { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public string CultureName { get; set; }

         /// <summary>
         /// 
         /// </summary>
         public ReportingParameter()
         {
             ApplicationSizeLimitSupSmall=200000;
             ApplicationSizeLimitSupMedium=500000;
             ApplicationSizeLimitSupLarge=1000000;

             ApplicationQualityVeryLow=2;
             ApplicationQualityLow=2.8;
             ApplicationQualityMedium=3.2;
             ApplicationQualityGood=3.5;

             NbResultDefault = 5;
         }
    }
}
