using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.Cache.Interface;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.FavoriteProduct;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class FavoriteProductController : BaseController
    {
        private readonly IFavoriteProductAppService favoriteProductAppService;
        private readonly ICustomerAppService customerAppService;

        public FavoriteProductController(ILogger<FavoriteProductController> logger,
            IFavoriteProductAppService favoriteProductAppService, ICustomerAppService customerAppService, AccessClient accessClient,
            AppConfig appConfig)
            : base(logger, accessClient, appConfig)
        {
            this.favoriteProductAppService = favoriteProductAppService;
            this.customerAppService = customerAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FavoriteProductListResponse))]
        public async Task<IActionResult> GetJTable(FavoriteProductListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var response = await favoriteProductAppService.GetJTable(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<CustomerList>>))]
        public async Task<IActionResult> GetListTopCustomer(CustomerListTopRequest request)
        {
            var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
            if (isPerm)
            {
                var response = await customerAppService.GetListTopCustomer(request);
                return Ok(response);
            }
            else
            { 
                var response = await customerAppService.GetListTopCustomerByCare(request);
                return Ok(response);
            } 
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListCustomer()
        {
            var response = await favoriteProductAppService.GetListCustomer();
            return Ok(response);
        }
        [HttpPost("{id}/{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FavoriteProductDeleteResponse))]
        public async Task<IActionResult> Delete(string id,string accountId)
        {
            var response = await favoriteProductAppService.Delete(id,accountId);
            return Ok(response);
        }
    }
}