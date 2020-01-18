using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.ACC.PrivateApi.AppModel.Request;
using iChiba.ACC.PrivateApi.AppModel.Response.Account;
using iChiba.ACC.PrivateApi.AppService.Interface;
using iChiba.ACC.PrivateApi.JTableModels;
using iChiba.ACC.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.ACC.PrivateApi.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountAppService accountAppService;

        public AccountController(ILogger<AccountController> logger,
            IAccountAppService accountAppService)
            : base(logger)
        {
            this.accountAppService = accountAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AccountListResponse))]
        public async Task<IActionResult> GetJtable(AccountJtableModel jtableModel)
        {
            try
            {
                var appserviceRequest = jtableModel.ToModel();
                var response = await accountAppService.GetAccount(appserviceRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AccountListResponse))]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var response = await accountAppService.GetAccountById(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }
    }
}