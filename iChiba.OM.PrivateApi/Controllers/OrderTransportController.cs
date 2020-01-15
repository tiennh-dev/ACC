using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.Cache.Cache.Model;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Model.Warehouse;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.Order;
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
    public class OrderTransportController : BaseController
    {
        private readonly IOrderAppService orderAppService;
        private readonly IOrderTransportAppService orderTransportAppService;
        private readonly ICustomerAddressAppService customerAddressAppService;
        private readonly IOrderServiceAppService orderServiceAppService;
        private readonly IWarehouseAppService warehouseAppService;
        private readonly IProductTypeAppService productTypeAppService;

        public OrderTransportController(ILogger<OrderTransportController> logger,
            IOrderAppService orderAppService,
            IOrderTransportAppService orderTransportAppService,
            ICustomerAddressAppService customerAddressAppService,
            IOrderServiceAppService orderServiceAppService,
            IWarehouseAppService warehouseAppService,
            IProductTypeAppService productTypeAppService,
            AppConfig appConfig,
            AccessClient accessClient
            ) : base(logger, accessClient, appConfig)
        {
            this.orderAppService = orderAppService;
            this.orderTransportAppService = orderTransportAppService;
            this.customerAddressAppService = customerAddressAppService;
            this.orderServiceAppService = orderServiceAppService;
            this.warehouseAppService = warehouseAppService;
            this.productTypeAppService = productTypeAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> GetJTable(OrderListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());

                //appserviceRequest.OrderType = Model.OrderRefType.TRANSPORT;
                ////appserviceRequest.PreCode = AutionConfig.AUC;
                //var response = await orderAppService.GetListJTableAution(appserviceRequest);
                //var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                //    request.Draw,
                //    response.Total);

                //return Ok(responseJTable);

                if (isPerm)
                {
                    appserviceRequest.OrderType = Model.OrderRefType.TRANSPORT;
                    var response = await orderAppService.GetListJTableOrderTransport(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                        request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
                else
                {
                    appserviceRequest.OrderType = Model.OrderRefType.TRANSPORT;
                    var response = await orderAppService.GetListJTableOrderTransportBySale(appserviceRequest);
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

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<OrderTransport>))]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                var response = await orderTransportAppService.GetDetail(id);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Add(OrderTransportRequest request)
        {
            var response = await orderTransportAppService.Add(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Update(OrderTransportRequest request)
        {
            var response = await orderTransportAppService.Update(request);

            return Ok(response);
        }



        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateStatus(int id,int? trackingStatus)
        {
            var response = await orderTransportAppService.UpdateStatus(id, trackingStatus);

            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<CustomerAddressList>>))]
        public async Task<IActionResult> GetListCustomerAddress(CustomerAddressListByAccountRequest request)
        {
            var response = await customerAddressAppService.GetListByAccountId(request);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<OrderServiceList>>))]
        public async Task<IActionResult> GetAllOrderService()
        {
            var response = await orderServiceAppService.GetAll();
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<WarehouseList>>))]
        public async Task<IActionResult> GetAllWarehouse()
        {
            var response = await warehouseAppService.GetAllWarehouseActive();
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<ProductTypeList>>))]
        public async Task<IActionResult> GetAllProductType()
        {
            var response = await productTypeAppService.GetAll();
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<AspNetUser>))]
        public async Task<IActionResult> GetSaler()
        {
            try
            {
                var response = new BaseResponse();
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var data = await orderAppService.GetSaler();
                    return Ok(data);
                }
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