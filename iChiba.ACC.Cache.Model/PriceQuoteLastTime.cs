using System;
using System.Collections.Generic;
using ProtoBuf;

namespace iChiba.ACC.Cache.Model
{
    [ProtoContract]
    public class PriceQuoteLastTime
    {
        [ProtoMember(1)]
        public string ProductId { get; set; }
        [ProtoMember(2)]
        public string AccountId { get; set; }
    }
}
