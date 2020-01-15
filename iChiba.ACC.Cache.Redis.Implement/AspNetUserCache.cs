using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Cache.Model;
using iChiba.ACC.Cache.Interface;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class AspNetUserCache : BaseHashCache<AspNetUser, string>, IAspNetUserCache
    {
        private const string KEY = "AIM_ASPNETUSER";

        public AspNetUserCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }

        public Task<bool> HashSet(AspNetUser model)
        {
            return redisStorage.HashSet(key, model.Id, model);
        }
    }
}
