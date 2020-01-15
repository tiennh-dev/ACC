using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.CustomException;
using iChiba.OM.PrivateApi.AppModel.Model.PurchaseReport;
using iChiba.OM.PrivateApi.AppModel.Request.PurchaseReport;
using iChiba.OM.PrivateApi.AppModel.Response.PurchaseReport;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChibaShopping.Core.AppService.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class PurchaseReportController : BaseController
    {
        private readonly IPurchaseReportAppService purchaseReportService;
        private readonly IHostingEnvironment environment;
        public PurchaseReportController(ILogger<PurchaseReportController> logger, IPurchaseReportAppService purchaseReportService, IHostingEnvironment environment) : base(logger)
        {
            this.purchaseReportService = purchaseReportService;
            this.environment = environment;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdatePurchaseReport(PurchaseReportRequest request)
        {
            try
            {
                var response = await purchaseReportService.UpdatePurchaseReport(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<PurchaseReportView>>))]
        public async Task<IActionResult> GetListPurchaseReport()
        {
            try
            {
                var response = await purchaseReportService.GetListPurchaseReport();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PurchaseReportClientRequest))]
        public async Task<IActionResult> PurchaseReportLoad(PurchaseReportClientRequest request)
        {
            try
            {
                var response = await purchaseReportService.PurcahseReportLoad(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var response = await purchaseReportService.Delete(Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }



        /// đơn hàng DG
        /// 
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> AddNewPurchaseReportDG(PurchaseReportRequest request)
        {
            try
            {
                var response = await purchaseReportService.AddNewPurchaseReport(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<PurchaseReportView>>))]
        public async Task<IActionResult> GetListPurchaseReportDG()
        {
            try
            {
                var response = await purchaseReportService.GetListPurchaseReportDG();
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteDG(int Id)
        {
            try
            {
                var response = await purchaseReportService.DeleteOrderDG(Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }

        // đơn hàng ME


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> AddNewPurchaseReportME(PurchaseReportRequest request)
        {
            try
            {
                var response = await purchaseReportService.AddNewPurchaseReportMe(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<PurchaseReportView>>))]
        public async Task<IActionResult> GetListPurchaseReportME()
        {
            try
            {
                var response = await purchaseReportService.GetListPurchaseReportME();
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteME(int Id)
        {
            try
            {
                var response = await purchaseReportService.DeleteOrderME(Id);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> AddNewPurchaseReportMH(PurchaseReportRequest request)
        {
            try
            {
                var response = await purchaseReportService.AddNewPurchaseReportMH(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<PurchaseReportView>>))]
        public async Task<IActionResult> GetListPurchaseReportMH()
        {
            try
            {
                var response = await purchaseReportService.GetListPurchaseReportMH();
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteMH(int Id)
        {
            try
            {
                var response = await purchaseReportService.DeleteOrderMH(Id);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetOrderPurchase(PurchaseReportDataRequest request)
        {
            try
            {
                var data = await purchaseReportService.GetPurchaseReportData(request);
                return Ok(data);
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
        public async Task<IActionResult> GetListAccountId(PurchaseReportDataRequest request)
        {
            try
            {
                var data = purchaseReportService.GetListAccount(request);
                return Ok(data);
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
        public async Task<IActionResult> ExportPurchase(PurchaseReportDataRequest request)
        {
            try
            {

                var data = await purchaseReportService.GetPurchaseReportData(request);
                var time = DateTime.Now;
                var irow = 9;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "PurchaseReportAuc_Teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";

                    foreach (var item in data.Data)
                    {
                     
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.OrderCode;
                        try
                        {
                            workSheet.Cells[irow, 3].Hyperlink = new Uri(item.ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 4].Value = item.ProductTitle;
                        workSheet.Cells[irow, 5].Value = item.PaymentByString;
                        workSheet.Cells[irow, 6].Value = item.PaymentDateDisplay;
                        workSheet.Cells[irow, 7].Value = item.CancelByString;
                        workSheet.Cells[irow, 8].Value = item.CancelDateDisplay;
                        workSheet.Cells[irow, 9].Value = item.Price;
                        workSheet.Cells[irow, 10].Value = item.Tax;
                        workSheet.Cells[irow, 11].Value = item.ShippingFee;
                        workSheet.Cells[irow, 12].Value = item.Surcharge;
                        workSheet.Cells[irow, 13].Value = item.DebitAmount;
                        workSheet.Cells[irow, 14].Value = item.CreditAmount;
                        workSheet.Cells[irow, 15].Value = item.WarehouseString;
                        workSheet.Cells[irow, 16].Value = item.Fullname;
                        workSheet.Cells[irow, 17].Value = item.TypeDisplay;
                        irow += 1;
                    }

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));
              
                    workSheet.Cells["A9:Q" + (data.Data.Count + 9)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:Q" + (data.Data.Count + 9)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:Q" + (data.Data.Count + 9)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:Q" + (data.Data.Count + 9)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PurchaseReportAuc_Teamplate.xlsx");
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]

        public async Task<IActionResult> UpdateLock(int Id,bool? valueLock)
        {
            try
            {
                var response = await purchaseReportService.UpdateLock(Id, valueLock);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]

        public async Task<IActionResult> UnLock(int Id, bool? valueLock)
        {
            try
            {
                var response = await purchaseReportService.UpdateLock(Id, valueLock);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }

    }
}