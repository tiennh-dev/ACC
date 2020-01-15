using Core.Cache.Redis.Interface;
using iChiba.ACC.Cache.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Redis.Implement
{
    public class BaseHashCache<TEntity, TId> : IBaseHashCache<TEntity, TId>
    {
        protected readonly IRedisStorage redisStorage;
        protected readonly string key;

        public BaseHashCache(IRedisStorage redisStorage,
            string key)
        {
            this.redisStorage = redisStorage;
            this.key = key;
        }

        protected string BuildParameter(params object[] parameters)
        {
            var separator = string.Empty;
            var result = string.Empty;

            foreach (var item in parameters)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.ToString()))
                {
                    continue;
                }

                var value = item.ToString();

                result = $"{result}{separator}{value}";
                separator = "-";
            }

            return result;
        }

        public async Task<TEntity> GetById(TId id)
        {
            return await redisStorage.HashGet<TEntity>(key, id.ToString());
        }

        public async Task<IList<TEntity>> GetByIds(params TId[] ids)
        {
            var fields = ids.Select(m => m.ToString())
                .ToArray();

            return await redisStorage.HashGet<TEntity>(key, fields);
        }

        public async Task HashDelete(TId id)
        {
            await redisStorage.HashDelete(key, id.ToString());
        }

        public async Task<IList<TEntity>> GetAll()
        {
            return await redisStorage.HashGetAll<TEntity>(key);
        }
    }
}
