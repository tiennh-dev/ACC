using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface.YahooAuctions;
using iChiba.ACC.Cache.Model.YahooAuctions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement.YahooAuctions
{
    public class UserBidProductCache : BaseHashCache<UserBidProduct, string>, IUserBidProductCache
    {
        private const string KEY = "YahooAuctions-User-Bid-Product";

        public UserBidProductCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public Task<bool> HashSet(UserBidProduct model)
        {
            return redisStorage.HashSet(key, model.ProductId, model);
        }

        public Task<IList<UserBidProduct>> StringGet()
        {
            return redisStorage.StringGet<IList<UserBidProduct>>(KEY);
        }

        public Task<bool> StringSet(IList<UserBidProduct> models)
        {
            return redisStorage.StringSet(KEY, models);
        }
    }
}
