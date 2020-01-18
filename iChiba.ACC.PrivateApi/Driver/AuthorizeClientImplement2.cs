using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.Driver
{
    public class AuthorizeClientImplement2 : IAuthorizeClient
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorizeClientImplement2(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAuthorizeToken()
        {
            var isAuthenticated = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            string authorizationToken = null;

            if (isAuthenticated)
            {
                authorizationToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            }

            return authorizationToken;
        }
    }
}
