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
    }
}
