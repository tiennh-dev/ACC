using ProtoBuf;
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Cache.Model.YahooAuctions
{
    [ProtoContract]
    public class ProductBidClientInfo
    {
        [ProtoMember(1)]
        public string YAUserName { get; set; }

        [ProtoMember(2)]
        public bool IsReady { get; set; }

        [ProtoMember(3)]
        public int TotalPageRunning { get; set; }

        [ProtoMember(4)]
        public DateTime UpdatedDate { get; set; }

        [ProtoMember(5)]
        public string ApiUrl { get; set; }

        [ProtoMember(6)]
        public IList<string> AppIds { get; set; }

        [ProtoMember(7)]
        public bool IsAllowBid { get; set; }
    }
}
