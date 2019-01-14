using System;
using System.Collections.Generic;
using System.Linq;
using Orleans.Runtime;

namespace Orleans.Clustering.ServiceFabricReliableCollections
{
    [Serializable]
    internal class SerializableMembershipEntry
    {
        public string SiloAddress { get; set; }
        public SiloStatus Status { get; set; }
        public int ProxyPort { get; set; }
        public string HostName { get; set; }
        public string SiloName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime IAmAliveTime { get; set; }
        public List<SerializableSuspectTime> SuspectTimes { get; set; }
        public string eTag { get; set; }

        public MembershipEntry ToMembershipEntry()
        {
            return new MembershipEntry
            {
                SiloAddress = Runtime.SiloAddress.FromParsableString(SiloAddress),
                Status = Status,
                ProxyPort = ProxyPort,
                HostName = HostName,
                SiloName = SiloName,
                StartTime = StartTime,
                IAmAliveTime = IAmAliveTime,
                SuspectTimes = SuspectTimes.Select(
                    x => Tuple.Create(Runtime.SiloAddress.FromParsableString(x.SiloAddress), x.SuspectTime)).ToList()
            };
        }
    }

    [Serializable]
    internal class SerializableSuspectTime
    {
        public string SiloAddress { get; set; }
        public DateTime SuspectTime { get; set; }
    }
}
