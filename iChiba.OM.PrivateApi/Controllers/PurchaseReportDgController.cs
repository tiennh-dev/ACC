using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Request.PurchaseReport;
using iChiba.OM.PrivateApi.AppService.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace iChiba.OM.PrivateApi.Controllers
{
 
    public class PurchaseReportDgController : BaseController
    {
        private readonly IPurchaseReportAppService purchaseReportService;
        private readonly IHostingEnvironment environment;
        public PurchaseReportDgController(ILogger<PurchaseReportController> logger, IPurchaseReportAppService purchaseReportService, IHostingEnvironment environment) : base(logger)
        {
            this.purchaseReportService = purchaseReportService;
            this.environment = environment;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetPurchaseReportDg()
        {
            try
            {
                var response = await purchaseReportService.GetPurchaseReportDG();

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
        public async Task<IActionResult> GetPurchaseReportDatadg(PurchaseReportMercariRequest request)
        {
            try
            {
                var data = await purchaseReportService.GetPurchaseReportDataDG(request);
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
        public async Task<IActionResult> Export(PurchaseReportDataRequest request)
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

    }
}