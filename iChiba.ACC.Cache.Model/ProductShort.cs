using ProtoBuf;
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Cache.Model
{
    [ProtoContract]
    public class ProductShort
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string Title { get; set; }

        [ProtoMember(3)]
        public long Price { get; set; }

        [ProtoMember(4)]
        public decimal BidOrBuy { get; set; }

        [ProtoMember(5)]
        public string SellerId { get; set; }

        [ProtoMember(6)]
        public DateTime EndTime { get; set; }

        [ProtoMember(7)]
        public IList<string> Images { get; set; }

        [ProtoMember(8)]
        public string PreviewImage { get; set; }

        [ProtoMember(9)]
        public string CategoryID { get; set; }

        [ProtoMember(10)]
        public DateTime CreatedDate { get; set; }

        [ProtoMember(11)]
        public DateTime UpdatedDate { get; set; }
    }
}
