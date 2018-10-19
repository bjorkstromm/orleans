using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Messaging;
using Orleans.Configuration;

namespace Orleans.Hosting
{
    public static class ReliableCollectionsClusteringExtensions
    {
        /// <summary>
        /// Configures the silo to use Service Fabric Standalone Reliable Collections for clustering.
        /// </summary>
        /// <param name="builder">
        /// The silo builder.
        /// </param>
        /// <param name="configureOptions">
        /// The configuration delegate.
        /// </param>
        /// <returns>
        /// The provided <see cref="ISiloHostBuilder"/>.
        /// </returns>
        public static ISiloHostBuilder UseServiceFabricReliableCollectionsClustering(
            this ISiloHostBuilder builder,
            Action<ReliableCollectionsClusteringOptions> configureOptions)
        {
            return builder.ConfigureServices(
                services =>
                {
                    if (configureOptions != null)
                    {
                        services.Configure(configureOptions);
                    }

                    services.AddSingleton<IMembershipTable, ReliableCollectionsBasedMembershipTable>()
                    .ConfigureFormatter<ReliableCollectionsClusteringOptions>();
                });
        }

        /// <summary>
        /// Configures the silo to use Service Fabric Standalone Reliable Collections for clustering.
        /// </summary>
        /// <param name="builder">
        /// The silo builder.
        /// </param>
        /// <param name="configureOptions">
        /// The configuration delegate.
        /// </param>
        /// <returns>
        /// The provided <see cref="ISiloHostBuilder"/>.
        /// </returns>
        public static ISiloHostBuilder UseServiceFabricReliableCollectionsClustering(
            this ISiloHostBuilder builder,
            Action<OptionsBuilder<ReliableCollectionsClusteringOptions>> configureOptions)
        {
            return builder.ConfigureServices(
                services =>
                {
                    configureOptions?.Invoke(services.AddOptions<ReliableCollectionsClusteringOptions>());
                    services.AddSingleton<IMembershipTable, ReliableCollectionsBasedMembershipTable>()
                    .ConfigureFormatter<ReliableCollectionsClusteringOptions>();
                });
        }

        /// <summary>
        /// Configures the client to use Service Fabric Standalone Reliable Collections for clustering.
        /// </summary>
        /// <param name="builder">
        /// The client builder.
        /// </param>
        /// <param name="configureOptions">
        /// The configuration delegate.
        /// </param>
        /// <returns>
        /// The provided <see cref="IClientBuilder"/>.
        /// </returns>
        public static IClientBuilder UseServiceFabricReliableCollectionsClustering(
            this IClientBuilder builder,
            Action<ReliableCollectionsGatewayOptions> configureOptions)
        {
            return builder.ConfigureServices(
                services =>
                {
                    if (configureOptions != null)
                    {
                        services.Configure(configureOptions);
                    }

                    services.AddSingleton<IGatewayListProvider, ReliableCollectionsGatewayListProvider>()
                    .ConfigureFormatter<ReliableCollectionsGatewayOptions>();
                });
        }

        /// <summary>
        /// Configures the client to use Service Fabric Standalone Reliable Collections for clustering.
        /// </summary>
        /// <param name="builder">
        /// The client builder.
        /// </param>
        /// <param name="configureOptions">
        /// The configuration delegate.
        /// </param>
        /// <returns>
        /// The provided <see cref="IClientBuilder"/>.
        /// </returns>
        public static IClientBuilder UseServiceFabricReliableCollectionsClustering(
            this IClientBuilder builder,
            Action<OptionsBuilder<ReliableCollectionsGatewayOptions>> configureOptions)
        {
            return builder.ConfigureServices(
                services =>
                {
                    configureOptions?.Invoke(services.AddOptions<ReliableCollectionsGatewayOptions>());
                    services.AddSingleton<IGatewayListProvider, ReliableCollectionsGatewayListProvider>()
                    .ConfigureFormatter<ReliableCollectionsGatewayOptions>();
                });
        }
    }
}
