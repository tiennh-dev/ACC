using iChiba.OM.PrivateApi.AppModel.Response.BankClientInfo;
using iChiba.OM.PrivateApi.AppService.Interface;
using Ichiba.Bank.Api.Driver.Models.Request;
using Ichiba.Bank.Api.Driver.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class BankClientInfoController : BaseController
    {
        private readonly IBankClientInfoAppService bankClientInfoAppService;
        public BankClientInfoController(ILogger<BankClientInfoController> logger,
            IBankClientInfoAppService bankClientInfoAppService

            ) : base(logger)
        {
            this.bankClientInfoAppService = bankClientInfoAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankClientInfoListResponse))]
        public async Task<IActionResult> GetList()
        {
            BankClientInfoListResponse response = await bankClientInfoAppService.GetAll();

            return Ok(response);
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CapchaResponse))]
        public async Task<IActionResult> GetCapcha(string apiUrl)
        {
            CapchaResponse response = await bankClientInfoAppService.GetCapcha(apiUrl);

            return Ok(response);
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login(LoginRequest request, string apiUrl = null)
        {
            LoginResponse response = await bankClientInfoAppService.Login(request, apiUrl);

            return Ok(response);
        }
    }
}