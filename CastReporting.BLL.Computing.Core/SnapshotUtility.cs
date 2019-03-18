using CastReporting.Domain;
using System;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public static class SnapshotUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static string GetSnapshotVersionNumber(Snapshot snapshot)
        {
            return snapshot?.Annotation?.Version;
        }

        public static string GetSnapshotNameVersion(Snapshot snapshot)
        {
            return snapshot?.Annotation?.Name + " - " + snapshot?.Annotation?.Version;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static DateTime? GetSnapshotDate(Snapshot snapshot)
        {
			return snapshot?.Annotation?.Date?.DateSnapShot;
        }
    }
}
