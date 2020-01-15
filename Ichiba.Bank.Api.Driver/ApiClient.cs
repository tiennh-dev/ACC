using Core.AppModel.Response;
using Core.Resilience.Http;
using Ichiba.Bank.Api.Driver.Models.Request;
using System.Threading.Tasks;

namespace Ichiba.Bank.Api.Driver
{
    public class ApiConfig
    {
        public string Login { get; set; }
        public string Restart { get; set; }
        public string GetCapcha { get; set; }
    }

    public class ApiClient : BaseClient
    {
        private readonly ApiConfig apiConfig;

        public ApiClient(IHttpClient httpClient,
            IAuthorizeClient authorizeClient,
            ApiConfig apiConfig)
            : base(httpClient, authorizeClient)
        {
            this.apiConfig = apiConfig;
        }

        public async Task<BaseEntityResponse<string>> GetCapcha(string baseUrl = null)
        {
            var url = Join('/', baseUrl, apiConfig.GetCapcha);
                var response = await PostAsync<BaseEntityResponse<string>, object>(url, null);

            return response;
        }

        public async Task<BaseEntityResponse<bool>> Restart(string baseUrl = null)
        {
            var url = Join('/', baseUrl, apiConfig.Restart);
            var response = await PostAsync<BaseEntityResponse<bool>, object>(url, null);

            return response;
        }

        public async Task<BaseEntityResponse<bool>> Login(LoginRequest request, string baseUrl = null)
        {
            var url = Join('/', baseUrl, apiConfig.Login);
            var response = await PostAsync<BaseEntityResponse<bool>, LoginRequest>(url, request);

            return response;
        }
    }
}
