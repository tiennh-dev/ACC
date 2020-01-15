using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iChiba.OM.PrivateApi.AppModel.Request.CustomerBankingInfo;
using iChiba.OM.PrivateApi.AppModel.Response.CustomerBankingInfo;
using iChiba.OM.PrivateApi.AppService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class CustomerBankingInfoController : BaseController
    {
        private readonly ICustomerBankingInfoAppService customerbkinfoappserivice;
        public CustomerBankingInfoController(ILogger<CustomerBankingInfoController> logger,
            ICustomerBankingInfoAppService customerbkinfoappserivice)
            : base(logger)
        {
            this.customerbkinfoappserivice = customerbkinfoappserivice;
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerBankingInfoResponse))]
        public async Task<IActionResult> GetJTable(CustomerBankingInfoListRequest request)
        {
            try
            {
                var data =await customerbkinfoappserivice.GetListTable(request);
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