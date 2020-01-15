using System.Threading.Tasks;

namespace Ichiba.Bank.Api.Driver
{
    public interface IAuthorizeClient
    {
        Task<string> GetAuthorizeToken();
    }
}