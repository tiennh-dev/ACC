using Core.Common;
using Core.Resilience.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Ichiba.Partner.Api.Driver
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

            //response.EnsureSuccessStatusCode();
            EnsureSuccessStatusCode(response);

            var content = await response.Content.ReadAsStringAsync();
            var data = Serialize.JsonDeserializeObject<TResponse>(content);

            return data;
        }

        protected virtual async Task<TResponse> Get<TResponse>(string uri,
            string authorizationMethod = "Bearer")
        {
            var authorizationToken = await AuthorizationToken();
            var response = await httpClient.GetStringAsync(uri,
                authorizationToken,
                authorizationMethod);
            var data = Serialize.JsonDeserializeObject<TResponse>(response);

            return data;
        }


        //protected virtual async Task<T> GetTAsync<T>(string uri)
        //{
        //    httpClient.BaseAddress = uri;
        //    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + request.Accesstoken);
        //    var res = await httpClient.GetAsync(uri);
        //    res.EnsureSuccessStatusCode();

        //    var stringResult = await res.Content.ReadAsStringAsync();
        //    var data = Serialize.JsonDeserializeObject<T>(stringResult);
        //}

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

        protected void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var handlerStatusCodes = new Dictionary<HttpStatusCode, Action>()
                {
                    { HttpStatusCode.Unauthorized, () => throw new UnauthorizedAccessException() }
                };

                if (handlerStatusCodes.ContainsKey(response.StatusCode))
                {
                    handlerStatusCodes[response.StatusCode].Invoke();
                }
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
