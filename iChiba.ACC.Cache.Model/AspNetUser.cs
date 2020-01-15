using ProtoBuf;
using System;
using System.Collections.Generic;

namespace iChiba.ACC.Cache.Cache.Model
{
    [ProtoContract]
    public  class AspNetUser
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Email { get; set; }
        [ProtoMember(3)]
        public bool EmailConfirmed { get; set; }
        [ProtoMember(4)]
        public string PhoneNumber { get; set; }
        [ProtoMember(5)]
        public bool PhoneNumberConfirmed { get; set; }
        [ProtoMember(6)]
        public string UserName { get; set; }
        [ProtoMember(7)]
        public int? OrgId { get; set; }
        [ProtoMember(8)]
        public string FullName { get; set; }
        [ProtoMember(9)]
        public List<string> Roles { get; set; }
    }
}
