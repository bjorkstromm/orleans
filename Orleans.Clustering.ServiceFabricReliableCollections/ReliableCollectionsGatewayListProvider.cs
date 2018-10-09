using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Messaging;

namespace Orleans.Hosting
{
    internal sealed class ReliableCollectionsGatewayListProvider : IGatewayListProvider
    {
        public TimeSpan MaxStaleness => throw new NotImplementedException();

        public bool IsUpdatable => throw new NotImplementedException();

        public Task<IList<Uri>> GetGateways()
        {
            throw new NotImplementedException();
        }

        public Task InitializeGatewayListProvider()
        {
            throw new NotImplementedException();
        }
    }
}