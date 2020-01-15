using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Model.Customer;
using iChiba.OM.PrivateApi.AppModel.Model.Warehouse;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Request.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Request.Payment;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.CustomerWallet;
using iChiba.OM.PrivateApi.AppModel.Response.Order;
using iChiba.OM.PrivateApi.AppModel.Response.Orderdetail;
using iChiba.OM.PrivateApi.AppModel.Response.WalletTrans;
using iChiba.OM.PrivateApi.AppService.Implement;
using iChiba.OM.PrivateApi.AppService.Implement.Common;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using Ichiba.IS4.Api.Driver;
using iChiba.OM.PrivateApi.AppModel.Model.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using iChiba.OM.Cache.Cache.Model;
using Core.CustomException;
using iChiba.OM.CustomException;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class OrderAutionDgsController : BaseController
    {
        private readonly IOrderAppService orderAppService;
        private readonly ICustomerAppService customerAppService;
        private readonly OrderWorkflowAppService orderWorkflowAppService;
        private readonly ILevelAppService levelAppService;
        private readonly IWalletTransAppService walletTransAppService;
        private readonly IWalletAppService walletAppService;
        private readonly IWarehouseAppService warehouseAppService;
        private readonly ICustomerWalletAppService customerWalletAppService;
        private readonly IOrderBuyForYouAppService orderBuyForYouAppService;
        private readonly DepositWorkflowAppService depositWorkflowAppService;
        private readonly IProductBidClientInfoAppService bidClientCache;
        private readonly IHostingEnvironment environment;


        public OrderAutionDgsController(ILogger<OrderAutionDgsController> logger,
            IOrderAppService orderAppService,
            ICustomerAppService customerAppService,
            OrderWorkflowAppService orderWorkflowAppService,
            ILevelAppService levelAppService,
            IWalletTransAppService walletTransAppService,
            IWalletAppService walletAppService,
            ICustomerWalletAppService customerWalletAppService,
            IOrderBuyForYouAppService orderBuyForYouAppService,
            DepositWorkflowAppService depositWorkflowAppService,
            IProductBidClientInfoAppService bidClientCache,
            IHostingEnvironment environment,
            IWarehouseAppService warehouseAppService, AccessClient accessClient, AppConfig appConfig) : base(logger, accessClient, appConfig)
        {
            this.orderAppService = orderAppService;
            this.customerAppService = customerAppService;
            this.orderWorkflowAppService = orderWorkflowAppService;
            this.levelAppService = levelAppService;
            this.walletTransAppService = walletTransAppService;
            this.walletAppService = walletAppService;
            this.warehouseAppService = warehouseAppService;
            this.customerWalletAppService = customerWalletAppService;
            this.orderBuyForYouAppService = orderBuyForYouAppService;
            this.depositWorkflowAppService = depositWorkflowAppService;
            this.bidClientCache = bidClientCache;
            this.environment = environment;
        }

        //[Permission(ActionPermission.ACCESS, ActionPermission.OPEN)] // DEMO, có thể thêm nhiều actions, nếu không định nghĩa resource key thì mặc định lấy tên controller
        //[Permission("ORDERAUCTIONPRETREATMENT", ActionPermission.ACCESS, ActionPermission.OPEN)] // DEMO
        //[Permission("ORDERAUCTIONPRETREATMENT", ActionPermission.ACCESS)] // Chờ fix AIM
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
                    appserviceRequest.OrderType = Model.OrderRefType.AUCTION;
                    appserviceRequest.PreCode = AutionConfig.DGS;
                    var response = await orderAppService.GetListJTableAution(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                        request.Draw,
                        response.Total); 
                    return Ok(responseJTable);
                }
                else
                {
                    appserviceRequest.OrderType = Model.OrderRefType.AUCTION;
                    appserviceRequest.PreCode = AutionConfig.DGS;
                    var response = await orderAppService.GetListJTableAutionBySale(appserviceRequest);
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
                OrderListResponse data = null;
                request.OrderType = Model.OrderRefType.AUCTION;
                request.PreCode = AutionConfig.DGS;
                if (!string.IsNullOrWhiteSpace(request.RefType)
                    && !Model.OrderRefType.BuyForYouRefTypes.ContainsKey(request.RefType))
                {
                    request.RefType = null;
                }

                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    data = await orderAppService.ExportAllOrderAution(request);
                }
                else
                {
                    data = await orderAppService.ExportOrderAutionBySale(request);
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
                    workSheet.Cells["D2"].Value = "Đơn chờ tạm ứng DGC " + String.Format("{0:dd/MM/yyyy}", time);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderdetailListResponse))]
        public async Task<IActionResult> GetOrderDetail(OrderDetailListRequest request)
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductBidClientInfoListResponse))]
        public async Task<IActionResult> GetBidClient()
        {

            try
            {
                var response = await bidClientCache.Gets(false);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }



        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<WalletList>>))]
        public async Task<IActionResult> GetListWallet()
        {
            var response = await walletAppService.GetAll();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailPaymentResponse))]
        public async Task<IActionResult> GetOrderDetailPayment(int id, string accountId)
        {
            try
            {
                var refType = "ORDER";
                var response = await orderAppService.GetOrderDetailPayment(id, accountId, refType);

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
                var response = await orderAppService.GetListCustomer();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetCustomerByAccountId(string accountId)
        {
            try
            {
                var response = await orderBuyForYouAppService.GetCustomerByAccountId(accountId);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderDetailPaymentResponse))]
        public async Task<IActionResult> UpdateStatusOrderDetail(int id, string status)
        {
            var response = await orderAppService.UpdateStatusOrderDetail(new UpdateStatusOrderDetailRequest() { OrderId = id, Status = status });
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateStatusOrderResponse))]
        public async Task<IActionResult> UpdateStatusOrder(UpdateStatusOrderRequest request)
        {
            var response = await orderAppService.UpdateStatusOrder(request);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ChangeAutionStatus(int status, int id)
        {
            var response = await orderAppService.ChangeAutionStatus(status, id);

            return Ok(response);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<Model.OrderMessage>>))]
        public async Task<IActionResult> GetMessagesMany(OrderListRequestDialog request)
        {
            try
            {
                var response = await orderAppService.GetMessagesMany(request.Id);
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
        public IActionResult GetPaymentProfile(string accountId)
        {
            try
            {
                var response = levelAppService.GetAuctionFeeCancel(accountId);

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
        public async Task<IActionResult> AddPayment(PaymentAddRequest request)
        {
            try
            {
                var response = await orderAppService.AddPayment(request);
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
        public async Task<IActionResult> AddPaymentOrderBuyForyou(PaymentAddRequest request)
        {
            try
            {
                var response = await orderAppService.AddPaymentBuyForYou(request);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WalletTransListResponse))]
        public async Task<IActionResult> GetwalletByaccountId(string accountId)
        {
            try
            {
                var response = await walletTransAppService.GetWalletByAccountId(accountId);

                return Ok(response);
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
                var response = await walletAppService.GetWalletbyId(walletId);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<WalletList>))]
        public async Task<IActionResult> GetWallets()
        {
            try
            {
                var response = await walletAppService.GetAll();

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
        public async Task<IActionResult> PreUpdate(OrderPreUpdateRequest request)
        {
            try
            {
                var response = await orderAppService.PreUpdate(request);

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<CustomerInfo>))]
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


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> AddRechangeDgs(RechageAddRequest request)
        {
            var response = await orderAppService.CreateDeposite(request);
            if (response.Data != null)
            {
                var data = response.Data;
                var requestWf = new DepositWVModel()
                {
                    Id = data.Id,
                    Action = "DUYỆT CẤP 1",
                    Message = request.BankDescription
                };
                var responsewf = await depositWorkflowAppService.ApproveLevel1(requestWf);
            }
            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> createDeposite(RechageAddRequest request)
        {
            var response = await orderAppService.CreateDeposite(request);

            return Ok(response);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> PayOrders(OrderPaysRequest request)
        {
            var response = await orderAppService.PayOrders(request);

            return Ok(response);
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
                    Action = "Y/C HUỶ ĐƠN",
                    Message = request.Message
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

        /// <summary>
        /// y/c duyệt tạm ứng đơn lẻ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> RequestApproveTempDeposit(OrderWorkflowRequest request)
        {
            try
            {
                var requestWf = new OrderWVModel()
                {
                    Id = request.Id,
                    Action = "Y/C DUYỆT TẠM ỨNG",
                    Message = request.Message
                };
                var response = await orderWorkflowAppService.RequestApproveTempDeposit(requestWf);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Y/C duyệt tạm ứng theo lô
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> RequestApproveManyTempDeposit(OrderWorkflowByRequest request)
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
                        Action = "Y/C DUYỆT TẠM ỨNG",
                        Message = request.Message
                    };
                    response = await orderWorkflowAppService.RequestApproveTempDeposit(requestWf);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkflowTriggerInfo))]
        public async Task<IActionResult> Delivery(OrderWorkflowRequest request)
        {
            try
            {
                var requestWf = new OrderWVModel()
                {
                    Id = request.Id,
                    Message = request.Message,
                    Action = "ĐÃ GIAO HÀNG"
                };
                var response = await orderWorkflowAppService.Delivery(requestWf);

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