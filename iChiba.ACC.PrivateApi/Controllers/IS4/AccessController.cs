using Core.AppModel.Response;
using iChiba.ACC.PrivateApi.AppService.Interface;
using iChiba.ACC.PrivateApi.Controllers;
using Ichiba.IS4.Api.Driver.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.Controllers
{
    public class AccessController : BaseController
    {
        private readonly IAccessAppService accessAppService;

        public AccessController(ILogger<AccessController> logger,
            IAccessAppService accessAppService) : base(logger)
        {
            this.accessAppService = accessAppService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<Resource>>))]
        public async Task<IActionResult> GetResources()
        {
            var response = await accessAppService.GetResources();

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CheckPermission(string resourceKey, string action)
        {
            var response = await accessAppService.CheckPermission(resourceKey, action);

            return Ok(response);
        }
    }
}