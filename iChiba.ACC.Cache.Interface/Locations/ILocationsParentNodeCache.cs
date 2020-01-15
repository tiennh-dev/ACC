using iChiba.ACC.Cache.Cache.Model;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface ILocationsParentNodeCache : IBaseHashCache<LocationsParentNode, int>
    {
        Task<bool> HashSet(LocationsParentNode model);
    }
}
