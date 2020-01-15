using Core.Common.JTable;
using iChiba.OM.Cache.Cache.Model;
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
    public class CustomerAddressController : BaseController
    {
        private readonly ICustomerAddressAppService customerAddressAppService;

        public CustomerAddressController(ILogger<CustomerAddressController> logger,
            ICustomerAddressAppService customerAddressAppService)
            : base(logger)
        {
            this.customerAddressAppService = customerAddressAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddressListResponse))]
        public async Task<IActionResult> GetJTable(CustomerAddressListJTableModel request)
        {
            try
            {
                //var keyword = request.Search.Value;
                //bổ sung tìm kiếm nhanh
                var appserviceRequest = request.ToModel();
                var response = await customerAddressAppService.GetList(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddressListResponse))]
        public async Task<IActionResult> GetList(CustomerAddressListRequest request)
        {
            var response = await customerAddressAppService.GetList(request);

            return Ok(response);
        }


        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddressDetailResponse))]
        public async Task<IActionResult> GetDetail(int id)
        {
            var response = await customerAddressAppService.GetDetail(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddressAddResponse))]
        public async Task<IActionResult> Add(CustomerAddressAddRequest request)
        {
            var response = await customerAddressAppService.Add(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddressUpdateResponse))]
        public async Task<IActionResult> Update(CustomerAddressUpdateRequest request)
        {
            var response = await customerAddressAppService.Update(request);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddressDeleteResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await customerAddressAppService.Delete(id);

            return Ok(response);
        }


        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK,Type =typeof(LocationListResponse))]
        public async Task<IActionResult> GetLocation()
        {
            const int Id = 0;
            var response = await customerAddressAppService.GetListLocation(Id);
            return Ok(response);
        }


        [HttpPost("{code}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LocationListResponse))]
        public async Task<IActionResult> GetListDistins(string code)
        {
            var response = await customerAddressAppService.GetListDistins(code);
            return Ok(response);
        }

        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LocationListResponse))]
        public async Task<IActionResult> GetListWard(int Id)
        {
            var response = await customerAddressAppService.GetListWard(Id);
            return Ok(response);
        }


        [HttpPost("{name}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LocationListResponse))]
        public async Task<IActionResult> Get(string name)
        {
            var response = await customerAddressAppService.GetProvinceDetail(name);
            return Ok(response);
        }
    }
}