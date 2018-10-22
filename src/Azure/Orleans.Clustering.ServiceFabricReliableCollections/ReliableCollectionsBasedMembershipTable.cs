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
                await storage.SetAsync(tx, entry.SiloAddress, entry);
                await tx.CommitAsync();
                return true;
            }
        }

        public Task<MembershipTableData> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateIAmAlive(MembershipEntry entry)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            throw new System.NotImplementedException();
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