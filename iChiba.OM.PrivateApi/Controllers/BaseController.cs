using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
        protected readonly ILogger logger;
        private readonly AccessClient accessClient;
        private readonly AppConfig appConfig;
        public BaseController(ILogger logger)
        {
            this.logger = logger; 
        }
        public BaseController(ILogger logger, AccessClient accessClient, AppConfig appConfig)
        {
            this.logger = logger;
            this.accessClient = accessClient;
            this.appConfig = appConfig;
        }
        protected async Task<bool> CheckPermission(string[] actions)
        {
            string resourceKey = ControllerContext.RouteData.Values["controller"].ToString();
            var isAccessAllow = await accessClient.CheckPermission(appConfig.AppGroupResourceKey, resourceKey.ToUpper(), actions);
            return isAccessAllow;
        }
        protected async Task<bool> CheckPermission(string resource, string[] actions)
        {
            var isAccessAllow = await accessClient.CheckPermission(appConfig.AppGroupResourceKey, resource.ToUpper(), actions);
            return isAccessAllow;
        }
        protected async Task<bool> CheckPermission(string action)
        {
            var actions = new string[1] { action };
            
            string resourceKey = ControllerContext.RouteData.Values["controller"].ToString();
            var isAccessAllow = await accessClient.CheckPermission(appConfig.AppGroupResourceKey, resourceKey.ToUpper(), actions);
            return isAccessAllow;
        }
        protected async Task<bool> CheckPermission(string resource, string action)
        {
            var actions = new string[1] { action };
            var isAccessAllow = await accessClient.CheckPermission(appConfig.AppGroupResourceKey, resource.ToUpper(), actions);
            return isAccessAllow;
        }
    }
}
