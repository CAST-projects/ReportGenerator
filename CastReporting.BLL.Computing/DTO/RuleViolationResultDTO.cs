using CastReporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastReporting.BLL.Computing
{
    public class RuleViolationResultDTO
    {
        public RuleDetailsDTO Rule { get; set; }

        public string TechnicalCriteraiName { get; set; }

        public Int32? TotalChecks { get; set; }

        public Int32? TotalFailed { get; set; }

        public Double? Grade { get; set; }
        
        public class Comparer : IEqualityComparer<RuleViolationResultDTO> {
			#region IEqualityComparer implementation
			public bool Equals(RuleViolationResultDTO x, RuleViolationResultDTO y)
			{
				if (x == null && y == null)
					return true;
				if (x == null || y == null)
					return false;
				int xk = (x.Rule != null) ? x.Rule.Key : 0;
				int yk = (y.Rule != null) ? y.Rule.Key : 0;
				return (xk != 0 && xk == yk);
			}
			public int GetHashCode(RuleViolationResultDTO obj)
			{
				return (obj == null || obj.Rule == null) ? 0 : obj.Rule.Key;
			}
			#endregion
        }
    }

    public class RuleVariationResultDTO
    {
        public RuleDetailsDTO Rule { get; set; }

        public Int32? CurrentNbViolations { get; set; }

        public Int32? PreviousNbViolations { get; set; }

        public Double? Evolution { get; set; }

        public Double? Grade { get; set; }

        public Double? GradeEvolution { get; set; }
    
    }

    public class RuleViolationsVariationResultDTO
    {
        public RuleDetailsDTO Rule { get; set; }

        public Int32? CurrentNbViolations { get; set; }

        public Int32? PreviousNbViolations { get; set; }

        public Double? Variation { get; set; }

        public Double? Ratio { get; set; }

    }


    public class RuleDetailsDTO
    {
        public Int32 Key { get; set; }

        public string Name { get; set; }
       
        public Int32 CompoundedWeight { get; set; }

        public bool Critical { get; set; }

       

    }


    public class TechnicalCriteriaResultDTO
    {
        public Int32 Key { get; set; }

        public string Name { get; set; }

        public Double? Grade { get; set; }

        public Int32? TotalChecks { get; set; }

        public Int32? TotalFailed { get; set; }
       

    }
}
