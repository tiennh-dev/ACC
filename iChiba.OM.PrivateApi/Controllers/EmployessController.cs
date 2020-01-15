using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Empolyess;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.Employess;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class EmployessController : BaseController
    {
        private readonly IEmployessAppservice employessappservice;
        private readonly ICustomerAppService customerAppService;
        public EmployessController(ILogger<EmployessController> logger,
            IEmployessAppservice employessappservice,
             ICustomerAppService customerAppService
          )
          : base(logger)
        {
            this.employessappservice = employessappservice;
            this.customerAppService = customerAppService;
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(EmployessResponse))]
        public async Task<IActionResult> GetListEmployess(EmployessAddRequest request)
        {
            try
            {
                var response = await employessappservice.GetListEmployess(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerUpdateResponse))]
        public async Task<IActionResult> UpdateCareby(CustomerUpdateRequest request)
        {
            var response = await employessappservice.Update(request);

            return Ok(response);
        }
    }
}