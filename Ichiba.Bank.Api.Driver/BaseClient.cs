using Core.Common;
using Core.Resilience.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Ichiba.Bank.Api.Driver
{
    public class BaseClient
    {
        protected readonly IHttpClient httpClient;
        private readonly IAuthorizeClient authorizeClient;

        public BaseClient(IHttpClient httpClient,
            IAuthorizeClient authorizeClient)
        {
            this.httpClient = httpClient;
            this.authorizeClient = authorizeClient;
        }

        protected async Task<string> AuthorizationToken()
        {
            if (authorizeClient == null)
            {
                return null;
            }

              return await authorizeClient.GetAuthorizeToken();
        }

        protected virtual async Task<HttpResponseMessage> PostAsync<T>(string uri,
            T request,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            var authorizationToken = await AuthorizationToken();

            return await httpClient.PostAsync(uri, request, authorizationToken, requestId, authorizationMethod);
        }

        protected virtual async Task<TResponse> PostAsync<TResponse, TRequest>(string uri,
            TRequest request,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            var response = await PostAsync(uri,
                request,
                requestId,
                authorizationMethod);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = Serialize.JsonDeserializeObject<TResponse>(content);

            return data;
        }

        protected virtual async Task<TResponse> Get<TResponse>(string uri,
            string authorizationToken = null)
        {
            var response = await httpClient.GetStringAsync(uri,
                authorizationToken);
            var data = Serialize.JsonDeserializeObject<TResponse>(response);

            return data;
        }

        protected virtual async Task<TResponse> Get<TResponse>(string uri,
            Func<string, string> executeBeforeParse,
            string authorizationToken = null)
        {
            var response = await httpClient.GetStringAsync(uri,
                authorizationToken);

            if (executeBeforeParse != null)
            {
                response = executeBeforeParse.Invoke(response);
            }

            var data = Serialize.JsonDeserializeObject<TResponse>(response);

            return data;
        }

        protected string Join(char separator, params object[] inputs)
        {
            if (inputs == null)
            {
                return null;
            }

            var inputStandard = inputs.Where(m => m != null)
                .Select(m => m.ToString().Trim(separator));

            return string.Join(separator, inputStandard);
        }

        protected string BuildParameter(IDictionary<string, object> parameters)
        {
            var separator = string.Empty;
            var result = string.Empty;

            foreach (var item in parameters)
            {
                if (item.Value == null || string.IsNullOrWhiteSpace(item.Value.ToString()))
                {
                    continue;
                }

                var value = HttpUtility.UrlEncode(item.Value.ToString());

                result = $"{result}{separator}{item.Key}={value}";
                separator = "&";
            }

            return result;
        }
    }
}
