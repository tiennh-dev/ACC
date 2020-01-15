using ProtoBuf;
using System;

namespace iChiba.ACC.Cache.Model
{
    [ProtoContract]
    public class BankClientInfo
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
        [ProtoMember(2)]
        public bool IsReady { get; set; }
        [ProtoMember(3)]
        public DateTime UpdatedDate { get; set; }
        [ProtoMember(4)]
        public bool CapchaRequired { get; set; }
        [ProtoMember(5)]
        public string BankType { get; set; }
        [ProtoMember(6)]
        public string ApiUrl { get; set; }
        public string Id
        {
            get
            {
                return $"{BankType}_{UserName}";
            }
        }
    }
}
