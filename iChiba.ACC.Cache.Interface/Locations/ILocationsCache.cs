using iChiba.ACC.Cache.Cache.Model;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface ILocationsCache : IBaseHashCache<Locations, int>
    {
        Task<bool> HashSet(Locations model);
    }
}
