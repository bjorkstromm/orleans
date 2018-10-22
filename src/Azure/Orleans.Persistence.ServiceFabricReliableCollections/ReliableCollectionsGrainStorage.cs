using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Orleans.Configuration;
using Orleans.Configuration.Overrides;
using Orleans.Runtime;
using Orleans.Serialization;
using Orleans.Storage;

namespace Orleans.Hosting
{
    public class ReliableCollectionsGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly ReliableCollectionsStorageOptions options;
        private readonly ClusterOptions clusterOptions;
        private readonly string name;
        private readonly IReliableStateManager stateManager;
        private readonly SerializationManager serializationManager;
        private readonly IGrainFactory grainFactory;
        private readonly ITypeResolver typeResolver;
        private readonly ILogger logger;

        private IReliableDictionary<string, byte[]> stateDictionary;
        private readonly AsyncLock asyncLock = new AsyncLock();

        /// <summary> Default constructor </summary>
        public ReliableCollectionsGrainStorage(string name,
            IReliableStateManager stateManager,
            ReliableCollectionsStorageOptions options,
            IOptions<ClusterOptions> clusterOptions,
            SerializationManager serializationManager,
            IGrainFactory grainFactory,
            ITypeResolver typeResolver,
            ILoggerFactory loggerFactory)
        {
            this.stateManager = stateManager;
            this.options = options;
            this.clusterOptions = clusterOptions.Value;
            this.name = name;
            this.serializationManager = serializationManager;
            this.grainFactory = grainFactory;
            this.typeResolver = typeResolver;
            var loggerName = $"{typeof(ReliableCollectionsGrainStorage).FullName}.{name}";
            this.logger = loggerFactory.CreateLogger(loggerName);
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var keyString = grainReference.GrainId.ToParsableString();
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetStorage();
                await storage.TryRemoveAsync(tx, keyString);
                await tx.CommitAsync();
            }
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var keyString = grainReference.GrainId.ToParsableString();
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetStorage();
                var result = await storage.TryGetValueAsync(tx, keyString);
                if (result.HasValue)
                {
                    grainState.State = this.serializationManager.DeserializeFromByteArray<object>(result.Value);
                }
                await tx.CommitAsync();
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var keyString = grainReference.GrainId.ToParsableString();
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storage = await this.GetStorage();
                var bytes = this.serializationManager.SerializeToByteArray(grainState.State);
                await storage.SetAsync(tx, keyString, bytes);
                await tx.CommitAsync();
            }
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<ReliableCollectionsGrainStorage>(this.name), this.options.InitStage, Init, Close);
        }

        private Task Init(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Task Close(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private ValueTask<IReliableDictionary<string, byte[]>> GetStorage()
        {
            if (this.stateDictionary != null) return new ValueTask<IReliableDictionary<string, byte[]>>(this.stateDictionary);

            return Async();

            async ValueTask<IReliableDictionary<string, byte[]>> Async()
            {
                using (await asyncLock.LockAsync())
                {
                    if (this.stateDictionary == null)
                    {
                        this.stateDictionary = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, byte[]>>(this.options.StateName ?? this.name);
                    }
                    return this.stateDictionary;
                }
            }
        }
    }

    public static class ReliableCollectionsGrainStorageFactory
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            IOptionsSnapshot<ReliableCollectionsStorageOptions> optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<ReliableCollectionsStorageOptions>>();
            IOptions<ClusterOptions> clusterOptions = services.GetProviderClusterOptions(name);
            return ActivatorUtilities.CreateInstance<ReliableCollectionsGrainStorage>(services, name, optionsSnapshot.Get(name), clusterOptions);
        }
    }
}