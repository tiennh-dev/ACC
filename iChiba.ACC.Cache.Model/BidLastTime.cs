using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iChiba.ACC.Cache.Model
{
    [ProtoContract]
    public class BidLastTime
    {
        [ProtoMember(1)]
        public string ProductId { get; set; }

        [ProtoMember(2)]
        public DateTime BidTimeSchedule { get; set; }

        [ProtoMember(3)]
        public bool IsProcessed { get; set; }

        [ProtoMember(4)]
        public IList<BidLastTimeInfo> Infos { get; set; }

        [ProtoMember(5)]
        public string Title { get; set; }

        [ProtoMember(6)]
        public string SellerId { get; set; }

        [ProtoMember(7)]
        public DateTime EndDate { get; set; }

        [ProtoMember(8)]
        public string PreviewImage { get; set; }

        public IList<BidLastTimeInfo> InfosSorted
        {
            get
            {
                if (Infos == null || Infos.Count == 0)
                {
                    return Infos;
                }

                return Infos.OrderBy(m => m.CreatedDate)
                    .ToList();
            }
        }

        public BidLastTime()
        {
            Infos = new List<BidLastTimeInfo>();
            IsProcessed = false;
        }

        public void Register(string userId, long price, DateTime createdDate)
        {
            if (IsProcessed)
            {
                return;
            }

            var model = Infos?.FirstOrDefault(m => m.UserId.Equals(userId));

            if (model == null)
            {
                model = new BidLastTimeInfo()
                {
                    UserId = userId
                };

                Infos.Add(model);
            }

            model.Price = price;
            model.CreatedDate = createdDate;
            model.RegisteredDate = DateTime.UtcNow;
        }

        public void Cancel(string userId)
        {
            Infos = Infos?.Where(m => !m.UserId.Equals(userId))
                .ToList();
        }

        public BidLastTimeInfo GetByUserId(string userId)
        {
            return Infos?.FirstOrDefault(m => m.UserId.Equals(userId));
        }
    }

    [ProtoContract]
    public class BidLastTimeInfo
    {
        public enum State
        {
            New = 0,
            Success = 1,
            Fail = 2
        }

        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string UserId { get; set; }

        [ProtoMember(4)]
        public long Price { get; set; }

        [ProtoMember(5)]
        public State Status { get; set; }

        [ProtoMember(6)]
        public DateTime CreatedDate { get; set; }

        [ProtoMember(7)]
        public DateTime RegisteredDate { get; set; }

        [ProtoMember(8)]
        public DateTime? ProcessedDate { get; set; }

        public BidLastTimeInfo()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.UtcNow;
            Status = State.New;
            ProcessedDate = null;
        }
    }
}
