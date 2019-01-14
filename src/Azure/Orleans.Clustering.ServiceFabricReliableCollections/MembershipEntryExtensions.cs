using System;
using System.Collections.Generic;
using System.Linq;

namespace Orleans.Clustering.ServiceFabricReliableCollections
{
    internal static class MembershipEntryExtensions
    {
        public static SerializableMembershipEntry ToSerializable(this MembershipEntry entry, string eTag = null)
        {
            return new SerializableMembershipEntry
            {
                eTag = string.IsNullOrEmpty(eTag) ? Guid.NewGuid().ToString("n").Substring(0, 8) : eTag,
                HostName = entry.HostName,
                IAmAliveTime = entry.IAmAliveTime,
                ProxyPort = entry.ProxyPort,
                SiloAddress = entry.SiloAddress.ToParsableString(),
                SiloName = entry.SiloName,
                StartTime = entry.StartTime,
                Status = entry.Status,
                SuspectTimes = entry.SuspectTimes?.Select(x => new SerializableSuspectTime
                {
                    SiloAddress = x.Item1.ToParsableString(),
                    SuspectTime = x.Item2
                }).ToList() ?? new List<SerializableSuspectTime>(),
            };
        }
    }
}
