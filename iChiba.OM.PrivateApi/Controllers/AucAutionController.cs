using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.Cache.Cache.Model;
using iChiba.OM.Cache.Redis.Implement.YahooAuctions;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppService.Implement;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using iChiba.OM.Service.Interface;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class AucAutionController : BaseController
    {
        private readonly ISuccessfulBidService successfulBidService;
        private readonly IProductBidClientInfoAppService bidClientCache;
        private readonly ISuccessfulBidAppService _successfulBidAppService;
        private readonly ICustomerAppService customerAppService;
        private readonly IOrderAppService orderAppService;

        public AucAutionController(ILogger<AucAutionController> logger,
            ISuccessfulBidService successfulBidService, IProductBidClientInfoAppService bidClientCache,
            ISuccessfulBidAppService successfulBidAppService,
            IOrderAppService orderAppService,
            ICustomerAppService customerAppService, AccessClient accessClient, AppConfig appConfig)
            : base(logger, accessClient, appConfig)
        {
            this.successfulBidService = successfulBidService;
            this.bidClientCache = bidClientCache;
            this._successfulBidAppService = successfulBidAppService;
            this.customerAppService = customerAppService;
            this.orderAppService = orderAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SuccessfulBidListResponse))]
        public async Task<IActionResult> GetListJTable(SuccessfulBidListJTableModel request)
        {
            try
            {
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var appserviceRequest = request.ToModel();
                    appserviceRequest.PreCode = AutionConfig.AUC;
                    var response = await _successfulBidAppService.GetListAutionJTable(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
                else
                {

                    var appserviceRequest = request.ToModel();
                    appserviceRequest.PreCode = AutionConfig.AUC;
                    var response = await _successfulBidAppService.GetListAutionJTableByCare(appserviceRequest);
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
        //[HttpPost]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType((int)HttpStatusCode.Forbidden)]
        //[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SuccessfulBidListResponse))]
        //public async Task<IActionResult> GetListSuccessfulBidJTable(SuccessfulBidListJTableModel request)
        //{
        //    try
        //    {
        //        //var keyword = request.Search.Value;
        //        //bổ sung tìm kiếm nhanh
        //        var appserviceRequest = request.ToModel();
        //        //var defaultSorts = new Sort()
        //        //{
        //        //    SortBy = "Order",
        //        //    SortDirection = Sort.SORT_DIRECTION_DESC
        //        //};
        //        //appserviceRequest.Sorts.Insert(0, defaultSorts);

        //        var response = await _successfulBidAppService.GetListSuccessfulBidJTable(appserviceRequest);
        //        var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
        //             request.Draw,
        //            response.Total);

        //        return Ok(responseJTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, ex.Message);

        //        return BadRequest();
        //    }
        //}

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SuccessfulBidListResponse))]
        public async Task<IActionResult> GetList(SuccessfulBidListRequest request)
        {
            var response = await _successfulBidAppService.GetList(request);

            return Ok(response);
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



        [HttpPost("{code}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<OrderdetailListResponse>))]
        public IActionResult GetOrderByCode(string code)
        {
            try
            {
                var response = orderAppService.GetOrderByCode(code);
                return Ok(response);
            }catch(Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<Model.OrderMessage>>))]
        public async Task<IActionResult> GetMessages(OrderMessageGetByOrderIdRequest request)
        {
            try
            {
                var response = await orderAppService.GetMessages(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SuccessfulBidListResponse))]
        public async Task<IActionResult> GetAllList()
        {
            try
            {
                var response = await _successfulBidAppService.GetAll();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductBidClientInfoListResponse))]
        public async Task<IActionResult> GetBidClient()
        {
          
            try
            {
                var response = await bidClientCache.Gets(true);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetMapSuccessfulBid()
        {
            var response = await _successfulBidAppService.GetMapSuccessful();

            return Ok(response);
        }
        [HttpPost("{status}/{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ChangePaymentStatus(int status, string id)
        {
            var response = await _successfulBidAppService.ChangePaymentStatus(status, id);

            return Ok(response);
        }
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> ExportExcel()
        {
            byte[] fileContent;

            using (ExcelPackage package = new ExcelPackage())
            {

                IList<Successfulbid> customerList = successfulBidService.GetAll();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromCollection(customerList, true);
                worksheet.Cells.AutoFitColumns();
                fileContent = package.GetAsByteArray();
            }
            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound();
            }
            return File(
                fileContents: fileContent,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "Successful Bid.xlsx"
                );
        }
    }
}