using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using Core.CustomException;
using iChiba.OM.Cache.Interface;
using iChiba.OM.CustomException;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Model.DepositsMessage;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Deposits;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.CustomerWallet;
using iChiba.OM.PrivateApi.AppService.Implement;
using iChiba.OM.PrivateApi.AppService.Implement.Common;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.Service.Interface;
using iChibaShopping.Core.AppService.Implement;
using iChibaShopping.Core.AppService.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class DepositsController : BaseController
    {
        private readonly IDepositsAppService depositsAppService;
        private readonly IDepositsService depositsService;
        private readonly IWalletAppService walletAppService;
        private readonly ICustomerAppService customerAppService;
        private readonly IOrderAppService orderAppService;
        private readonly ICustomerWalletAppService customerWalletAppService;
        private readonly ICurrentContext currentContext;
        private readonly DepositWorkflowAppService depositWorkflowAppService;
        private readonly IHostingEnvironment environment;
        private readonly IAspNetUserCache aspNetUserCache;
        private readonly IBankInfoAppService bankInfoAppService;

        public DepositsController(ILogger<DepositsController> logger,
            IDepositsAppService depositsAppService,
            IWalletAppService walletAppService,
            ICustomerAppService customerAppService,
            IOrderAppService orderAppService,
            IDepositsService depositsService,
            ICustomerWalletAppService customerWalletAppService,
            ICurrentContext currentContext,
            IAspNetUserCache aspNetUserCache,
            DepositWorkflowAppService depositWorkflowAppService,
            IBankInfoAppService bankInfoAppService,
            IHostingEnvironment environment)
            : base(logger)
        {
            this.depositsAppService = depositsAppService;
            this.walletAppService = walletAppService;
            this.customerAppService = customerAppService;
            this.orderAppService = orderAppService;
            this.customerWalletAppService = customerWalletAppService;
            this.depositWorkflowAppService = depositWorkflowAppService;
            this.environment = environment;
            this.depositsService = depositsService;
            this.currentContext = currentContext;
            this.aspNetUserCache = aspNetUserCache;
            this.bankInfoAppService = bankInfoAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingResponse<IList<DepositsList>>))]
        public async Task<IActionResult> GetJTable(DepositsListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                appserviceRequest.DepositeType = DepositType.REFUND;
                var response = await depositsAppService.GetListJTable(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListCustomer(DepositListTopRequest request)
        {
            try
            {
                var response = await depositsAppService.GetListCustomer(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<DepositsDetail>))]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await depositsAppService.GetById(id);

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<DepositsDetail>))]
        public async Task<IActionResult> GetStateById(int id)
        {
            var response = await depositsAppService.GetStateById(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateConfirmStatus(DepositsConfirmStatusRequest request)
        {
            var response = await depositsAppService.UpdateConfirmStatus(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Add(DepositAddRequest request)
        {
            var response = await depositsAppService.Add(request);

            return Ok(response);
        }


        [HttpPost("{bankNumber}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetBankInfoByBankNumber(string bankNumber)
        {
            var response = await bankInfoAppService.GetBankInfoByBankNumber(bankNumber);

            return Ok(response);
        }

        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetListBankInfoByBankNumber()
        {
            var response = await bankInfoAppService.GetListBankInfoByBankNumber();

            return Ok(response);
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



        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CancelPayment(int id)
        {
            var response = await depositsAppService.CancelPayment(id);

            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Export(DepositsListRequest request)
        {
            try
            {
                var accountName = "";
                var userId = currentContext.UserId;
                var userByAccountId = await aspNetUserCache.GetById(userId);
                if (userByAccountId != null)
                {
                    accountName = userByAccountId.FullName;
                }
                else
                {
                    accountName = "";
                }
                request.DepositeType = DepositType.REFUND;
                var data = await depositsAppService.GetDataExport(request);
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
                using (var package = new ExcelPackage(fileInfo,true))
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<IList<DepositsMessage>>))]
        public async Task<IActionResult> GetMessages(DepositsMessageGetByIdRequest request)
        {
            try
            {
                var response = await depositsAppService.GetMessages(request);

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

        [HttpPost("{Id}/{ftCode}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateFTcode(int Id, string ftCode)
        {
            try
            {
                var data = await depositsAppService.UpDateFtCode(Id, ftCode);
                return Ok(data);
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
        public async Task<IActionResult> ApproveLevel1(DepositWorkflowRequest request)
        {
            try
            {
                var requestWf = new DepositWVModel()
                {
                    Id = request.Id,
                    Action = "DUYỆT CẤP 1",
                    Message = request.Message
                };
                var response = await depositWorkflowAppService.ApproveLevel1(requestWf);

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
        public async Task<IActionResult> ApproveLevel2(DepositWorkflowRequest request)
        {
            try
            {
                var requestWf = new DepositWVModel()
                {
                    Id = request.Id,
                    Action = "DUYỆT CẤP 2",
                    Message = request.Message
                };
                var response = await depositWorkflowAppService.ApproveLevel2(requestWf);

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
        public async Task<IActionResult> ApproveLevel3(DepositWorkflowRequest request)
        {
            try
            {
                var requestWf = new DepositWVModel()
                {
                    Id = request.Id,
                    Action = "DUYỆT CẤP 3",
                    Message = request.Message
                };
                var response = await depositWorkflowAppService.ApproveLevel3(requestWf);

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
        public async Task<IActionResult> Reject(DepositWorkflowRequest request)
        {
            try
            {
                var requestWf = new DepositWVModel()
                {
                    Id = request.Id,
                    Action = "TỪ CHỐI",
                    Message = request.Message
                };
                var response = await depositWorkflowAppService.Reject(requestWf);

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
        public async Task<IActionResult> Cancel(DepositWorkflowRequest request)
        {
            try
            {
                var requestWf = new DepositWVModel()
                {
                    Id = request.Id,
                    Action = "HUỶ",
                    Message = request.Message
                };
                var response = await depositWorkflowAppService.Cancel(requestWf);

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