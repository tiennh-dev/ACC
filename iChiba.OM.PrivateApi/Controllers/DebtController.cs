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
using iChiba.OM.CustomException;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.debt;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.Order;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using Ichiba.IS4.Api.Driver;
using iChibaShopping.Core.AppService.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class DebtController : BaseController
    {
        private readonly IDebtAppService debtappservice;
        private readonly IOrderAppService orderappservice;
        private readonly IHostingEnvironment environment;
        private readonly IOrderBuyForYouAppService orderBuyForYouAppService;
        private readonly ICurrentContext currentContext;
        private readonly IDepositsAppService depositsAppService;

        public DebtController(ILogger<OrderController> logger, IOrderAppService orderappservice,
            IHostingEnvironment environment,
            ICurrentContext currentContext,
              IDepositsAppService depositsAppService,
            IOrderBuyForYouAppService orderBuyForYouAppService,
            IDebtAppService debtappservice, AccessClient accessClient, AppConfig appConfig) : base(logger, accessClient, appConfig)
        {
            this.debtappservice = debtappservice;
            this.depositsAppService = depositsAppService;
            this.orderappservice = orderappservice;
            this.environment = environment;
            this.orderBuyForYouAppService = orderBuyForYouAppService;
            this.currentContext = currentContext;
        }



        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetJTable(CustomerListJTableModel request)
        {
            try
            {
                //var keyword = request.Search.Value;
                //bổ sung tìm kiếm nhanh
                var appserviceRequest = request.ToModel();
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var response = await debtappservice.GetList(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);
                    return Ok(responseJTable);
                }
                else
                {
                    var response = await debtappservice.GetListByCare(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetTableAucPre(OrderListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                int[] orderStatus = new int[]
                {
                    (int)OrderStatus.CHO_XU_LY
                };
                appserviceRequest.Status = orderStatus;
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var response = await orderappservice.GetListPre(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                        request.Draw,
                        response.Total);

                    return Ok(responseJTable);
                }
                else
                {
                    var response = await orderappservice.GetListPreBuySale(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> GetJTableWaitingDepositeAuc(AucPreListRequest request)
        {
            try
            {
                int[] orderStatus = new int[]
                {
                    (int)OrderStatus.TAM_UNG,
                };
                request.Status = orderStatus;
                var data = await debtappservice.GetWaitingDepositeAuc(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> GetJTableWaitingDepositeBuyForYou(AucPreListRequest request)
        {
            try
            {
                int[] orderStatus = new int[]
                {
                    (int)OrderStatus.TAM_UNG,
                };
                request.Status = orderStatus;
                var data = await debtappservice.GetWaitingDepositeBuyForYou(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> GetJTableWaitingDepositeMer(AucPreListRequest request)
        {
            try
            {
                int[] orderStatus = new int[]
                {
                    (int)OrderStatus.TAM_UNG,
                };
               
                request.Status = orderStatus;
                var data = await debtappservice.GetWaitingDepositeMER(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderListResponse))]
        public async Task<IActionResult> GetJTableDebtWaitPurchase(AucPreListRequest request)
        {
            try
            {
                int[] orderStatus = new int[]
                {
                    (int)OrderStatus.MUA_HANG
                };
                request.Status = orderStatus;
                var response = await debtappservice.GetWaitingPurchase(request);

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
        public async Task<IActionResult> GetJTablePurchased(AucPreListRequest request)
        {
            try
            {
                string[] state = new string[] { AutionConfig.DA_MUA_HANG };
                request.State = state;
                var response = await debtappservice.GetWaitingPurchase(request);

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
        public async Task<IActionResult> GetJTableWaitPayment(AucPreListRequest request)
        {
            try
            {
                string[] arrString = new string[]
                   {
                        OrderState.CHO_TT_DON_HANG
                   };
                request.State = arrString;
                var response = await debtappservice.GetWaitingPurchase(request);

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
        public async Task<IActionResult> GetJTablePaid(AucPreListRequest request)
        {
            try
            {
                string[] arrString = new string[]
                 {
                         OrderPayment.DA_THANH_TOAN
                 };
                request.State = arrString;
                var response = await debtappservice.GetWaitingPurchase(request);

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
        public async Task<IActionResult> GetJTableDeliverd(AucPreListRequest request)
        {
            try
            {
                 int[] arrString = new int[]
                  {
                           (int)OrderStatus.DA_GIAO_HANG
                  };
                request.Status = arrString;
                var response = await debtappservice.GetWaitingPurchase(request);
               

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
        public async Task<IActionResult> ExportAucPre(OrderListRequest request)
        {
            try
            {
                OrderListResponse data = null;
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    data = await orderappservice.ExportOrderProcessing(request);
                }
                else
                {
                    data = await orderappservice.ExportOrderProcessingBuySale(request);
                }

                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "auc-pre_teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    foreach (var item in data.Data)
                    {
                        var ProductLink = "https://page.auctions.yahoo.co.jp/jp/auction/" + item.ProductLink;
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Hyperlink = new Uri(ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 4].Value = item.ProductType == "Chọn" ? "" : item.ProductType;
                        workSheet.Cells[irow, 5].Value = item.ProductOrigin == "Chọn" ? "" : item.ProductOrigin;
                        workSheet.Cells[irow, 6].Value = item.BidAccount;
                        workSheet.Cells[irow, 7].Value = item.Price;
                        workSheet.Cells[irow, 8].Value = item.PriceBuyTax;
                        workSheet.Cells[irow, 9].Value = item.ShippingFee;
                        workSheet.Cells[irow, 10].Value = item.Warehouse == "Chọn kho" ? "" : item.Warehouse;
                        workSheet.Cells[irow, 11].Value = item.OrderDateDetailDisplay;
                        workSheet.Cells[irow, 12].Value = item.FullName;
                        workSheet.Cells[irow, 13].Value = item.EmpployeeSupport;

                        irow += 1;

                    }

                    workSheet.Cells["A8:M" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:M" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:M" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:M" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "auc-pre_teamplate.xlsx");
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
        public async Task<IActionResult> ExportWaitDeposite(OrderListRequest request)
        {
            try
            {
                OrderListResponse data = null;
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    data = await orderappservice.ExportAllOrderAution(request);
                }
                else
                {
                    data = await orderappservice.ExportOrderAutionBySale(request);
                }

                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "SalerManager_teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "Đơn chờ tạm ứng AUC " + String.Format("{0:dd/MM/yyyy}", time);

                    foreach (var item in data.Data)
                    {
                        long? accepPayment = 0;
                        if (item.Paid > 0)
                        {
                            accepPayment = item.Paid;
                        }
                        long? payment = item.Paid > 0 ? (item.AmountVND - item.Paid) : item.AmountVND;
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Formula = "HYPERLINK(\"" + item.ProductLink + "\",\"" + item.ProductName + "\")";
                        }
                        catch (Exception) { }
                        try
                        {
                            workSheet.Cells[irow, 4].Hyperlink = new Uri(item.ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 5].Value = item.OrderDateDisplay;
                        workSheet.Cells[irow, 6].Value = item.FullName;
                        workSheet.Cells[irow, 7].Value = item.Total;
                        workSheet.Cells[irow, 8].Value = item.ShippingFee;
                        workSheet.Cells[irow, 9].Value = item.Surcharge;
                        workSheet.Cells[irow, 10].Value = item.BuyFee;
                        workSheet.Cells[irow, 11].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 12].Value = item.ShippingUnitGlobal;
                        workSheet.Cells[irow, 13].Value = item.Weight;
                        workSheet.Cells[irow, 14].Value = item.ShippingGlobalVND;
                        workSheet.Cells[irow, 15].Value = item.DeliveryFee;
                        workSheet.Cells[irow, 16].Value = item.AmountVND;
                        workSheet.Cells[irow, 17].Value = accepPayment;
                        workSheet.Cells[irow, 18].Value = payment;
                        workSheet.Cells[irow, 19].Value = item.EmpployeeSupport;

                        irow += 1;
                    }

                    workSheet.Cells["A8:S" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:S" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:S" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:S" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Orange);
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];

                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalerManager_teamplate.xlsx");
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
        public async Task<IActionResult> ExportWaitPurchase(OrderBuyForYouListRequest request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                request.PreState = PurchaseConfig.CHO_MUA_HANG;
                var data = await orderBuyForYouAppService.Export(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "Purchase_Waiting_teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "Đơn chờ mua MH " + String.Format("{0:dd/MM/yyyy}", time);


                    foreach (var item in data.Data)
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
                        workSheet.Cells[irow, 3].Hyperlink = new Uri(item.Link, UriKind.Absolute);

                        workSheet.Cells[irow, 4].Value = noteEx;// $"{item.ProAttribute.ToString()},{item.Note}";


                        workSheet.Cells[irow, 5].Value = "";
                        workSheet.Cells[irow, 6].Value = item.ProductType;
                        workSheet.Cells[irow, 7].Value = item.ProductOrigin;
                        workSheet.Cells[irow, 8].Value = item.BarCode;
                        workSheet.Cells[irow, 9].Value = item.Amount;
                        workSheet.Cells[irow, 10].Value = total;
                        workSheet.Cells[irow, 11].Value = item.ShippingFee;
                        workSheet.Cells[irow, 12].Value = totaltemp;
                        workSheet.Cells[irow, 13].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 14].Value = "";
                        workSheet.Cells[irow, 15].Value = item.BuyFee;
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
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    workSheet.Cells["A" + (data.Data.Count + 8) + ":W" + (data.Data.Count + 8)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + (data.Data.Count + 8) + ":W" + (data.Data.Count + 8)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    workSheet.Cells[data.Data.Count + 8, 9].Formula = "SUM(I8:I" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 10].Formula = "SUM(J8:J" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 12].Formula = "SUM(L8:L" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 16].Formula = "SUM(P8:P" + (data.Data.Count + 7) + ")";
                    workSheet.Cells["A7:W7"].Style.Font.Size = 11;
                    workSheet.Cells["A7:W7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A7:W7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A7:W" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:W" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:W" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:W" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));
                    workSheet.Cells["A7:W7"].Style.Font.Bold = true;
                    //workSheet.Cells["D1:E1"].Style.Font.Bold = true;

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Purchase_Waiting_teamplate.xlsx");
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
        public async Task<IActionResult> ExportPurchased(OrderListRequest request)
        {
            try
            {
                string[] state = new string[] { AutionConfig.DA_MUA_HANG };
                request.State = state;
                var data = await orderappservice.ExportData(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                //var stream = new MemoryStream();
                var path = Path.Combine(environment.WebRootPath, "PuchaseAutionBought.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "File Tổng Hợp Đơn Đấu Giá " + String.Format("{0:dd/MM/yyyy}", time);


                    foreach (var item in data.Data)
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
                        workSheet.Cells[irow, 15].Value = tempPrice;
                        workSheet.Cells[irow, 16].Value = item.BidAccount;
                        workSheet.Cells[irow, 17].Value = item.FullName;
                        workSheet.Cells[irow, 18].Value = item.EmpployeeSupport;
                        workSheet.Cells[irow, 19].Value = item.Tracking;
                        workSheet.Cells[irow, 20].Value = "";
                        workSheet.Cells[irow, 21].Value = "";
                        workSheet.Cells[irow, 22].Value = "";

                        irow += 1;
                    }
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    workSheet.Cells["A" + (data.Data.Count + 8) + ":V" + (data.Data.Count + 8)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + (data.Data.Count + 8) + ":V" + (data.Data.Count + 8)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    workSheet.Cells[data.Data.Count + 8, 7].Formula = "SUM(G8:G" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 8].Formula = "SUM(H8:H" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 9].Formula = "SUM(I8:I" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 10].Formula = "SUM(J8:J" + (data.Data.Count + 7) + ")";
                    workSheet.Cells[data.Data.Count + 8, 14].Formula = "SUM(N8:N" + (data.Data.Count + 7) + ")";
                    workSheet.Cells["A7:V7"].Style.Font.Size = 11;
                    workSheet.Cells["A7:V7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A7:V7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A7:V" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:V" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:V" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:V" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));
                    workSheet.Cells["A7:V7"].Style.Font.Bold = true;

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
        public async Task<IActionResult> ExportWaitPayment(OrderListRequest request)
        {
            try
            {
                OrderListResponse data = null;
                string[] arrString = new string[]
                 {
                        OrderState.CHO_TT_DON_HANG
                 };
                request.State = arrString;
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    data = await orderappservice.ExportAllOrderAution(request);
                }
                else
                {
                    data = await orderappservice.ExportOrderAutionBySale(request);
                }

                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "SalerManager_teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "Đơn Chờ Tất Toán " + String.Format("{0:dd/MM/yyyy}", time);

                    foreach (var item in data.Data)
                    {
                        long? accepPayment = 0;
                        if (item.Paid > 0)
                        {
                            accepPayment = item.Paid;
                        }
                        long? payment = item.Paid > 0 ? (item.AmountVND - item.Paid) : item.AmountVND;
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Formula = "HYPERLINK(\"" + item.ProductLink + "\",\"" + item.ProductName + "\")";
                        }
                        catch (Exception) { }
                        try
                        {
                            workSheet.Cells[irow, 4].Hyperlink = new Uri(item.ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 5].Value = item.OrderDateDisplay;
                        workSheet.Cells[irow, 6].Value = item.FullName;
                        workSheet.Cells[irow, 7].Value = item.Total;
                        workSheet.Cells[irow, 8].Value = item.ShippingFee;
                        workSheet.Cells[irow, 9].Value = item.Surcharge;
                        workSheet.Cells[irow, 10].Value = item.BuyFee;
                        workSheet.Cells[irow, 11].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 12].Value = item.ShippingUnitGlobal;
                        workSheet.Cells[irow, 13].Value = item.Weight;
                        workSheet.Cells[irow, 14].Value = item.ShippingGlobalVND;
                        workSheet.Cells[irow, 15].Value = item.DeliveryFee;
                        workSheet.Cells[irow, 16].Value = item.AmountVND;
                        workSheet.Cells[irow, 17].Value = accepPayment;
                        workSheet.Cells[irow, 18].Value = payment;
                        workSheet.Cells[irow, 19].Value = item.EmpployeeSupport;

                        irow += 1;
                    }

                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Orange);
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalerManager_teamplate.xlsx");
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
        public async Task<IActionResult> ExportPaid(OrderListRequest request)
        {
            try
            {
                OrderListResponse data = null;
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }
                request.PreState = OrderPayment.DA_THANH_TOAN;
                data = await orderappservice.ExportApprovalOrder(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "SalerManager_teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "Đơn hàng đã thanh toán " + String.Format("{0:dd/MM/yyyy}", time);

                    foreach (var item in data.Data)
                    {
                        long? accepPayment = 0;
                        if (item.Paid > 0)
                        {
                            accepPayment = item.Paid;
                        }
                        long? payment = item.Paid > 0 ? (item.AmountVND - item.Paid) : item.AmountVND;
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Formula = "HYPERLINK(\"" + item.ProductLink + "\",\"" + item.ProductName + "\")";
                        }
                        catch (Exception) { }
                        try
                        {
                            workSheet.Cells[irow, 4].Hyperlink = new Uri(item.ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 5].Value = item.OrderDateDisplay;
                        workSheet.Cells[irow, 6].Value = item.FullName;
                        workSheet.Cells[irow, 7].Value = item.Total;
                        workSheet.Cells[irow, 8].Value = item.ShippingFee;
                        workSheet.Cells[irow, 9].Value = item.Surcharge;
                        workSheet.Cells[irow, 10].Value = item.BuyFee;
                        workSheet.Cells[irow, 11].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 12].Value = item.ShippingUnitGlobal;
                        workSheet.Cells[irow, 13].Value = item.Weight;
                        workSheet.Cells[irow, 14].Value = item.ShippingGlobalVND;
                        workSheet.Cells[irow, 15].Value = item.DeliveryFee;
                        workSheet.Cells[irow, 16].Value = item.AmountVND;
                        workSheet.Cells[irow, 17].Value = accepPayment;
                        workSheet.Cells[irow, 18].Value = payment;
                        workSheet.Cells[irow, 19].Value = item.EmpployeeSupport;

                        irow += 1;
                    }

                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Orange);
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalerManager_teamplate.xlsx");
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
        public async Task<IActionResult> ExportDeliverd(OrderListRequest request)
        {
            try
            {
                OrderListResponse data = null;
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }
                data = await orderappservice.ExportApprovalOrder(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 8;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "SalerManager_teamplate.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";
                    workSheet.Cells["D2"].Value = "Đã giao hàng " + String.Format("{0:dd/MM/yyyy}", time);

                    foreach (var item in data.Data)
                    {
                        long? accepPayment = 0;
                        if (item.Paid > 0)
                        {
                            accepPayment = item.Paid;
                        }
                        long? payment = item.Paid > 0 ? (item.AmountVND - item.Paid) : item.AmountVND;
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.Code;
                        try
                        {
                            workSheet.Cells[irow, 3].Formula = "HYPERLINK(\"" + item.ProductLink + "\",\"" + item.ProductName + "\")";
                        }
                        catch (Exception) { }
                        try
                        {
                            workSheet.Cells[irow, 4].Hyperlink = new Uri(item.ProductLink, UriKind.Absolute);
                        }
                        catch (Exception) { }
                        workSheet.Cells[irow, 5].Value = item.OrderDateDisplay;
                        workSheet.Cells[irow, 6].Value = item.FullName;
                        workSheet.Cells[irow, 7].Value = item.Total;
                        workSheet.Cells[irow, 8].Value = item.ShippingFee;
                        workSheet.Cells[irow, 9].Value = item.Surcharge;
                        workSheet.Cells[irow, 10].Value = item.BuyFee;
                        workSheet.Cells[irow, 11].Value = item.ExchangeRate;
                        workSheet.Cells[irow, 12].Value = item.ShippingUnitGlobal;
                        workSheet.Cells[irow, 13].Value = item.Weight;
                        workSheet.Cells[irow, 14].Value = item.ShippingGlobalVND;
                        workSheet.Cells[irow, 15].Value = item.DeliveryFee;
                        workSheet.Cells[irow, 16].Value = item.AmountVND;
                        workSheet.Cells[irow, 17].Value = accepPayment;
                        workSheet.Cells[irow, 18].Value = payment;
                        workSheet.Cells[irow, 19].Value = item.EmpployeeSupport;

                        irow += 1;
                    }

                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A8:R" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["C8:C" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Orange);
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.UnderLine = true;
                    workSheet.Cells["D8:D" + (data.Data.Count + 8)].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));

                    package.Save();

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalerManager_teamplate.xlsx");
                    }
                }

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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDeposite(DepositeListRequest request)
        {
            try
            {
                var response = debtappservice.GetDepositeByAccountId(request);
                return Ok(response.Result);
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
        public async Task<IActionResult> ExportDeposite(DepositsListRequest request)
        {
            try
            {
                var data = await depositsAppService.ExportDebtDeposite(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.DEPOSIT_DATA_NOTFOUND);
                }
                var fromDateSearch = request.StartTime;
                var toDateSearch = request.EndTime;
                var irow = 7;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "Deposit_Template.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["D2"].Value = "Từ ngày: " + String.Format("{0:dd/MM/yyyy HH:mm:ss}", fromDateSearch) + " - Đến ngày: " + String.Format("{0:dd/MM/yyyy HH:mm:ss}", toDateSearch);
                    foreach (var item in data.Data)
                    {
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.AccountName;
                        workSheet.Cells[irow, 3].Value = item.WalletId;
                        workSheet.Cells[irow, 4].Value = item.FTCode;
                        workSheet.Cells[irow, 5].Value = item.Amount;
                        workSheet.Cells[irow, 6].Value = item.BankDescription;
                        workSheet.Cells[irow, 7].Value = item.BankNumber;
                        workSheet.Cells[irow, 8].Value = string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.CreatedDate);
                        workSheet.Cells[irow, 9].Value = item.PayStatus == 0 ? "Đang xử lý" : item.PayStatus == 1 ? "Đã thanh toán" : item.PayStatus == 2 ? "Đã hủy" : "";
                        workSheet.Cells[irow, 10].Value = item.DepositStatus == 0 ? "Chờ xử lý" : item.DepositStatus == 1 ? "Đã nạp tiền" : item.PayStatus == 2 ? "Hủy nạp tiền" : "";
                        workSheet.Cells[irow, 11].Value = item.State == "KHOI_TAO" ? "Khởi tạo" : item.State == "DA_DUYET_CAP_1" ? "Đã duyệt cấp 1" : item.State == "DA_DUYET_CAP_2" ? "Đã duyệt cấp 2" : item.State == "DA_DUYET_CAP_3" ? "Đã duyệt cấp 3" : item.State == "KET_THUC" ? "Kết thúc" : item.State == "DA_HUY" ? "Đã hủy" : item.State == "AUTO" ? "Tự động xử lý" : "";

                        irow += 1;
                    }

                    workSheet.Cells[data.Data.Count + 7, 4].Value = "Tổng số tiền: ";
                    workSheet.Cells[data.Data.Count + 7, 5].Formula = "SUM(E7:E" + (data.Data.Count + 6) + ")";
                    workSheet.Cells["A6:J6"].Style.Font.Size = 12;
                    workSheet.Cells["A6:J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A6:J6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A6:K" + (data.Data.Count + 7)].AutoFitColumns();
                    workSheet.Cells["A6:K" + (data.Data.Count + 7)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A6:K" + (data.Data.Count + 7)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A6:K" + (data.Data.Count + 7)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A6:K" + (data.Data.Count + 7)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A6:A" + (data.Data.Count + 7)].AutoFitColumns(10);
                    workSheet.Cells["E6:E" + (data.Data.Count + 7)].AutoFitColumns(20);
                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 12));
                    workSheet.Cells["A6:K6"].Style.Font.Bold = true;
                    workSheet.Cells["D1:E1"].Style.Font.Bold = true;
                    workSheet.Cells["E7:E" + (data.Data.Count + 7)].Style.Numberformat.Format = "#,##";

                    package.Save();
                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Deposit.xlsx");
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
        public async Task<IActionResult> ExportWithDrawal(withdrawalListRequest request)
        {
            try
            {
                var data = await debtappservice.ExportDebtWithDraWal(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.DEPOSIT_DATA_NOTFOUND);
                }

                var irow = 9;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "DebtWithDrawal_template.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    foreach (var item in data.Data)
                    {
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.CreatedDateDisplay;
                        workSheet.Cells[irow, 3].Value = item.Amount;
                        workSheet.Cells[irow, 4].Value = item.BankNumber;
                        workSheet.Cells[irow, 5].Value = item.BankAccountName;
                        workSheet.Cells[irow, 6].Value = item.BankName;
                        workSheet.Cells[irow, 7].Value = item.WithDrawalStatus == 0 ? "Đang xử lý" : item.WithDrawalStatus == 1 ? "Đã trừ tiền" : item.WithDrawalStatus == 2 ? "Đã hủy" : "";
                        workSheet.Cells[irow, 8].Value = item.State == "KHOI_TAO" ? "Khởi tạo" : item.State == "DUYET_CAP_1" ? "Duyệt cấp 1" : item.State == "DUYET_CAP_2" ? "Duyệt cấp 2" : item.State == "DUYET_CAP_3" ? "Duyệt cấp 3" : item.State == "DA_CHUYEN_KHOAN" ? "Đã chuyển khoản" : item.State == "KET_THUC" ? "Kết thúc" : item.State == "DA_HUY" ? " Đã hủy" : item.State == "TU_DONG" ? "Tự động" : "";
                        workSheet.Cells[irow, 9].Value = item.CreateByString;

                        irow += 1;
                    }
                    workSheet.Cells["A9:I" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:I" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:I" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:I" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 12));

                    package.Save();
                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DebtWithDrawal_template.xlsx");
                    }
                }
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDepositeRe(DebtRefundListRequest request)
        {
            try
            {
                var response = debtappservice.GetDepositeReByAccountId(request);
                return Ok(response.Result);
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
        public async Task<IActionResult> ExportRefundTransaction(DebtRefundListRequest request)
        {
            try
            {
                var data = await debtappservice.ExportDepositeReByAccountId(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.DEPOSIT_DATA_NOTFOUND);
                }

                var irow = 9;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "DebtRefundTransaction_template.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    foreach (var item in data.Data)
                    {
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.CreatedDateDisplay;
                        workSheet.Cells[irow, 3].Value = item.Amount;
                        workSheet.Cells[irow, 4].Value = item.CreateByString;
                        workSheet.Cells[irow, 5].Value = item.FTCode;
                        workSheet.Cells[irow, 6].Value = item.BankDescription;
                        workSheet.Cells[irow, 7].Value = item.Note;
                        workSheet.Cells[irow, 8].Value = item.DepositStatus == 0 ? "Chưa nạp tiền" : item.DepositStatus == 1 ? "Đã nạp tiền" : "";
                        workSheet.Cells[irow, 9].Value = item.State == "KHOI_TAO" ? "Khởi tạo" : item.State == "DUYET_CAP_1" ? "Duyệt cấp 1"
                            : item.State == "DUYET_CAP_2" ? "Duyệt cấp 2" : item.State == "DUYET_CAP_3" ? "Duyệt cấp 3"
                            : item.State == "DA_CHUYEN_KHOAN" ? "Đã chuyển khoản" : item.State == "KET_THUC" ? "Kết thúc"
                            : item.State == "DA_HUY" ? " Đã hủy" : item.State == "TU_DONG" ? "Tự động" : "";
                        workSheet.Cells[irow, 10].Value = item.ProcessDateDisplay;

                        irow += 1;
                    }
                    workSheet.Cells["A9:J" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:J" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:J" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:J" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 12));

                    package.Save();
                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DebtRefundTransaction_template.xlsx");
                    }
                }
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSaler()
        {
            try
            {
                var response = await orderappservice.GetSaler();
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetWithDrawal(withdrawalListRequest request)
        {
            try
            {
                var response = debtappservice.GetWithDraWalByAccountId(request);
                return Ok(response.Result);
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderDetailAccountId(DebtListRequest request)
        {
            try
            {
                var response = debtappservice.GetOrderDetailAccountId(request);
                return Ok(response.Result);
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
        public async Task<IActionResult> GetCustomerWallet(string accountId)
        {
            try
            {
                var response = debtappservice.GetCustomerWallet(accountId);
                return Ok(response.Result);
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPayment(DebtPaymentListRequest request)
        {
            try
            {
                var response = debtappservice.GetPaymentByAccountId(request);
                return Ok(response.Result);
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
        public async Task<IActionResult> ExportPayment(DebtPaymentListRequest request)
        {
            try
            {
                var data = await debtappservice.ExportDebtPaymentByAccountId(request);
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.DEPOSIT_DATA_NOTFOUND);
                }

                var irow = 9;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "DebtPayment_template.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    foreach (var item in data.Data)
                    {
                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.PaymentForm;
                        workSheet.Cells[irow, 3].Value = item.PaymentType;
                        workSheet.Cells[irow, 4].Value = item.RefCode;
                        workSheet.Cells[irow, 5].Value = item.Amount;
                        workSheet.Cells[irow, 6].Value = item.Description;
                        workSheet.Cells[irow, 7].Value = item.CreatedDateDisplay;
                        workSheet.Cells[irow, 8].Value = item.ProcessDateDisplay;
                        workSheet.Cells[irow, 9].Value = item.CreateByString;
                        workSheet.Cells[irow, 10].Value = item.Status == 0 ? "Chờ xử lý" : (item.Status == 1 && item.Refund != true) ? "Đã thanh toán" : (item.Status == 1 && item.Refund == true) ?
                            "Đã hoàn tiền" : item.Status == 2 ? "Hủy thanh toán" : item.Status == 3 ? "Xác nhận thanh toán" : "";
                        workSheet.Cells[irow, 11].Value = item.State == "KHOI_TAO" ? "Khởi tạo" : item.State == "DUYET_CAP_1" ? "Duyệt cấp 1" : item.State == "DUYET_CAP_2" ? "Duyệt cấp 2" : item.State == "DUYET_CAP_3" ? "Duyệt cấp 3" : item.State == "DA_CHUYEN_KHOAN" ? "Đã chuyển khoản" : item.State == "KET_THUC" ? "Kết thúc" : item.State == "DA_HUY" ? " Đã hủy" : item.State == "TU_DONG" ? "Tự động" : "";

                        irow += 1;
                    }
                    workSheet.Cells["A9:K" + (data.Data.Count + 8)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:K" + (data.Data.Count + 8)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:K" + (data.Data.Count + 8)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A9:K" + (data.Data.Count + 8)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 12));

                    package.Save();
                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DebtPayment_template.xlsx");
                    }
                }
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
        public async Task<IActionResult> GetInfomation(string accountId)
        {
            try
            {
                var response = debtappservice.GetInfomation(accountId);
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