using iChiba.ACC.Cache.Interface;
using iChiba.ACC.Cache.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface IPriceQuoteLastTimeCache : IBaseHashCache<PriceQuoteLastTime, string>
    {
        Task<bool> HashSet(string accountId, IList<PriceQuoteLastTime> model);
        Task<IList<PriceQuoteLastTime>> GetByAcount(string accountId);
        Task<bool> HashDelete(string accountId);
    }
}
