using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface;
using iChiba.ACC.Cache.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class BidLastTimeCache : BaseHashCache<BidLastTime, string>, IBidLastTimeCache
    {
        private const string KEY = "YahooAuctions-Bid-Last-Time";

        public BidLastTimeCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public Task<bool> HashSet(BidLastTime model)
        {
            return redisStorage.HashSet(key, model.ProductId, model);
        }

        public async Task<bool> CheckProductExistingByUserId(string userId)
        {
            IList<BidLastTime> allProducts = await GetAll();

            if (allProducts == null)
            {
                return false;
            }

            var isExisting = allProducts.Any(m => !m.IsProcessed
                    && m.Infos.Any(x => x.UserId.Equals(userId)
                        && x.Status == BidLastTimeInfo.State.New));

            return isExisting;
        }

        public async Task<bool> CheckProductExistingByUserId(string userId, string productId)
        {
            var byProduct = await GetById(productId);

            if (byProduct == null)
            {
                return false;
            }

            if (byProduct.IsProcessed)
            {
                return false;
            }

            var isExisting = byProduct.Infos.Any(x => x.UserId.Equals(userId)
                    && x.Status == BidLastTimeInfo.State.New);

            return isExisting;
        }
    }
}
