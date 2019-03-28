using CastReporting.Domain;
using System;
using System.Linq;

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

        public static bool IsLatestSnapshot(Application application, Snapshot snapshot)
        {
            int nbSnapshot = application.Snapshots.Count();
            if (nbSnapshot <= 0) return false;
            Snapshot latest = nbSnapshot == 1 ? application.Snapshots.FirstOrDefault() : application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).FirstOrDefault();
            return latest?.Equals(snapshot) ?? false;
        }

    }
}
