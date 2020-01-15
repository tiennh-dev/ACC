using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common;
using Core.Common.JTable;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class SuccessfulBidController : BaseController
    {
        private readonly ISuccessfulBidService successfulBidService;
        private readonly ISuccessfulBidAppService _successfulBidAppService;

        public SuccessfulBidController(ILogger<SuccessfulBidController> logger,
            ISuccessfulBidService successfulBidService,
            ISuccessfulBidAppService successfulBidAppService)
            : base(logger)
        {
            this.successfulBidService = successfulBidService;
            this._successfulBidAppService = successfulBidAppService;
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
                //var keyword = request.Search.Value;
                //bổ sung tìm kiếm nhanh
                var appserviceRequest = request.ToModel();
                //sort by order default
                //var defaultSorts = new Sort()
                //{
                //    SortBy = "Order",
                //    SortDirection = Sort.SORT_DIRECTION_DESC
                //};
                //appserviceRequest.Sorts.Insert(0, defaultSorts);


                var response = await _successfulBidAppService.GetListJTable(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SuccessfulBidListResponse))]
        public async Task<IActionResult> GetListSuccessfulBidJTable(SuccessfulBidListJTableModel request)
        {
            try
            {
                //var keyword = request.Search.Value;
                //bổ sung tìm kiếm nhanh
                var appserviceRequest = request.ToModel();
                //var defaultSorts = new Sort()
                //{
                //    SortBy = "Order",
                //    SortDirection = Sort.SORT_DIRECTION_DESC
                //};
                //appserviceRequest.Sorts.Insert(0, defaultSorts);

                var response = await _successfulBidAppService.GetListSuccessfulBidJTable(appserviceRequest);
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