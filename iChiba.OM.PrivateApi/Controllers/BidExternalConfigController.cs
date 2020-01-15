using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.BidExternalConfigList;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class BidExternalConfigController : BaseController
    {
        private readonly IBidExternalConfigAppService bidexternalconfigAppService;
        private readonly ICustomerAppService customerAppService;
        private readonly IOrderAppService orderAppService;


        public BidExternalConfigController(ILogger<BidExternalConfigController> logger,
            IBidExternalConfigAppService bidexternalconfigAppService, ICustomerAppService customerAppService,
            IOrderAppService orderAppService)
            : base(logger)
        {
            this.bidexternalconfigAppService = bidexternalconfigAppService;
            this.customerAppService = customerAppService;
            this.orderAppService = orderAppService;
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BidExternalConfigListResponse))]
        public async Task<IActionResult> GetJTable(BidExternalConfigListJTableModel request)
        {
            try
            {
                var modelRequest = request.ToModel();
                var response = await bidexternalconfigAppService.Gets(modelRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListTopCustomer(CustomerListTopRequest request)
        {
            var response = await customerAppService.GetListTopCustomer(request);
            return Ok(response);
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
                var response = await customerAppService.GetListAllCustomer();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BidExternalConfigListResponse))]
        public async Task<IActionResult> Add(BidExternalConfigAddListRequest request)
        {
            var response = await bidexternalconfigAppService.Add(request);
            return Ok(response);
        }

        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BidExternalConfigListResponse))]
        public async Task<IActionResult> Delete(int Id)
        {
            var response = await bidexternalconfigAppService.Delete(Id);
            return Ok(response);
        }


        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BidExternalConfigListResponse))]
        public async Task<IActionResult> GetDetail(int Id)
        {
            var response = bidexternalconfigAppService.GetDetail(Id);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BidExternalConfigListResponse))]
        public async Task<IActionResult> Update(BidExternalConfigUpdateListRequest request)
        {
            var response = await bidexternalconfigAppService.Update(request);
            return Ok(response);
        }


    }
}