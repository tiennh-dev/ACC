using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface;
using iChiba.ACC.Cache.Model;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class ProductShortCache : BaseHashCache<ProductShort, string>, IProductShortCache
    {
        private const string KEY = "YahooAuctions-Product-Short";

        public ProductShortCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public Task<ProductShort[]> GetAll()
        {
            return redisStorage.HashGetAll<ProductShort>(key);
        }

        public Task<bool> HashSet(ProductShort model)
        {
            return redisStorage.HashSet(key, model.Id, model);
        }
    }
}
