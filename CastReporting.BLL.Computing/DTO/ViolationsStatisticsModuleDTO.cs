using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public class ViolationsStatisticsModuleDTO
    {
        public string ModuleName { get; set; }

        public List<ViolationsStatisticsDTO> Stats { get; set; }

        public ViolationsStatisticsDTO this[Constants.BusinessCriteria idx]
        {
            get
            {
                return Stats.FirstOrDefault(_ => _.BusinessCriteria.Equals(idx));
            }
        }
    }

    public class ViolationsStatisticsDTO
    {

        public Constants.BusinessCriteria BusinessCriteria { get; set; }

        public Int32? TotalViolations { get; set; }

        public Int32? AddedViolations { get; set; }

        public Int32? RemovedViolations { get; set; }

        public Int32? TotalCriticalViolations { get; set; }

        public Int32? AddedCriticalViolations { get; set; }

        public Int32? RemovedCriticalViolations { get; set; }
    }

}
