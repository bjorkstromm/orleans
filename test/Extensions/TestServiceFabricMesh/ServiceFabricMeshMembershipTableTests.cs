using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Messaging;
using TestExtensions;
using Xunit;
using Orleans.Hosting;
using Microsoft.ServiceFabric.Data;
using System;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Mesh.Data.Collections;

namespace UnitTests.MembershipTests
{
    /// <summary>
    /// Tests for operation of Orleans SiloInstanceManager using ZookeeperStore - Requires access to external Zookeeper storage
    /// </summary>
    [TestCategory("Membership"), TestCategory("ServiceFabricMesh")]
    public class ServiceFabricMeshMembershipTableTests : MembershipTableTestsBase
    {
        public ServiceFabricMeshMembershipTableTests(ConnectionStringFixture fixture, TestEnvironmentFixture environment)
            : base(fixture, environment, CreateFilters())
        {
        }

        private static LoggerFilterOptions CreateFilters()
        {
            var filters = new LoggerFilterOptions();
            filters.AddFilter(typeof(ServiceFabricMeshMembershipTableTests).Name, LogLevel.Trace);
            return filters;
        }

        private readonly Lazy<IReliableStateManager> ReliableStateManager = new Lazy<IReliableStateManager>(() =>
        {
            ReliableCollectionsExtensions.UseReliableCollectionsService("TestServiceFabricMesh", ReliableCollectionMode.Volatile);
            return ReliableCollectionsExtensions.GetReliableStateManager();
        });

        protected override IMembershipTable CreateMembershipTable(ILogger logger)
        {
            var options = new ReliableCollectionsClusteringOptions();

            return new ReliableCollectionsBasedMembershipTable(
                ReliableStateManager.Value,
                loggerFactory,
                Options.Create(options),
                clusterOptions);
        }

        protected override IGatewayListProvider CreateGatewayListProvider(ILogger logger)
        {
            var options = new ReliableCollectionsGatewayOptions();

            return new ReliableCollectionsGatewayListProvider(
                ReliableStateManager.Value,
                loggerFactory,
                Options.Create(options),
                clusterOptions);
        }

        protected override Task<string> GetConnectionString()
        {
            return Task.FromResult("noop");
        }

        [SkippableFact, TestCategory("Functional")]
        public void MembershipTable_ServiceFabricMesh_Init()
        {
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_GetGateways()
        {
            await MembershipTable_GetGateways();
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_ReadAll_EmptyTable()
        {
            await MembershipTable_ReadAll_EmptyTable();
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_InsertRow()
        {
            await MembershipTable_InsertRow(extendedProtocol: false); // TODO...
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_ReadRow_Insert_Read()
        {
            await MembershipTable_ReadRow_Insert_Read(extendedProtocol: false); // TODO...
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_ReadAll_Insert_ReadAll()
        {
            await MembershipTable_ReadAll_Insert_ReadAll(extendedProtocol: false); // TODO...
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_UpdateRow()
        {
            await MembershipTable_UpdateRow(extendedProtocol: false); // TODO...
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_UpdateRowInParallel()
        {
            await MembershipTable_UpdateRowInParallel(extendedProtocol: false); // TODO...
        }

        [SkippableFact, TestCategory("Functional")]
        public async Task MembershipTable_ServiceFabricMesh_UpdateIAmAlive()
        {
            await MembershipTable_UpdateIAmAlive(extendedProtocol: false); // TODO...
        }
    }
}
