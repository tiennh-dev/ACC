using iChiba.ACC.Cache.Model.YahooAuctions;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface.YahooAuctions
{
    public interface IUserBidProductCache : IBaseHashCache<UserBidProduct, string>
    {
        Task<bool> HashSet(UserBidProduct model);
    }
}
