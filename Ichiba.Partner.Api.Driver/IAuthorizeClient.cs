using System.Threading.Tasks;

namespace Ichiba.Partner.Api.Driver
{
    public interface IAuthorizeClient
    {
        Task<string> GetAuthorizeToken();
    }
}
