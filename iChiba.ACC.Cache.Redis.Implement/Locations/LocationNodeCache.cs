using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Cache.Model;
using iChiba.ACC.Cache.Interface;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class LocationsNodeCache : BaseHashCache<LocationsNode, int>, ILocationsNodeCache
    {
        private const string KEY = "Locations-Node";

        public LocationsNodeCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public async Task<bool> HashSet(LocationsNode model)
        {
            return await redisStorage.HashSet(key, model.Id.ToString(), model);
        }
    }
}
