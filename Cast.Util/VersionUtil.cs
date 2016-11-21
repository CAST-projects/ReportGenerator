using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cast.Util.Version
{
    public class VersionUtil
    {
        public static Boolean isAdgVersion82Compliant(string version)
        {
            int majorVersion = extractVersionNumber(version, 0);
            if (majorVersion >= 9)
            {
                return true;
            }
            int minorVersion = extractVersionNumber(version, 1);
            return (8 == majorVersion) && (minorVersion >= 2);
        }

        public static int extractVersionNumber(string version, int idx)
        {
            String[] digits = version.Split('.');
            int[] intdigits = new int[digits.Length];
            for(int i=0; i < digits.Length; i++)
            {
                intdigits[i] = Int32.Parse(digits[i]);
            }
            return intdigits[idx];
        }
    }
}
