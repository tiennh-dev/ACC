using iChiba.ACC.Cache.Model.YahooAuctions;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface.YahooAuctions
{
    public interface IBockedYAUserBidCache : IBaseHashCache<BockedYAUserBid, string>
    {
        Task<bool> HashSet(BockedYAUserBid model);
    }
}
