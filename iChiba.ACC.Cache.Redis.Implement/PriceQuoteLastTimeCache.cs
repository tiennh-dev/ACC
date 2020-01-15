using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface;
using iChiba.ACC.Cache.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class PriceQuoteLastTimeCache : BaseHashCache<PriceQuoteLastTime, string>, IPriceQuoteLastTimeCache
    {
        private const string KEY = "Price-Quote-Last-Time";

        public PriceQuoteLastTimeCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public async Task<bool> HashSet(string accountId, IList<PriceQuoteLastTime> model)
        {
            return await redisStorage.HashSet(KEY,accountId, model);
        }

        public async Task<IList<PriceQuoteLastTime>> GetByAcount(string accountId)
        {
            return await redisStorage.HashGet<IList<PriceQuoteLastTime>>(KEY,accountId);
        }
        public async Task<bool> HashDelete(string accountId)
        {
            return await redisStorage.HashDelete(KEY,accountId);
        }
    }
}
