using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Messaging;
using TestExtensions;
using Xunit;
using Orleans.Hosting;
using Microsoft.ServiceFabric.Data;
using System;
using ServiceFabric.Mocks;
using Microsoft.Extensions.Options;

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

        private readonly Lazy<IReliableStateManager> ReliableStateManager = new Lazy<IReliableStateManager>(() => new MockReliableStateManager());

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
            return new ReliableCollectionsGatewayListProvider();
        }

        protected override Task<string> GetConnectionString()
        {
            return Task.FromResult<string>(null);
        }

        [SkippableFact]
        public void MembershipTable_ServiceFabricMesh_Init()
        {
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_GetGateways()
        {
            await MembershipTable_GetGateways();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_ReadAll_EmptyTable()
        {
            await MembershipTable_ReadAll_EmptyTable();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_InsertRow()
        {
            await MembershipTable_InsertRow();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_ReadRow_Insert_Read()
        {
            await MembershipTable_ReadRow_Insert_Read();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_ReadAll_Insert_ReadAll()
        {
            await MembershipTable_ReadAll_Insert_ReadAll();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_UpdateRow()
        {
            await MembershipTable_UpdateRow();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_UpdateRowInParallel()
        {
            await MembershipTable_UpdateRowInParallel();
        }

        [SkippableFact]
        public async Task MembershipTable_ServiceFabricMesh_UpdateIAmAlive()
        {
            await MembershipTable_UpdateIAmAlive();
        }
    }
}
