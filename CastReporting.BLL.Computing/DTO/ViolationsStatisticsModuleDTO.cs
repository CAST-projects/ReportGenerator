using CastReporting.Domain;
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

        public int? TotalViolations { get; set; }

        public int? AddedViolations { get; set; }

        public int? RemovedViolations { get; set; }

        public int? TotalCriticalViolations { get; set; }

        public int? AddedCriticalViolations { get; set; }

        public int? RemovedCriticalViolations { get; set; }
    }

    public class ViolStatMetricIdDTO
    {

        public int? Id { get; set; }

        public int? TotalViolations { get; set; }

        public int? AddedViolations { get; set; }

        public int? RemovedViolations { get; set; }

        public int? TotalCriticalViolations { get; set; }

        public int? AddedCriticalViolations { get; set; }

        public int? RemovedCriticalViolations { get; set; }
    }
}
