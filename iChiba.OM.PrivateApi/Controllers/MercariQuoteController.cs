using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using Core.CustomException;
using iChiba.OM.Cache.Cache.Model;
using iChiba.OM.CustomException;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Model.Order;
using iChiba.OM.PrivateApi.AppModel.Model.Warehouse;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Request.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.CustomerWallet;
using iChiba.OM.PrivateApi.AppModel.Response.Order;
using iChiba.OM.PrivateApi.AppService.Implement;
using iChiba.OM.PrivateApi.AppService.Implement.Common;
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
    public class MercariQuoteController : BaseController
    {
        private readonly IOrderAppService orderAppService;
        private readonly ICustomerAppService customerAppService;
        private readonly OrderWorkflowAppService orderWorkflowAppService;
        private readonly ICustomerWalletAppService customerWalletAppService;
        private readonly IWarehouseAppService warehouseAppService;
        private readonly IProductBidClientInfoAppService bidClientCache;
        private readonly IHostingEnvironment environment;
        private readonly IOrderBuyForYouAppService orderBuyForYouAppService;

        public MercariQuoteController(ILogger<QuoteAuctionController> logger,
            IOrderAppService orderAppService,
            ICustomerAppService customerAppService,
            OrderWorkflowAppService orderWorkflowAppService,
            ICustomerWalletAppService customerWalletAppService,
            IProductBidClientInfoAppService bidClientCache,
            IHostingEnvironment environment,
            IOrderBuyForYouAppService orderBuyForYouAppService,
            IWarehouseAppService warehouseAppService, AccessClient accessClient, AppConfig appConfig) : base(logger, accessClient, appConfig)
        {
            this.orderAppService = orderAppService;
            this.orderBuyForYouAppService = orderBuyForYouAppService;
            this.customerAppService = customerAppService;
            this.orderWorkflowAppService = orderWorkflowAppService;
            this.customerWalletAppService = customerWalletAppService;
            this.warehouseAppService = warehouseAppService;
            this.bidClientCache = bidClientCache;
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
                    var response = await orderAppService.GetListJTableQuoteMercari(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
                else
                {
                    var response = await orderAppService.GetListJTableMercariBySale(appserviceRequest);
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
                request.PreState = PurchaseConfig.DA_MUA_HANG;
                request.OrderType = Model.OrderRefType.MERCARI;
                OrderListResponse orderLists = null;
                if (request.OrderId != null)
                {
                    int[] IdOrder = request.OrderId.Distinct().ToArray();
                    orderLists = await orderAppService.GetListOrderById(IdOrder);
                }
                else
                {
                    orderLists = await orderAppService.ExportPurchaseAuction(request);
                }
                if (orderLists == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                //var stream = new MemoryStream();
                var path = Path.Combine(environment.WebRootPath, "PuchaseAution.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "File Tổng Hợp Đơn Đấu Giá " + String.Format("{0:dd/MM/yyyy}", time);


                    foreach (var item in orderLists.Data)
                    {
                        var totaltemp = item.ShippingFee != null ? (item.Total + item.ShippingFee) : item.Total;
                        var tempPrice = totaltemp * item.ExchangeRate;
                        var hyperlink = String.Format("https://page.auctions.yahoo.co.jp/jp/auction/" + item.Link);
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        workSheet.Cells[irow, 3].Hyperlink = new Uri(hyperlink, UriKind.Absolute);
                        workSheet.Cells[irow, 4].Value = "";
                        workSheet.Cells[irow, 5].Value = item.ProductType;
                        workSheet.Cells[irow, 6].Value = item.ProductOrigin;
                        workSheet.Cells[irow, 7].Value = item.BarCode;
                        workSheet.Cells[irow, 8].Value = item.Amount;
                        workSheet.Cells[irow, 9].Value = item.Total;
                        workSheet.Cells[irow, 10].Value = item.ShippingFee;
                        workSheet.Cells[irow, 11].Value = totaltemp;
                        workSheet.Cells[irow, 12].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 13].Value = "";
                        workSheet.Cells[irow, 14].Value = item.BuyFee;
                        workSheet.Cells[irow, 15].Value = item.ShippingUnitGlobal;
                        workSheet.Cells[irow, 16].Value = tempPrice;
                        workSheet.Cells[irow, 17].Value = item.BidAccount;
                        workSheet.Cells[irow, 18].Value = item.FullName;
                        workSheet.Cells[irow, 19].Value = item.EmpployeeSupport;
                        workSheet.Cells[irow, 20].Value = item.Tracking;
                        workSheet.Cells[irow, 21].Value = "";
                        workSheet.Cells[irow, 22].Value = "";
                        workSheet.Cells[irow, 23].Value = "";
                        irow += 1;
                    }
                    workSheet.Cells["C8:C" + (orderLists.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (orderLists.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    workSheet.Cells["A" + (orderLists.Data.Count + 8) + ":V" + (orderLists.Data.Count + 8)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + (orderLists.Data.Count + 8) + ":V" + (orderLists.Data.Count + 8)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    workSheet.Cells[orderLists.Data.Count + 8, 7].Formula = "SUM(G8:G" + (orderLists.Data.Count + 7) + ")";
                    workSheet.Cells[orderLists.Data.Count + 8, 8].Formula = "SUM(H8:H" + (orderLists.Data.Count + 7) + ")";
                    workSheet.Cells[orderLists.Data.Count + 8, 9].Formula = "SUM(I8:I" + (orderLists.Data.Count + 7) + ")";
                    workSheet.Cells[orderLists.Data.Count + 8, 10].Formula = "SUM(J8:J" + (orderLists.Data.Count + 7) + ")";
                    workSheet.Cells[orderLists.Data.Count + 8, 14].Formula = "SUM(N8:N" + (orderLists.Data.Count + 7) + ")";
                    workSheet.Cells["A7:W7"].Style.Font.Size = 11;
                    workSheet.Cells["A7:W7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A7:W7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A7:W" + (orderLists.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:W" + (orderLists.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:W" + (orderLists.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:W" + (orderLists.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));
                    workSheet.Cells["A7:W7"].Style.Font.Bold = true;


                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PuchaseAution.xlsx");
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


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<ShippingInfo>))]
        public async Task<IActionResult> GetShippingInfo(GetShippingInfoRequest request)
        {
            try
            {
                var response = await orderAppService.GetShippingInfo(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> ImportFile()
        {
            try
            {
                List<OrderList> data = new List<OrderList>();
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = environment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            int i;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            int rows = worksheet.Dimension.Rows;
                            int columns = worksheet.Dimension.Columns;
                            for (i = 2; i <= rows; i++)
                            {

                                double weight = 0;
                                try
                                {
                                    try
                                    {
                                        var weightCheckNull = worksheet.Cells[i, 3].Value;
                                        if (weightCheckNull == null) continue;
                                        weight = Convert.ToDouble(worksheet.Cells[i, 3].Value.ToString().TrimStart().TrimEnd());
                                    }
                                    catch (Exception)
                                    { }
                                    data.Add(new OrderList
                                    {
                                        Code = worksheet.Cells[i, 1].Value.ToString(),
                                        CustomerIdName = worksheet.Cells[i, 2].Value.ToString(),
                                        Weight = Convert.ToInt32(weight * 1000),
                                    });
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }

                }
                var obj = new { data, file.FileName, ImportType = "Trọng lượng" };
                return Ok(obj);
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
        public async Task<IActionResult> UpdateFile(ImportFileRequest request)
        {
            try
            {
                var path = Path.Combine(environment.WebRootPath, "Upload/" + request.FileName);
                FileInfo fileInfo = new FileInfo(path);
                List<OrderList> orderList = new List<OrderList>();
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    int i;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rows = worksheet.Dimension.Rows;
                    for (i = 2; i <= rows; i++)
                    {
                        double weight = 0;
                        try
                        {
                            try
                            {
                                var weightCheckNull = worksheet.Cells[i, 3].Value;
                                if (weightCheckNull == null) continue;
                                weight = Convert.ToDouble(worksheet.Cells[i, 3].Value.ToString().TrimStart().TrimEnd());
                            }
                            catch (Exception)
                            { }
                            orderList.Add(new OrderList
                            {
                                Code = worksheet.Cells[i, 1].Value.ToString().TrimStart(),
                                CustomerIdName = worksheet.Cells[i, 2].Value.ToString(),
                                Weight = Convert.ToInt32(weight * 1000),
                            });
                        }
                        catch (Exception)
                        { }
                    }
                }
                var response = await orderAppService.UpdateFile(orderList);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> ImportDeliveryFee()
        {
            try
            {
                List<OrderList> data = new List<OrderList>();
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = environment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            int i;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            int rows = worksheet.Dimension.Rows;
                            int columns = worksheet.Dimension.Columns;
                            for (i = 2; i <= rows; i++)
                            {
                                long? deliveryFee = null;
                                try
                                {
                                    try
                                    {
                                        var checkNullDeliveryFee = worksheet.Cells[i, 5].Value;
                                        if (checkNullDeliveryFee == null) continue;
                                        bool isNumberic = Regex.IsMatch(worksheet.Cells[i, 5].Value.ToString(), @"^\d+$");
                                        if (!isNumberic) continue;
                                        deliveryFee = Convert.ToInt64(worksheet.Cells[i, 5].Value.ToString());
                                    }
                                    catch (Exception) { }

                                    data.Add(new OrderList
                                    {
                                        Code = worksheet.Cells[i, 1].Value.ToString(),
                                        DeliveryFee = deliveryFee
                                    });
                                }
                                catch (Exception)
                                { }
                            }
                        }
                    }

                }
                var obj = new { data, file.FileName, ImportType = "DeliveryFee" };
                return Ok(obj);
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
        public async Task<IActionResult> UpdateDeliveryFee(ImportFileRequest request)
        {
            try
            {
                var path = Path.Combine(environment.WebRootPath, "Upload/" + request.FileName);
                FileInfo fileInfo = new FileInfo(path);
                List<OrderList> orderList = new List<OrderList>();
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    int i;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rows = worksheet.Dimension.Rows;
                    for (i = 2; i <= rows; i++)
                    {
                        long? deliveryFee = null;
                        try
                        {
                            try
                            {
                                var checkNullDeliveryFee = worksheet.Cells[i, 5].Value;
                                if (checkNullDeliveryFee == null) continue;
                                bool isNumberic = Regex.IsMatch(worksheet.Cells[i, 5].Value.ToString(), @"^\d+$");
                                if (!isNumberic) continue;
                                deliveryFee = Convert.ToInt64(worksheet.Cells[i, 5].Value.ToString());
                            }
                            catch (Exception) { }

                            orderList.Add(new OrderList
                            {
                                Code = worksheet.Cells[i, 1].Value.ToString(),
                                DeliveryFee = deliveryFee
                            });
                        }
                        catch (Exception)
                        { }
                    }
                }
                var response = await orderAppService.UpdateDeliveryFee(orderList);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }



        private string GetRangeText(ExcelWorksheet wks, int row, int col)
        {
            var cell = wks.Cells[row, col];
            if (cell.Merge)
            {
                var mergedId = wks.MergedCells[row, col];
                return wks.Cells[mergedId].First().Value.ToString();
            }
            else
            {
                return cell.Value.ToString();
            }
        }

        //private string[] GetWrapText(ExcelWorksheet wks, int row, int col)
        //{
        //    var cell = wks.Cells[row, col];
        //    string[] arrStr = cell.RichText.Text.Split("\n");

        //}


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> ImportTracking()
        {
            try
            {
                List<OrderList> data = new List<OrderList>();
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = environment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            int i;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            int rows = worksheet.Dimension.Rows;
                            int columns = worksheet.Dimension.Columns;
                            for (i = 2; i <= rows; i++)
                            {
                                try
                                {
                                    //try
                                    //{
                                    //    bool isNumberic = Regex.IsMatch(worksheet.Cells[i, 4].Value.ToString(), @"^\d+$");
                                    //    if (!isNumberic) continue;
                                    //}
                                    //catch (Exception) { }

                                    data.Add(new OrderList
                                    {
                                        Code = worksheet.Cells[i, 1].Value.ToString(),
                                        ProductLink = worksheet.Cells[i, 2].Value != null ? worksheet.Cells[i, 2].Value.ToString() : "",
                                        CustomerIdName = worksheet.Cells[i, 3].Value != null ? worksheet.Cells[i, 3].Value.ToString() : "",
                                        Tracking = GetRangeText(worksheet, i, 4),
                                    });
                                }
                                catch (Exception)
                                { }
                            }
                        }
                    }

                }
                var obj = new { data, file.FileName, ImportType = "Tracking" };
                return Ok(obj);
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
        public async Task<IActionResult> UpdateTrackingExcel(ImportFileRequest request)
        {
            try
            {
                var path = Path.Combine(environment.WebRootPath, "Upload/" + request.FileName);
                FileInfo fileInfo = new FileInfo(path);
                List<OrderList> orderList = new List<OrderList>();
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    int i;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rows = worksheet.Dimension.Rows;
                    for (i = 2; i <= rows; i++)
                    {
                        try
                        {

                            //try
                            //{
                            //    bool isNumberic = Regex.IsMatch(worksheet.Cells[i, 4].Value.ToString(), @"^\d+$");
                            //    if (!isNumberic) continue;
                            //}
                            //catch (Exception) { }
                            orderList.Add(new OrderList
                            {
                                Code = worksheet.Cells[i, 1].Value.ToString(),
                                ProductLink = worksheet.Cells[i, 2].Value != null ? worksheet.Cells[i, 2].Value.ToString() : "",
                                CustomerIdName = worksheet.Cells[i, 3].Value != null ? worksheet.Cells[i, 3].Value.ToString() : "",
                                Tracking = GetRangeText(worksheet, i, 4),
                            });
                        }
                        catch (Exception)
                        { }
                    }
                }
                var response = await orderAppService.UpdateTracking(orderList);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> ImportSurcharge()
        {
            try
            {
                List<OrderList> data = new List<OrderList>();
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = environment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            int i;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            int rows = worksheet.Dimension.Rows;
                            int columns = worksheet.Dimension.Columns;
                            for (i = 2; i <= rows; i++)
                            {
                                try
                                {
                                    long? surcharge = null;
                                    string[] arStr = null;
                                    try
                                    {
                                        surcharge = Convert.ToInt64(worksheet.Cells[i, 2].Value.ToString());
                                    }
                                    catch (Exception) { }
                                    try
                                    {
                                        var cell = worksheet.Cells[i, 1];
                                        if (cell.Style.WrapText)
                                        {
                                            arStr = cell.RichText.Text.Split("\n");
                                            foreach (var item in arStr)
                                            {
                                                data.Add(new OrderList
                                                {
                                                    Code = item.Trim(),
                                                    Surcharge = surcharge,
                                                });
                                            }
                                        }
                                        else
                                        {
                                            data.Add(new OrderList
                                            {
                                                Code = worksheet.Cells[i, 1].Value.ToString().Trim(),
                                                Surcharge = surcharge,
                                            });
                                        }
                                    }
                                    catch (Exception) { }
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, ex.Message);
                                }
                            }
                        }
                    }

                }
                var obj = new { data, file.FileName, ImportType = "Surcharge" };
                return Ok(obj);
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
        public async Task<IActionResult> UpdateSurcharge(ImportFileRequest request)
        {
            try
            {
                var path = Path.Combine(environment.WebRootPath, "Upload/" + request.FileName);
                FileInfo fileInfo = new FileInfo(path);
                List<OrderList> orderList = new List<OrderList>();
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    int i;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rows = worksheet.Dimension.Rows;
                    for (i = 2; i <= rows; i++)
                    {
                        try
                        {
                            long? surcharge = null;
                            string[] arStr = null;
                            try
                            {
                                surcharge = Convert.ToInt64(worksheet.Cells[i, 2].Value.ToString());
                            }
                            catch (Exception) { }
                            try
                            {
                                var cell = worksheet.Cells[i, 1];
                                if (cell.Style.WrapText)
                                {
                                    arStr = cell.RichText.Text.Split("\n");
                                    foreach (var item in arStr)
                                    {
                                        orderList.Add(new OrderList
                                        {
                                            Code = item.Trim(),
                                            Surcharge = surcharge,
                                        });
                                    }
                                }
                                else
                                {
                                    orderList.Add(new OrderList
                                    {
                                        Code = worksheet.Cells[i, 1].Value.ToString().Trim(),
                                        Surcharge = surcharge,
                                    });
                                }
                            }
                            catch (Exception) { }
                        }
                        catch (Exception)
                        { }
                    }
                }
                var response = await orderAppService.UpdateSurchargeFromExcel(orderList);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderdetailListResponse))]
        public async Task<IActionResult> GetOrderDetail(OrderdetailGetByOrderIdRequest request)
        {
            try
            {
                var response = await orderBuyForYouAppService.GetListOrderDetail(request);

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
        public async Task<IActionResult> QuoteAuctionUpdate(PurchaseAuctionQuoteUpdateRequest request)
        {
            try
            {
                var response = await orderAppService.QuoteAuctionUpdate(request);

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
        public async Task<IActionResult> UpdateShippingFree(OrderUpdateShippingFreeRequest request)
        {
            try
            {
                var response = await orderAppService.UpdateShippingFree(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailPaymentResponse))]
        public async Task<IActionResult> GetOrderDetailPayment(int id, string accountId)
        {
            try
            {
                var response = await orderAppService.GetOrderDetailPayment(id, accountId, "ORDER");

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }


        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> GetListOrder(OrderListRequestDialog request)
        {
            try
            {
                var data = await orderAppService.GetListOrder(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<OrderList>>))]
        public async Task<IActionResult> GetOrderPayments(OrderPaymentsRequest request)
        {
            var response = await orderAppService.GetOrderPayments(request);

            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListCustomer()
        {
            try
            {
                var response = await orderAppService.GetListCustomer();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<WarehouseList>>))]
        public async Task<IActionResult> GetAllWarehouseActive()
        {
            try
            {
                var response = await warehouseAppService.GetAllWarehouseActive();

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
        public async Task<IActionResult> Update(OrderUpdateRequest request)
        {
            try
            {
                var response = await orderAppService.Update(request);

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
        public async Task<IActionResult> UpdateTracking(OrderUpdateTrackingRequest request)
        {
            try
            {
                var response = await orderAppService.UpdateTracking(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("{orderId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<OrderApp>))]
        public async Task<IActionResult> GetDetail(int orderId)
        {
            try
            {
                var response = await orderAppService.GetDetail(orderId);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> RequestCancelOrder(OrderWorkflowRequest request)
        {
            try
            {
                var requestWf = new OrderWVModel()
                {
                    Id = request.Id,
                    Message = request.Message,
                    Action = "YÊU CẦU HUỶ ĐƠN"
                };
                var response = await orderWorkflowAppService.RequestCancelOrder(requestWf);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> PaymentOrder(OrderWorkflowRequest request)
        {
            try
            {
                var requestWf = new OrderWVModel()
                {
                    Id = request.Id,
                    Message = request.Message,
                    Action = "ĐỀ NGHỊ THANH TOÁN"
                };
                var response = await orderWorkflowAppService.PaymentOrder(requestWf);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> PaymentOrderMulti(OrderWorkflowByRequest request)
        {
            try
            {
                var response = new WorkflowTriggerInfo();
                int[] arrId = request.Id.Distinct().ToArray();
                foreach (var item in arrId)
                {
                    var requestWf = new OrderWVModel()
                    {
                        Id = item,
                        Message = request.Message,
                        Action = "ĐỀ NGHỊ THANH TOÁN"
                    };
                    response = await orderWorkflowAppService.PaymentOrder(requestWf);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<Model.Customer>))]
        public async Task<IActionResult> GetCustomerInfo(string accountId)
        {
            try
            {
                var response = await customerAppService.GetCustomerInfo(accountId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMemberShipInfo(string accountId)
        {
            var response = await orderAppService.GetLevelInfo(accountId);

            return Ok(response);
        }
        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerWalletListResponse))]
        public async Task<IActionResult> GetWalletInfo(string accountId)
        {
            try
            {
                var data = await customerWalletAppService.GetByAccountId(accountId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }
        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<Employees>))]
        public async Task<IActionResult> GetEmpInfo(string accountId)
        {
            try
            {
                var data = await orderAppService.GetEmpInfo(accountId);
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
        public async Task<IActionResult> UpdateYCComplete(ReceivedProductRequest request)
        {
            try
            {
                var response = await orderAppService.ReceivedProduct(request);

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