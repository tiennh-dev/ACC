using System;
using System.Collections.Generic;
using ProtoBuf;

namespace iChiba.Cache.Model
{
    [ProtoContract]
    public class FavoriteProduct
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Title { get; set; }
        [ProtoMember(3)]
        public string Link { get; set; }
        [ProtoMember(4)]
        public string Image { get; set; }
        [ProtoMember(5)]
        public DateTime Date { get; set; }
        [ProtoMember(6)]
        public string AccountId { get; set; }
        [ProtoMember(7)]
        public DateTime EndTime { get; set; }
        [ProtoMember(8)]
        public decimal Price { get; set; }
        [ProtoMember(9)]
        public decimal BidOrBuy { get; set; }
        [ProtoMember(10)]
        public int Type { get; set; }
        [ProtoMember(11)]
        public string Seller { get; set; }
    }
}
