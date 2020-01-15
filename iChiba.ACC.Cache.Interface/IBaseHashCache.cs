using Core.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface IBaseHashCache<TEntity, TId>
    {
        Task<TEntity> GetById(TId id);
        Task<IList<TEntity>> GetByIds(params TId[] ids);
        Task HashDelete(TId id);
        Task<IList<TEntity>> GetAll();
    }
}
