
using iChiba.ACC.Cache.Cache.Model;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface IAspNetUserCache : IBaseHashCache<AspNetUser, string>
    {
        Task<bool> HashSet(AspNetUser model);
    }
}
