using Ichiba.Bank.Api.Driver;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Driver
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
