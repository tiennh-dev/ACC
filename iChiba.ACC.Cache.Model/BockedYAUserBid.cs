using ProtoBuf;
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Cache.Model.YahooAuctions
{
    [ProtoContract]
    public class BockedYAUserBid
    {
        [ProtoMember(1)]
        public string SellerId { get; set; }

        [ProtoMember(2)]
        public IList<YAUserBidBlockedInfo> YAUsers { get; set; }
    }

    [ProtoContract]
    public class YAUserBidBlockedInfo
    {
        [ProtoMember(1)]
        public string UserName { get; set; }

        [ProtoMember(2)]
        public DateTime BlockedDate { get; set; }
    }
}
