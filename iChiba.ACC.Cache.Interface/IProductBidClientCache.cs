using iChiba.ACC.Cache.Model.YahooAuctions;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface.YahooAuctions
{
    public interface IProductBidClientCache : IBaseHashCache<ProductBidClientInfo, string>
    {
        Task<bool> HashSet(ProductBidClientInfo model);
    }
}
