using Core.AppModel.Response;
using Core.Common.JTable;
using Core.CustomException;
using iChiba.LocalizationCommon;
using iChiba.OM.CustomException;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Model.Customer;
using iChiba.OM.PrivateApi.AppModel.Model.Order;
using iChiba.OM.PrivateApi.AppModel.Model.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Model.Warehouse;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Request.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Request.Payment;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.CustomerWallet;
using iChiba.OM.PrivateApi.AppModel.Response.Order;
using iChiba.OM.PrivateApi.AppModel.Response.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Response.PaymentAccount;
using iChiba.OM.PrivateApi.AppService.Implement;
using iChiba.OM.PrivateApi.AppService.Implement.Common;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class PurchaseBuyForYouController : BaseController
    {
        private readonly IOrderBuyForYouAppService orderBuyForYouAppService;
        private readonly IWalletTransAppService walletTransappService;
        private readonly ILevelAppService levelappservice;
        private readonly IWarehouseAppService warehouseAppService;
        private readonly OrderWorkflowAppService orderWorkflowAppService;
        private readonly IServiceProvider serviceProvider;
        private readonly IWalletAppService walletappservice;
        private readonly ICustomerAppService customerappservice;
        private readonly ICustomerAppService customerAppService;
        private readonly IOrderAppService orderAppService;
        private readonly ICustomerWalletAppService customerWalletAppService;
        private readonly IPaymentAccountAppService paymentAccountAppService;
        private readonly IHostingEnvironment environment;

        public PurchaseBuyForYouController(ILogger<PurchaseBuyForYouController> logger,
            IOrderBuyForYouAppService orderBuyForYouAppService,
            IWalletTransAppService walletTransappService,
            IWalletAppService walletappservice,
            ICustomerAppService customerappservice,
            ILevelAppService levelappservice,
            IWarehouseAppService warehouseAppService,
            ICustomerAppService customerAppService,
            IOrderAppService orderAppService,
            ICustomerWalletAppService customerWalletAppService,
            IPaymentAccountAppService paymentAccountAppService,
            IHostingEnvironment environment,
            OrderWorkflowAppService orderWorkflowAppService,
            IServiceProvider serviceProvider) : base(logger)
        {
            this.orderBuyForYouAppService = orderBuyForYouAppService;
            this.walletTransappService = walletTransappService;
            this.levelappservice = levelappservice;
            this.warehouseAppService = warehouseAppService;
            this.orderWorkflowAppService = orderWorkflowAppService;
            this.serviceProvider = serviceProvider;
            this.walletappservice = walletappservice;
            this.customerappservice = customerappservice;
            this.customerAppService = customerAppService;
            this.orderAppService = orderAppService;
            this.customerWalletAppService = customerWalletAppService;
            this.paymentAccountAppService = paymentAccountAppService;
            this.environment = environment;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderBuyForYouListJTableModel))]
        public async Task<IActionResult> GetJTable(OrderBuyForYouListJTableModel request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                var appserviceRequest = request.ToModel();
                appserviceRequest.PreState = PurchaseConfig.DA_MUA_HANG;
                var response = await orderBuyForYouAppService.GetListJTable(appserviceRequest);
                var responseJTable = JTableHelper.JObjectData(response.Data.ToList(),
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Export(OrderBuyForYouListRequest request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                request.PreState = PurchaseConfig.DA_MUA_HANG;
                OrderListResponse orderLists = null;
                if (request.OrderId != null)
                {
                    int[] IdOrder = request.OrderId.Distinct().ToArray();
                    orderLists = await orderBuyForYouAppService.GetListOrderByIdForBuyForYou(IdOrder);
                }
                else
                {
                    orderLists = await orderBuyForYouAppService.Export(request);
                }
                if (orderLists == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "PuchaseAution.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "Đơn đã mua MH " + String.Format("{0:dd/MM/yyyy}", time);


                    foreach (var item in orderLists.Data)
                    {
                        var noteEx = "";
                        var total = item.Price * item.Amount;
                        var totaltemp = item.ShippingFee != null ? (total + item.ShippingFee) : total;
                        try
                        {

                            if (!string.IsNullOrEmpty(item.ProAttribute) && !string.IsNullOrEmpty(item.Note))
                            {
                                noteEx = item.ProAttribute.ToString() + "," + item.Note;
                            }
                            else if (!string.IsNullOrEmpty(item.ProAttribute) && string.IsNullOrEmpty(item.Note))
                            {
                                noteEx = item.ProAttribute.ToString();
                            }
                            else if (string.IsNullOrEmpty(item.ProAttribute) && !string.IsNullOrEmpty(item.Note))
                            {
                                noteEx = item.Note;
                            }
                            else
                            {
                                noteEx = "";
                            }
                        }
                        catch (Exception) { }
                        var tempPrice = totaltemp * item.ExchangeRate;
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Hyperlink = new Uri(item.Link, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 4].Value = noteEx;
                        workSheet.Cells[irow, 5].Value = item.ProductType;
                        workSheet.Cells[irow, 6].Value = item.ProductOrigin;
                        workSheet.Cells[irow, 7].Value = item.BarCode;
                        workSheet.Cells[irow, 8].Value = item.Amount;
                        workSheet.Cells[irow, 9].Value = total;
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
                    workSheet.Cells["A" + (orderLists.Data.Count + 8) + ":W" + (orderLists.Data.Count + 8)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + (orderLists.Data.Count + 8) + ":W" + (orderLists.Data.Count + 8)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateSurcharge(OrderUpdateSurchargeRequest request)
        {
            try
            {
                var response = await orderAppService.UpdateSurcharge(request);

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

                                    try
                                    {
                                        bool isNumberic = Regex.IsMatch(worksheet.Cells[i, 4].Value.ToString(), @"^\d+$");
                                        if (!isNumberic) continue;
                                    }
                                    catch (Exception) { }
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

                            try
                            {
                                bool isNumberic = Regex.IsMatch(worksheet.Cells[i, 4].Value.ToString(), @"^\d+$");
                                if (!isNumberic) continue;
                            }
                            catch (Exception) { }
                            orderList.Add(new OrderList
                            {
                                Code = worksheet.Cells[i, 1].Value.ToString().Trim(),
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



        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<OrderDetailApp>))]
        public async Task<IActionResult> GetOrderDetail(OrderdetailGetByOrderIdRequest request)
        {
            try
            {
                var response = await orderAppService.GetListOrderDetail(request.OrderId);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaymentListResponse))]
        public async Task<IActionResult> GetOrderDetailPayment(int id, string accountId)
        {
            try
            {
                var refType = "ORDER";
                var response = await orderBuyForYouAppService.GetOrderDetailPayment(id, accountId, refType);

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
        public async Task<IActionResult> GetListCustomer()
        {
            try
            {
                var response = await orderBuyForYouAppService.GetListCustomer();
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
            var response = await customerappservice.GetListTopCustomer(request);
            return Ok(response);
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailUpdateResponse))]
        public async Task<IActionResult> UpdateDetail(OrderDetailUpdateRequest request)
        {
            try
            {
                var response = await orderBuyForYouAppService.UpdateDetail(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailUpdateResponse))]
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailUpdateResponse))]
        public async Task<IActionResult> UpdateStatusDetail(OrderDetailUpdateRequest request)
        {
            try
            {
                var response = await orderBuyForYouAppService.UpdateSstatusDetail(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailUpdateResponse))]
        public async Task<IActionResult> Add(PaymentAddRequest request)
        {
            try
            {
                var response = await orderBuyForYouAppService.Add(request);
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
                var response = await orderBuyForYouAppService.GetDetail(orderId);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateAmountOrderDetail(int id, int? amount)
        {
            try
            {
                var response = await orderBuyForYouAppService.UpdateAmountOrderDetail(id, amount);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdatePriceOrderDetail(int id, int? price)
        {
            try
            {
                var response = await orderBuyForYouAppService.UpdatePriceOrderDetail(id, price);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateTaxOrderDetail(int id, int? tax)
        {
            try
            {
                var response = await orderBuyForYouAppService.UpdateTaxOrderDetail(id, tax);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailUpdateResponse))]
        public async Task<IActionResult> GetwalletByaccountId(string accountId)
        {
            try
            {
                var response = await walletTransappService.GetWalletByAccountId(accountId);
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
        public async Task<IActionResult> Getpaymentprofile(string accountId)
        {
            try
            {
                var response = levelappservice.GetAuctionFeeCancel(accountId);
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
        public async Task<IActionResult> getcustomerbyaccount(string accountId)
        {
            try
            {
                var data = customerappservice.GetcustomerbyaccountId(accountId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPost("{walletId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetWalletbyId(string walletId)
        {
            try
            {
                var response = await walletappservice.GetWalletbyId(walletId);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<Model.OrderMessage>>))]
        public async Task<IActionResult> GetMessages(OrderMessageGetByOrderIdRequest request)
        {
            try
            {
                var response = await orderBuyForYouAppService.GetMessages(request);

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
                var response = await orderBuyForYouAppService.Update(request);

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
                var response = await orderBuyForYouAppService.UpdateTracking(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateOrderNumberBuyForYou(OrderUpdateOrderNumberRequest request)
        {
            try
            {
                var data = await orderAppService.UpdateOrderNumberPurchaseBuyForYou(request);
                return Ok(data);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateWareHouse(OrderUpdateWarehouseRequest request)
        {
            try
            {
                var data = await orderAppService.UpdateWareHousePurchaseBuyForYou(request);
                return Ok(data);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaymentAccountResponse))]
        public async Task<IActionResult> GetAllPaymentAccount()
        {
            try
            {
                var data = await paymentAccountAppService.GetAllPaymentAccount();
                return Ok(data);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdatePaymentAccount(OrderUpdatePaymentAccountRequest request)
        {
            try
            {
                var data = await orderAppService.UpdatePaymentAccount(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<Customer>))]
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

        /// <summary>
        /// Y/C mua hàng theo lô
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> BuyProductMany(OrderWorkflowByRequest request)
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
                        Action = "MUA HÀNG"
                    };
                    response = await orderWorkflowAppService.BuyProduct(requestWf);
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

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<int>>))]
        public async Task<IActionResult> SplitOrderBought(OrderSplitRequest request)
        {
            try
            {
                var response = await orderBuyForYouAppService.SplitOrder(request);

                if (response.Status)
                {
                    var requestWfSplitOrdersWaitForBuy = response.Data.Select(item => new OrderWVModel()
                    {
                        Id = item,
                        Message = "Hệ thống tách đơn, tự động duyệt chờ mua hàng",
                        Action = "TỰ ĐỘNG DUYỆT CHỜ MUA HÀNG"
                    }).ToList();
                    var requestWf = new OrderWVModel()
                    {
                        Id = request.OrderId,
                        Action = "Y/C HUỶ ĐƠN",
                        Message = "Hệ thống tách đơn, tự động huỷ đơn"
                    };

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var orderWorkflowAppServiceTemp = scope.ServiceProvider.GetService<OrderWorkflowAppService>();

                        requestWfSplitOrdersWaitForBuy.ForEach(item =>
                        {
                            var result = orderWorkflowAppServiceTemp.AutoApproveWaitForBuyProduct(item).Result;

                            if (!result.Succeeded)
                            {
                                throw new ErrorCodeException(ErrorCodeDefine.ORDER_SPLIT_CANT_AUTO_APPROVE_WAIT_FOR_BUY_PRODUCT);
                            }
                        });
                    }

                    var requestWfSplitOrdersBought = response.Data.Select(item => new OrderWVModel()
                    {
                        Id = item,
                        Message = "Hệ thống tách đơn, tự động duyệt mua hàng",
                        Action = "TỰ ĐỘNG DUYỆT MUA HÀNG"
                    }).ToList();

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var orderWorkflowAppServiceTemp = scope.ServiceProvider.GetService<OrderWorkflowAppService>();

                        requestWfSplitOrdersBought.ForEach(item =>
                        {
                            var result = orderWorkflowAppServiceTemp.BuyProduct(item).Result;

                            if (!result.Succeeded)
                            {
                                throw new ErrorCodeException(ErrorCodeDefine.ORDER_SPLIT_CANT_AUTO_APPROVE_BUY_PRODUCT);
                            }
                        });
                    }

                    var cancelResponse = await orderWorkflowAppService.RequestCancelOrder(requestWf);

                    if (!cancelResponse.Succeeded)
                    {
                        throw new ErrorCodeException(ErrorCodeDefine.ORDER_SPLIT_CANT_AUTO_CANCEL_ORDER_BY_FOR_YOU);
                    }
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateNoteOrDerDetail(int IdOrderDetail, string note)
        {
            try
            {
                var response = await orderAppService.UpdateNoteOrderDetail(IdOrderDetail, note);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }

        #region Workflow

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
        public async Task<IActionResult> BuyProduct(OrderWorkflowRequest request)
        {
            try
            {
                var requestWf = new OrderWVModel()
                {
                    Id = request.Id,
                    Message = request.Message,
                    Action = "MUA HÀNG"
                };
                var response = await orderWorkflowAppService.BuyProduct(requestWf);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        #endregion Workflow
    }
}