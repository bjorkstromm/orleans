using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Orleans.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Providers;

namespace Orleans.Hosting
{
    public static class ReliableCollectionsSiloBuilderExtensions
    {
        /// <summary>
        /// Configure silo to use Service Fabric Standalone Reliable Collections as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddReliableCollectionsGrainStorageAsDefault(this ISiloHostBuilder builder, Action<ReliableCollectionsStorageOptions> configureOptions)
        {
            return builder.AddReliableCollectionsGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Service Fabric Standalone Reliable Collections for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddReliableCollectionsGrainStorage(this ISiloHostBuilder builder, string name, Action<ReliableCollectionsStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddReliableCollectionsGrainStorage(name, ob => ob.Configure(configureOptions)));
        }

        /// <summary>
        /// Configure silo to use Service Fabric Standalone Reliable Collections as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddReliableCollectionsGrainStorageAsDefault(this ISiloHostBuilder builder, Action<OptionsBuilder<ReliableCollectionsStorageOptions>> configureOptions = null)
        {
            return builder.AddReliableCollectionsGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use Service Fabric Standalone Reliable Collections for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddReliableCollectionsGrainStorage(this ISiloHostBuilder builder, string name, Action<OptionsBuilder<ReliableCollectionsStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddReliableCollectionsGrainStorage(name, configureOptions));
        }

        internal static IServiceCollection AddReliableCollectionsGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<ReliableCollectionsStorageOptions>> configureOptions = null)
        {
            configureOptions?.Invoke(services.AddOptions<ReliableCollectionsStorageOptions>(name));
            services.AddTransient<IConfigurationValidator>(sp => new ReliableCollectionsGrainStorageOptionsValidator(sp.GetService<IOptionsSnapshot<ReliableCollectionsStorageOptions>>().Get(name), name));
            services.ConfigureNamedOptionForLogging<ReliableCollectionsStorageOptions>(name);
            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage>(name, ReliableCollectionsGrainStorageFactory.Create)
                           .AddSingletonNamedService<ILifecycleParticipant<ISiloLifecycle>>(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }
    }
}
