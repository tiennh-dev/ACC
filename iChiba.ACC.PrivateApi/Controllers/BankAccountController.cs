using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.ACC.PrivateApi.AppModel.Model;
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
    public class BankAccountController : BaseController
    {
        private readonly IBankAccountAppService bankaccountAppService;

        public BankAccountController(ILogger<AccountController> logger,
            IBankAccountAppService bankaccountAppService)
            : base(logger)
        {
            this.bankaccountAppService = bankaccountAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankAccountListResponse))]
        public async Task<IActionResult> GetJtable(BankAccountJtableModel jtableModel)
        {
            try
            {
                var appserviceRequest = jtableModel.ToModel();
                var response = await bankaccountAppService.GetBankAccounts(appserviceRequest);
                var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                jtableModel.Draw,
                response.Total);

                return Ok(responseJTable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> AddBankAccount(BankAccountAddRequest request)
        {
            try
            {
                var response = await bankaccountAppService.Add(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteBankAccount(int Id)
        {
            try
            {
                var response = await bankaccountAppService.Delete(Id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost("{BankAccountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<BankAccountView>))]
        public async Task<IActionResult> GetBankAccountBuyId(int BankAccountId)
        {
            try
            {
                var response = await bankaccountAppService.GetBankAccountById(BankAccountId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }



        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> EditBankAccount(BankAccountEditRequest request)
        {
            try
            {
                var response = await bankaccountAppService.EditBankAccount(request);

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