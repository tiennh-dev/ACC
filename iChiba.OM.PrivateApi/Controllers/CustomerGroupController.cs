using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Model.CustomerGroup;
using iChiba.OM.PrivateApi.AppService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class CustomerGroupController : BaseController
    {
        private readonly ICustomerGroupAppService customergroupapp;
        public CustomerGroupController(ILogger<CustomerGroupController> logger,
            ICustomerGroupAppService customergroupapp
             ) : base(logger)
        {
            this.customergroupapp = customergroupapp;
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<CustomerGroupApp>>))]
        public async Task<IActionResult> GetList()
        {
            var response = await customergroupapp.GetList();
            return Ok(response);
        }
    }
}