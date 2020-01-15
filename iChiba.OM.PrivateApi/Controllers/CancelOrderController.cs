using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using Core.CustomException;
using iChiba.OM.Cache.Cache.Model;
using iChiba.OM.CustomException;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Response.Order;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class CancelOrderController : BaseController
    {
        private readonly IOrderAppService orderAppService;
        private readonly ICustomerAppService customerAppService;
        private readonly IHostingEnvironment environment;


        public CancelOrderController(ILogger<CancelOrderController> logger,
            IOrderAppService orderAppService, ICustomerAppService customerAppService, IHostingEnvironment environment, AccessClient accessClient, AppConfig appConfig
             ) : base(logger, accessClient, appConfig)
        {
            this.orderAppService = orderAppService;
            this.customerAppService = customerAppService;
            this.environment = environment;
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
                if (isPerm)
                {
                    appserviceRequest.PreStatus = (int)OrderStatus.DA_HUY;
                    appserviceRequest.PreState = OrderState.CHO_XU_LY_HUY_DON;
                    var response = await orderAppService.GetOrderCancelJTable(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
                else
                {
                    appserviceRequest.PreStatus = (int)OrderStatus.DA_HUY;
                    appserviceRequest.PreState = OrderState.CHO_XU_LY_HUY_DON;
                    var response = await orderAppService.GetListJTableOrderCancelBySale(appserviceRequest);
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






        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Export(OrderListRequest request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }
                request.PreStatus = (int)OrderStatus.DA_HUY;
                request.PreState = OrderState.CHO_XU_LY_HUY_DON;
                var data = await orderAppService.ExportCancelOrder(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "Cancel_Order_Teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                


                    foreach (var item in data.Data)
                    {
                        
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Hyperlink = new Uri(item.ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 4].Value = item.BidAccount;
                        workSheet.Cells[irow, 5].Value = item.Total;
                        workSheet.Cells[irow, 6].Value = item.ShippingFee;
                        workSheet.Cells[irow, 7].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 8].Value = item.BuyFee;
                        workSheet.Cells[irow, 9].Value = item.Paid;
                        workSheet.Cells[irow, 10].Value = item.StatusNote;
                        workSheet.Cells[irow, 11].Value = item.OrderDateDisplay;
                        workSheet.Cells[irow, 12].Value = item.FullName;
                        workSheet.Cells[irow, 13].Value = item.EmpployeeSupport;
                      
                        irow += 1;
                    }
                  
                   
                    workSheet.Cells["A7:M7"].Style.Font.Size = 11;
                    workSheet.Cells["A7:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A7:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A7:M" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:M" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:M" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:M" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));
                    workSheet.Cells["A7:M7"].Style.Font.Bold = true;

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Cancel_Order_Teamplate.xlsx");
                    }
                }

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
            var response = await customerAppService.GetListTopCustomer(request);
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
                var data = await orderAppService.GetSaler();
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }
         
    }
}