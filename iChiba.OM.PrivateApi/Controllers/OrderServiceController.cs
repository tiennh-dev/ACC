using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.Cache.Cache.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Model.Locations;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.LocationListResponse;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class OrderServiceController : BaseController
    {
        private readonly IOrderServiceAppService orderServiceAppService;

        public OrderServiceController(ILogger<OrderServiceController> logger,
            IOrderServiceAppService orderServiceAppService)
            : base(logger)
        {
            this.orderServiceAppService = orderServiceAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingResponse<IList<OrderServiceList>>))]
        public async Task<IActionResult> GetJTable(OrderServiceListJTableModel request)
        {
            try
            {
                //var keyword = request.Search.Value;
                //bổ sung tìm kiếm nhanh
                var appserviceRequest = request.ToModel();
                var response = await orderServiceAppService.GetList(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingResponse<IList<OrderServiceList>>))]
        public async Task<IActionResult> GetList(OrderServiceListRequest request)
        {
            var response = await orderServiceAppService.GetList(request);

            return Ok(response);
        }


        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<OrderServiceList>))]
        public async Task<IActionResult> GetDetail(int id)
        {
            var response = await orderServiceAppService.GetDetail(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Add(OrderServiceAddRequest request)
        {
            var response = await orderServiceAppService.Add(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Update(OrderServiceUpdateRequest request)
        {
            var response = await orderServiceAppService.Update(request);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await orderServiceAppService.Delete(id);

            return Ok(response);
        }
    }
}