using System.Threading.Tasks;
using Orleans.Runtime;

namespace Orleans.Hosting
{
    internal sealed class ReliableCollectionsBasedMembershipTable : IMembershipTable
    {
        public Task DeleteMembershipTableEntries(string clusterId)
        {
            throw new System.NotImplementedException();
        }

        public Task InitializeMembershipTable(bool tryInitTableVersion)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
        {
            throw new System.NotImplementedException();
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
    }
}