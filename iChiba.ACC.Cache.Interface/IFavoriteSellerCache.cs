using iChiba.ACC.Cache.Interface;
using iChiba.ACC.Cache.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface.IFavoriteSellerCache
{
    public interface IFavoriteSellerCache : IBaseHashCache<FavoriteSeller, string>
    {
        Task<IList<FavoriteSeller>> GetByAcount(string accountId);
        Task<bool> HashSet(string accountId, IList<FavoriteSeller> model);
    }
}
