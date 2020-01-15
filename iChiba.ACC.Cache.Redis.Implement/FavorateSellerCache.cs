using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface.IFavoriteSellerCache;
using iChiba.ACC.Cache.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class FavoriteSellerCache : BaseHashCache<FavoriteSeller, string>, IFavoriteSellerCache
    {
        private const string KEY = "Favorite-Seller";

        public FavoriteSellerCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public async Task<bool> HashSet(string accountId, IList<FavoriteSeller> model)
        {
            return await redisStorage.HashSet(KEY, accountId, model);
        }

        public async Task<IList<FavoriteSeller>> GetByAcount(string accountId)
        {
            return await redisStorage.HashGet<IList<FavoriteSeller>>(KEY, accountId);
        }
    }
}
