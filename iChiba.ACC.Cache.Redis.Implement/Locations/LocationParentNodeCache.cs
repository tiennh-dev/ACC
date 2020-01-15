using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Cache.Model;
using iChiba.ACC.Cache.Interface;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class LocationsParentNodeCache : BaseHashCache<LocationsParentNode, int>, ILocationsParentNodeCache
    {
        private const string KEY = "Locations-Parent-Node";

        public LocationsParentNodeCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public async Task<bool> HashSet(LocationsParentNode model)
        {
            return await redisStorage.HashSet(key, model.Id.ToString(), model);
        }
    }
}
