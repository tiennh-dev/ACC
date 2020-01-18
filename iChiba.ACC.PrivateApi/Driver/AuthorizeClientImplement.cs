using Ichiba.Bank.Api.Driver;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.Driver
{
    public class AuthorizeClientImplement : IAuthorizeClient, Ichiba.Partner.Api.Driver.IAuthorizeClient
    {
        public AuthorizeClientImplement()
        {
        }

        public Task<string> GetAuthorizeToken()
        {
            return Task.FromResult(string.Empty);
        }
    }
}
