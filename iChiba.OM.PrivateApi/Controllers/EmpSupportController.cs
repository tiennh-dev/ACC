using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.OM.Cache.Cache.Model;
using iChiba.OM.Cache.Interface;
using iChiba.OM.PrivateApi.AppModel.Model.Employess;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.Employess;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class EmpSupportController : BaseController
    {
        private readonly ICustomerAppService customerAppService;
        private readonly IEmployessAppservice employessappservice;
        private readonly IAspNetUserCache aspNetUserCache;

        public EmpSupportController(ILogger<EmpSupportController> logger,
              ICustomerAppService customerAppService,
              IEmployessAppservice employessappservice, IAspNetUserCache aspNetUserCache
            ) : base(logger)
        {
            this.customerAppService = customerAppService;
            this.employessappservice = employessappservice;
            this.aspNetUserCache = aspNetUserCache;
        }


        /// <summary>
        /// Lấy tất cả danh sách khách hàng có phân trang
        /// </summary>
        /// <returns>tất cả danh sách khách hàng</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListAllCustomer(CustomerListRequest request)
        {
            var response = await employessappservice.GetListTable(request);
            return Ok(response);
        }

        /// <summary>
        /// lấy tất cả ds employees
        /// </summary>
        /// <returns>
        /// trả lại tất ds employees
        /// </returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(EmployessResponse))]
        public async Task<IActionResult> GetListAllEmp()
        {
            var response =await employessappservice.GetAll();
            return Ok(response);
        }

        ///// <summary>
        ///// Lấy thông tin khách hàng theo nhân viên hỗ trợ
        ///// </summary>
        ///// <param name="careBy"></param>
        ///// <returns>
        /////  thông tin khách hàng theo thông tin hỗ trợ
        ///// </returns>
        //[HttpPost("{careBy}")]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType((int)HttpStatusCode.Forbidden)]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //public async Task<IActionResult> GetCustomerByCareBy(string careBy)
        //{
        //    var cusdata = await employessappservice.GetcustomerbyCareby(careBy);
        //    return Ok(cusdata);

        //}


        /// <summary>
        /// Lấy thông tin khách hàng theo nhân viên hỗ trợ
        /// </summary>
        /// <param name="careBy"></param>
        /// <returns>
        ///  thông tin khách hàng theo thông tin hỗ trợ
        /// </returns>
        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomerByCareBy(EmpsJTable request)
        {
            var requestapp = request.ToModel();
            var cusdata = await employessappservice.GetcustomerbyCareby(requestapp);
            var responseJTable = JTableHelper.JObjectTable(cusdata.Data.ToList(),
                  request.Draw,
                 cusdata.Total);

            return Ok(responseJTable);

        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListCustomerwithCareBy(CustomerListRequest request)
        {
            var data = await employessappservice.GetListCustomerWithCareBy(request);
            return Ok(data);

        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListCustomerWithoutCareBy(CustomerListRequest request)
        {
            var data = await employessappservice.GetListCustomerWithoutCareBy(request);
            return Ok(data);

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