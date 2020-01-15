using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class BidLastTimeController : BaseController
    {
        private readonly IBidLastTimeAppService bidLastTimeAppService;

        public BidLastTimeController(ILogger<BidLastTimeController> logger,
            IBidLastTimeAppService bidLastTimeAppService, AccessClient accessClient, AppConfig appConfig)
            : base(logger, accessClient, appConfig)
        {
            this.bidLastTimeAppService = bidLastTimeAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BidLastTimeResponse))]
        public async Task<IActionResult> GetJTable(BidLastTimeJTableModel request)
        {
            try
            {
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var appserviceRequest = request.ToModel();
                    var response = await bidLastTimeAppService.GetJTable(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
                else
                {

                    var appserviceRequest = request.ToModel();
                    var response = await bidLastTimeAppService.GetJTableByCare(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
               
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListCustomer()
        {
            try
            {
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var response = await bidLastTimeAppService.GetListCustomer();
                    return Ok(response);
                }
                else
                {

                    var response = await bidLastTimeAppService.GetListCustomerByCare();
                    return Ok(response);
                }
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}