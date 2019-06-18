using System;

namespace Cast.Util.Date
{
    public class DateUtil
    {
        private DateUtil()
        {
            // Avoid instanciation of the class
        }
        public static int GetQuarter(DateTime dt)
        {
            switch (dt.Month)
            {
                case 1:
                case 2:
                case 3:
                    return 1;
                case 4:
                case 5:
                case 6:
                    return 2;
                case 7:
                case 8:
                case 9:
                    return 3;
                case 10:
                case 11:
                case 12:
                    return 4;
                default:
                    return 0;
            }
        }

        public static int GetYear(DateTime dt)
        {
            return dt.Year;
        }

        public static int GetPreviousQuarter(DateTime dt)
        {
            int currentQuarter = GetQuarter(dt);
            int previousQuarter = currentQuarter == 1 ? 4 : currentQuarter - 1;

            return previousQuarter;
        }

        public static int GetPreviousQuarterYear(DateTime dt)
        {
            int currentQuarter = GetQuarter(dt);
            int currentYear = dt.Year;
            int previousYear = currentQuarter == 1 ? currentYear - 1 : currentYear;
            return previousYear;
        }

        public static int GetPreviousQuarter(int currentQuater)
        {
            return currentQuater == 1 ? 4 : currentQuater - 1;
        }

        public static int GetPreviousQuarterYear(int currentQuater, int currentYear)
        {
            return currentQuater == 1 ? currentYear - 1 : currentYear;
        }

    }
}
