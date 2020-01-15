using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Request.BankTransactionHistory;
using iChiba.OM.PrivateApi.AppModel.Response.BankInfo;
using iChiba.OM.PrivateApi.AppModel.Response.BankTransactionHistory;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class BankTransactionHistoryController : BaseController
    {
        private readonly IBankTransactionHistoryAppService bankTransactionHistoryAppService;

        public BankTransactionHistoryController(ILogger<BankTransactionHistoryController> logger,
            IBankTransactionHistoryAppService bankTransactionHistoryAppService)
            : base(logger)
        {
            this.bankTransactionHistoryAppService = bankTransactionHistoryAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankTransactionHistoryListResponse))]
        public async Task<IActionResult> GetJTable(BankTransactionHistoryListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var response = await bankTransactionHistoryAppService.GetAll(appserviceRequest);
                var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                     request.Draw,
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankInfoListResponse))]
        public async Task<IActionResult> GetAllBankInfo()
        {
            try
            {
                var response = await bankTransactionHistoryAppService.GetAllBankInfo();

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankTransactionHistoryDetailResponse))]
        public async Task<IActionResult> GetDetail(string id)
        {
            try
            {
                var response = await bankTransactionHistoryAppService.GetDetail(id);

                return Ok(response);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankTransactionHistoryUpdateResponse))]
        public async Task<IActionResult> Update(BankTransactionHistoryUpdateRequest request)
        {
            try
            {
                var response = await bankTransactionHistoryAppService.Update(request);

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