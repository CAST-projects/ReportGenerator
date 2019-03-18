
namespace Cast.Util.Version
{
    public class VersionUtil
    {
        private VersionUtil()
        {
            // Avoid instanciation of the class
        }
        public static bool IsAdgVersion833Compliant(string version)
        {
            int majorVersion = ExtractVersionNumber(version, 0);
            if (majorVersion >= 9)
            {
                return true;
            }
            int minorVersion = ExtractVersionNumber(version, 1);
            if ((8 == majorVersion) && (minorVersion >= 4))
            {
                return true;
            }

            int spversion = ExtractVersionNumber(version, 2);
            return (8 == majorVersion) && (3 == minorVersion) && (spversion >= 3);
        }

        public static bool IsAdgVersion82Compliant(string version)
        {
            int majorVersion = ExtractVersionNumber(version, 0);
            if (majorVersion >= 9)
            {
                return true;
            }
            int minorVersion = ExtractVersionNumber(version, 1);
            return (8 == majorVersion) && (minorVersion >= 2);
        }

        public static int ExtractVersionNumber(string version, int idx)
        {
            string[] digits = version.Split('.');
            int[] intdigits = new int[digits.Length];
            for(int i=0; i < digits.Length; i++)
            {
                intdigits[i] = int.Parse(digits[i]);
            }
            return intdigits[idx];
        }

    }
}
