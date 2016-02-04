using System;
using System.Collections.Generic;

namespace CastReporting.Domain.Interfaces
{
    public interface ISnapshotExplorer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="moduleId"></param>
        /// <param name="snapshotId"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count);
    }
}
