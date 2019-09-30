
namespace Cast.Util.Version
{
    public class VersionUtil
    {
        private VersionUtil()
        {
            // Avoid instanciation of the class
        }

        private static bool IsVersionCompatible(string targetVersion, string serviceVersion)
        {
            if ("X.X.X-XXX".Equals(serviceVersion)) return false;
            return new System.Version(serviceVersion).CompareTo(new System.Version(targetVersion)) >= 0;
        }

        public static bool IsAdgVersion833Compliant(string version)
        {
            return IsVersionCompatible("8.3.3", version);
        }

        public static bool IsAdgVersion82Compliant(string version)
        {
            return IsVersionCompatible("8.2.0", version);
        }

        public static bool Is111Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.11.0.000", serviceVersion);
        }

        public static bool Is112Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.12.0.000", serviceVersion);
        }

        public static bool Is19Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.9.0.000", serviceVersion);
        }

        public static bool Is18Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.8.0.000", serviceVersion);
        }

        public static bool Is17Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.7.0.000", serviceVersion);
        }

    }
}
