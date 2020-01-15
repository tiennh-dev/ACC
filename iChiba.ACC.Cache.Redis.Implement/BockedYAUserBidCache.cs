using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface.YahooAuctions;
using iChiba.ACC.Cache.Model.YahooAuctions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement.YahooAuctions
{
    public class BockedYAUserBidCache : BaseHashCache<BockedYAUserBid, string>, IBockedYAUserBidCache
    {
        private const string KEY = "YahooAuctions-Blocked-YAUser-Seller";

        public BockedYAUserBidCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public Task<bool> HashSet(BockedYAUserBid model)
        {
            return redisStorage.HashSet(key, model.SellerId, model);
        }
    }
}
