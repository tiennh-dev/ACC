using Core.Cache.Redis.Interface;
using iChiba.Cache.Model;
using iChiba.ACC.Cache.Interface.IFavoriteProductCache;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class FavoriteProductCache : BaseHashCache<FavoriteProduct, string>, IFavoriteProductCache
    {
        private const string KEY = "Favorite-Product";

        public FavoriteProductCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public async Task<bool> HashSet(string accountId, IList<FavoriteProduct> model)
        {
            return await redisStorage.HashSet(KEY, accountId, model);
        }

        public async Task<IList<FavoriteProduct>> GetByAcount(string accountId)
        {
            return await redisStorage.HashGet<IList<FavoriteProduct>>(KEY, accountId);
        }
        public async Task<bool> DeleteAllByAccount(string accountId)
        {
             return await redisStorage.HashDelete(KEY, accountId);
        }
    }
}
