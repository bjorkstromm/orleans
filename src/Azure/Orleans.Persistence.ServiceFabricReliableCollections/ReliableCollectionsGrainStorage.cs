using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly SerializationManager serializationManager;
        private readonly IGrainFactory grainFactory;
        private readonly ITypeResolver typeResolver;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

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
            this.options = options;
            this.clusterOptions = clusterOptions.Value;
            this.name = name;
            this.serializationManager = serializationManager;
            this.grainFactory = grainFactory;
            this.typeResolver = typeResolver;
            this.loggerFactory = loggerFactory;
            var loggerName = $"{typeof(ReliableCollectionsGrainStorage).FullName}.{name}";
            this.logger = this.loggerFactory.CreateLogger(loggerName);
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new System.NotImplementedException();
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new System.NotImplementedException();
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