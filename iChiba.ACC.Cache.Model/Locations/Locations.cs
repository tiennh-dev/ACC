using ProtoBuf;
using System;

namespace iChiba.ACC.Cache.Cache.Model
{
    [ProtoContract]
    public class Locations
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Code { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }

        [ProtoMember(4)]
        public string Type { get; set; }

        [ProtoMember(5)]
        public int? Parent { get; set; }
    }
}
