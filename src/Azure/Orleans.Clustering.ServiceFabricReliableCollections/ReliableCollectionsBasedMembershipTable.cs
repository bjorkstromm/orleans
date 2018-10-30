using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Orleans.Configuration;
using Orleans.Runtime;

namespace Orleans.Hosting
{
    internal sealed class ReliableCollectionsBasedMembershipTable : IMembershipTable
    {
        public static readonly TableVersion _tableVersion = new TableVersion(0, "0");

        private readonly ReliableCollectionsClusteringOptions options;
        private readonly ClusterOptions clusterOptions;
        private readonly IReliableStateManager stateManager;
        private readonly ILogger logger;

        private IReliableDictionary<SiloAddress, MembershipEntry> membershipDictionary;
        private readonly AsyncLock asyncLock = new AsyncLock();

        public ReliableCollectionsBasedMembershipTable(
            IReliableStateManager stateManager,
            ILoggerFactory loggerFactory,
            IOptions<ReliableCollectionsClusteringOptions> options,
            IOptions<ClusterOptions> clusterOptions)
        {
            this.stateManager = stateManager;
            this.options = options.Value;
            this.clusterOptions = clusterOptions.Value;
            var loggerName = $"{typeof(ReliableCollectionsBasedMembershipTable).FullName}.{this.clusterOptions.ClusterId}";
            this.logger = loggerFactory.CreateLogger(loggerName);
        }

        public async Task DeleteMembershipTableEntries(string clusterId)
        {
            if (!clusterOptions.ClusterId.Equals(clusterId))
            {
                return;
            }

            var storage = await this.GetMembershipDictionary();
            await storage.ClearAsync();
        }

        public Task InitializeMembershipTable(bool tryInitTableVersion)
        {
            return Task.CompletedTask;
        }

        public async Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetMembershipDictionary();

                var added = await storage.TryAddAsync(tx, entry.SiloAddress, entry);

                if (added)
                {
                    await tx.CommitAsync();
                }
                
                return added;
            }
        }

        public async Task<MembershipTableData> ReadAll()
        {
            var tableData = new List<Tuple<MembershipEntry, string>>();

            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetMembershipDictionary();
                var asyncEnumerable = await storage.CreateEnumerableAsync(tx);

                using (var e = asyncEnumerable.GetAsyncEnumerator())
                {
                    while (await e.MoveNextAsync(CancellationToken.None))
                    {
                        tableData.Add(Tuple.Create(e.Current.Value, "TODO..."));
                    }
                }
            }

            return new MembershipTableData(tableData, _tableVersion);
        }

        public async Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetMembershipDictionary();

                var data = await storage.TryGetValueAsync(tx, key);

                return data.HasValue ?
                    new MembershipTableData(Tuple.Create(data.Value, "TODO..."), _tableVersion) :
                    new MembershipTableData(_tableVersion);
            }
        }

        public async Task UpdateIAmAlive(MembershipEntry entry)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetMembershipDictionary();
                var data = await storage.TryGetValueAsync(tx, entry.SiloAddress);
                
                if (!data.HasValue)
                {
                    return;
                }

                var newEntry = new MembershipEntry();
                newEntry.Update(data.Value);
                newEntry.IAmAliveTime = entry.IAmAliveTime;

                await storage.SetAsync(tx, entry.SiloAddress, newEntry);

                await tx.CommitAsync();
            }
        }

        public async Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetMembershipDictionary();
                var data = await storage.TryGetValueAsync(tx, entry.SiloAddress);

                if (!data.HasValue)
                {
                    return false;
                }

                var newEntry = new MembershipEntry();
                newEntry.Update(entry);

                await storage.SetAsync(tx, entry.SiloAddress, newEntry);

                await tx.CommitAsync();
                return true;
            }
        }

        private ValueTask<IReliableDictionary<SiloAddress, MembershipEntry>> GetMembershipDictionary()
        {
            if (this.membershipDictionary != null) return new ValueTask<IReliableDictionary<SiloAddress, MembershipEntry>>(this.membershipDictionary);

            return Async();

            async ValueTask<IReliableDictionary<SiloAddress, MembershipEntry>> Async()
            {
                using (await asyncLock.LockAsync())
                {
                    if (this.membershipDictionary == null)
                    {
                        this.membershipDictionary = await this.stateManager.GetOrAddAsync<IReliableDictionary<SiloAddress, MembershipEntry>>(this.options.StateName ?? clusterOptions.ClusterId);
                    }
                    return this.membershipDictionary;
                }
            }
        }
    }
}