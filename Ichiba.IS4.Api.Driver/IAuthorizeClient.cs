using System.Threading.Tasks;

namespace Ichiba.IS4.Api.Driver
{
    public interface IAuthorizeClient
    {
        Task<string> GetAuthorizeToken();
    }
}