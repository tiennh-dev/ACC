using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iChiba.OM.PrivateApi.AppModel.Request.CustomerWallet;
using iChiba.OM.PrivateApi.AppModel.Response.CustomerWallet;
using iChiba.OM.PrivateApi.AppService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class CustomerWalletController : BaseController
    {
        private readonly ICustomerWalletAppService customerwalletappservice;
        public CustomerWalletController(ILogger<CustomerWalletController> logger,
            ICustomerWalletAppService customerwalletappservice
            ) : base(logger)
        {
            this.customerwalletappservice = customerwalletappservice;
        }



        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerWalletListResponse))]
        public async Task<IActionResult> GetDataTable(string accountId)
        {
            try
            {
                var data =await customerwalletappservice.GetByAccountId(accountId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }
    }
}