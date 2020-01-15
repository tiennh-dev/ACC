using Core.Resilience.Http;
using Ichiba.IS4.Api.Driver.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ichiba.IS4.Api.Driver
{
    public class AccessConfig
    {
        public string GetResources { get; set; }
        public string CheckPermission { get; set; }
    }

    public class AccessClient : BaseClient
    {
        private readonly AccessConfig accessConfig;

        public AccessClient(IHttpClient httpClient,
            IAuthorizeClient authorizeClient,
            AccessConfig accessConfig)
            : base(httpClient, authorizeClient)
        {
            this.accessConfig = accessConfig;
        }

        public async Task<IList<Resource>> GetResources(string groupResourceKey)
        {
            var url = $"{accessConfig.GetResources}/{groupResourceKey}";
            var response = await Get<IList<Resource>>(url);

            return response;
        }

        public async Task<bool> CheckPermission(string groupResourceKey, string resourceKey, params string[] actions)
        {
            var action = string.Join(',', actions);
            var url = $"{accessConfig.CheckPermission}/{groupResourceKey}/{resourceKey}/{action}";
            var response = await Get<bool>(url);

            return response;
        }
    }
}
