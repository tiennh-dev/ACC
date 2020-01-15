using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface.YahooAuctions;
using iChiba.ACC.Cache.Model.YahooAuctions;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement.YahooAuctions
{
    public class ProductBidClientCache : BaseHashCache<ProductBidClientInfo, string>, IProductBidClientCache
    {
        private const string KEY = "YahooAuctions-Product-Bid-Client";

        public ProductBidClientCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public Task<bool> HashSet(ProductBidClientInfo model)
        {
            return redisStorage.HashSet(key, model.YAUserName, model);
        }
    }
}
