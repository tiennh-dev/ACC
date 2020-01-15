using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iChiba.OM.PrivateApi.AppModel.Request.ProductClientInfo;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppService.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class ProductBidController : BaseController
    {
        private readonly IProductBidClientInfoAppService productBidClientInfoAppService;

        public ProductBidController(ILogger<SuccessfulBidController> logger,
            IProductBidClientInfoAppService productBidClientInfoAppService)
            : base(logger)
        {
            this.productBidClientInfoAppService = productBidClientInfoAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductBidClientInfoListResponse))]
        public async Task<IActionResult> GetList()
        {
            var response = await productBidClientInfoAppService.Gets(null);

            return Ok(response);
        }
    }
}