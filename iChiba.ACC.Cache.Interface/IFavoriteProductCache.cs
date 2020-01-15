
using iChiba.Cache.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface.IFavoriteProductCache
{
    public interface IFavoriteProductCache : IBaseHashCache<FavoriteProduct, string>
    {
        Task<bool> HashSet(string accountId, IList<FavoriteProduct> model);
        Task<IList<FavoriteProduct>> GetByAcount(string accountId);
        Task<bool> DeleteAllByAccount(string accountId);
    }
}
