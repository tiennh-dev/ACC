using iChiba.ACC.Cache.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface IBidLastTimeCache : IBaseHashCache<BidLastTime, string>
    {
        Task<bool> CheckProductExistingByUserId(string userId);
        Task<bool> CheckProductExistingByUserId(string userId, string productId);
        Task<bool> HashSet(BidLastTime model);
    }
}
