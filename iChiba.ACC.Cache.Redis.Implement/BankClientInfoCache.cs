using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface;
using iChiba.ACC.Cache.Model;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class BankClientInfoCache : BaseHashCache<BankClientInfo, string>, IBankClientInfoCache
    {
        private const string KEY = "Bank-Client-Info";

        public BankClientInfoCache(IRedisStorage redisStorage)
            : base(redisStorage, KEY)
        {
        }
    }
}