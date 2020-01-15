using System;
using System.Collections.Generic;
using ProtoBuf;

namespace iChiba.ACC.Cache.Model
{
    [ProtoContract]
    public class FavoriteSeller
    {
        [ProtoMember(1)]
        public string SellerId { get; set; }
        [ProtoMember(2)]
        public int Type { get; set; }
        [ProtoMember(3)]
        public string Description { get; set; }
    }
}
