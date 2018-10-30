using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Orleans.Configuration;
using Orleans.Messaging;
using Orleans.Runtime;

namespace Orleans.Hosting
{
    internal sealed class ReliableCollectionsGatewayListProvider : IGatewayListProvider
    {
        private readonly ReliableCollectionsGatewayOptions options;
        private readonly ClusterOptions clusterOptions;
        private readonly IReliableStateManager stateManager;
        private readonly ILogger logger;

        private IReliableDictionary<SiloAddress, MembershipEntry> membershipDictionary;
        private readonly AsyncLock asyncLock = new AsyncLock();

        public TimeSpan MaxStaleness => TimeSpan.FromMinutes(1); // TODO...

        public bool IsUpdatable => true;

        public ReliableCollectionsGatewayListProvider(
            IReliableStateManager stateManager,
            ILoggerFactory loggerFactory,
            IOptions<ReliableCollectionsGatewayOptions> options,
            IOptions<ClusterOptions> clusterOptions)
        {
            this.stateManager = stateManager;
            this.options = options.Value;
            this.clusterOptions = clusterOptions.Value;
            var loggerName = $"{typeof(ReliableCollectionsBasedMembershipTable).FullName}.{this.clusterOptions.ClusterId}";
            this.logger = loggerFactory.CreateLogger(loggerName);
        }

        public async Task<IList<Uri>> GetGateways()
        {
            var gateways = new List<Uri>();

            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetMembershipDictionary();
                var asyncEnumerable = await storage.CreateEnumerableAsync(tx);

                using (var e = asyncEnumerable.GetAsyncEnumerator())
                {
                    while (await e.MoveNextAsync(CancellationToken.None))
                    {
                        gateways.Add(e.Current.Key.ToGatewayUri());
                    }
                }
            }

            return gateways;
        }

        public Task InitializeGatewayListProvider()
        {
            return Task.CompletedTask;
        }

        // TODO...
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