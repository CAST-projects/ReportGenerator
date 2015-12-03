using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public class ViolationSummaryModuleDTO
    {
        public string ModuleName { get; set; }

        public List<ViolationSummaryDTO> Stats { get; set; }

        public ViolationSummaryDTO this[Constants.BusinessCriteria index]    
        {
            get 
            {
                return Stats.FirstOrDefault(_ => _.BusinessCriteria.Equals(index));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ViolationSummaryDTO
    {
 
        public Constants.BusinessCriteria BusinessCriteria { get; set; }

        public Int32? Total {get;set;}

        public Int32? Added { get; set; }

        public Int32? Removed { get; set; }
    }
}
