using System.Collections.Generic;

namespace CastReporting.BLL.Computing
{
    public class RuleViolationResultDTO
    {
        public RuleDetailsDTO Rule { get; set; }

        public string TechnicalCriteraiName { get; set; }

        public int? TotalChecks { get; set; }

        public int? TotalFailed { get; set; }

        public double? Grade { get; set; }
        
        public class Comparer : IEqualityComparer<RuleViolationResultDTO> {
			#region IEqualityComparer implementation
			public bool Equals(RuleViolationResultDTO x, RuleViolationResultDTO y)
			{
				if (x == null && y == null)
					return true;
				if (x == null || y == null)
					return false;
				int xk = x.Rule?.Key ?? 0;
				int yk = y.Rule?.Key ?? 0;
				return xk != 0 && xk == yk;
			}
			public int GetHashCode(RuleViolationResultDTO obj)
			{
				return obj.Rule?.Key ?? 0;
			}
			#endregion
        }
    }

    public class RuleVariationResultDTO
    {
        public RuleDetailsDTO Rule { get; set; }

        public int? CurrentNbViolations { get; set; }

        public int? PreviousNbViolations { get; set; }

        public double? Evolution { get; set; }

        public double? Grade { get; set; }

        public double? GradeEvolution { get; set; }
    
    }

    public class RuleViolationsVariationResultDTO
    {
        public RuleDetailsDTO Rule { get; set; }

        public int? CurrentNbViolations { get; set; }

        public int? PreviousNbViolations { get; set; }

        public double? Variation { get; set; }

        public double? Ratio { get; set; }

    }


    public class RuleDetailsDTO
    {
        public int Key { get; set; }

        public string Name { get; set; }
       
        public int CompoundedWeight { get; set; }

        public bool Critical { get; set; }

    }


    public class TechnicalCriteriaResultDTO
    {
        public int Key { get; set; }

        public string Name { get; set; }

        public double? Grade { get; set; }

        public int? TotalChecks { get; set; }

        public int? TotalFailed { get; set; }
       
    }
}
