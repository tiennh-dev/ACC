using iChiba.ACC.Cache.Model;
using System.Threading.Tasks;

namespace iChiba.ACC.Cache.Interface
{
    public interface IProductShortCache : IBaseHashCache<ProductShort, string>
    {
        Task<bool> HashSet(ProductShort model);
        Task<ProductShort[]> GetAll();
    }

}
