using Core.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class OrderPackageProductController : BaseController
    {
        private readonly IOrderPackageProductAppService orderPackageProductAppService;

        public OrderPackageProductController(ILogger<OrderPackageProductController> logger,
            IOrderPackageProductAppService orderPackageProductAppService)
            : base(logger)
        {
            this.orderPackageProductAppService = orderPackageProductAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingResponse<IList<OrderPackageProductList>>))]
        public async Task<IActionResult> GetList(OrderPackageProductListRequest request)
        {
            var response = await orderPackageProductAppService.GetList(request);

            return Ok(response);
        }


        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<OrderPackageProductList>))]
        public async Task<IActionResult> GetDetail(int id)
        {
            var response = await orderPackageProductAppService.GetDetail(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Add(OrderPackageProductAddRequest request)
        {
            var response = await orderPackageProductAppService.Add(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Update(OrderPackageProductUpdateRequest request)
        {
            var response = await orderPackageProductAppService.Update(request);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await orderPackageProductAppService.Delete(id);

            return Ok(response);
        }
    }
}