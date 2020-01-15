using iChiba.ACC.Cache.Cache.Model;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface ILocationsNodeCache : IBaseHashCache<LocationsNode, int>
    {
        Task<bool> HashSet(LocationsNode model);
    }
}
