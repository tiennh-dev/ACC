using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Cache.Model;
using iChiba.ACC.Cache.Interface;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class LocationsCache : BaseHashCache<Locations, int>, ILocationsCache
    {
        private const string KEY = "Locations";

        public LocationsCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public async Task<bool> HashSet(Locations model)
        {
            return await redisStorage.HashSet(key, model.Id.ToString(), model);
        }
    }
}
