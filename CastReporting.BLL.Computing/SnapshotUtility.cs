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
        public static String GetSnapshotVersionNumber(Snapshot snapshot)
        {
            if (snapshot != null  && snapshot.Annotation != null && snapshot.Annotation.Version != null)
                return snapshot.Annotation.Version;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static DateTime? GetSnapshotDate(Snapshot snapshot)
        {
			return (snapshot != null  && snapshot.Annotation != null && snapshot.Annotation.Date != null)
				? snapshot.Annotation.Date.DateSnapShot
				: null;
        }
    }
}
