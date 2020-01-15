using Core.AppModel.Response;
using Core.Common.JTable;
using Core.CustomException;
using iChiba.OM.Cache.Interface;
using iChiba.OM.CustomException;
using iChiba.OM.PrivateApi.AppModel.Model;
using iChiba.OM.PrivateApi.AppModel.Request;
using iChiba.OM.PrivateApi.AppModel.Request.Empolyess;
using iChiba.OM.PrivateApi.AppModel.Request.MembershipLevel;
using iChiba.OM.PrivateApi.AppModel.Request.Order;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.Employess;
using iChiba.OM.PrivateApi.AppModel.Response.Level;
using iChiba.OM.PrivateApi.AppService.Implement;
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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerAppService customerAppService;
        private readonly ICustomerProfileAppService customerProfileAppService;
        private readonly ILevelAppService levelAppService;
        private readonly IFreezeAppService freezeAppService;
        private readonly IEmployessAppservice employessappservice;
        private readonly IWalletAppService walletAppService;
        private readonly IOrderAppService orderAppService;
        private readonly DepositWorkflowAppService depositWorkflowAppService;
        private readonly IHostingEnvironment environment;

        public CustomerController(ILogger<CustomerController> logger,
            ICustomerAppService customerAppService,
            IEmployessAppservice employessappservice,
            ICustomerProfileAppService customerProfileAppService, 
            ILevelAppService levelAppService, 
            IAspNetUserCache aspNetUserCache,
            AccessClient accessClient, 
            AppConfig appConfig,
            IFreezeAppService freezeAppService, IWalletAppService walletAppService, IOrderAppService orderAppService, IHostingEnvironment environment,
            DepositWorkflowAppService depositWorkflowAppService)
            : base(logger, accessClient, appConfig)
        {
            this.customerAppService = customerAppService;
            this.customerProfileAppService = customerProfileAppService;
            this.levelAppService = levelAppService;
            this.freezeAppService = freezeAppService;
            this.employessappservice = employessappservice;
            this.walletAppService = walletAppService;
            this.orderAppService = orderAppService;
            this.depositWorkflowAppService = depositWorkflowAppService;
            this.environment = environment;
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
                    var response = await customerAppService.GetList(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),
                         request.Draw,
                        response.Total);
                    return Ok(responseJTable);
                }
                else
                {
                    var response = await customerAppService.GetListByCare(appserviceRequest);
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
        public async Task<IActionResult> Export(CustomerListRequest request)
        {
            try
            {
                var data = new CustomerListResponse();
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                  data = await customerAppService.ExportData(request);
                }
                else
                {
                  data = await customerAppService.ExportDataByCare(request);
                }
                if (data == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.EXPORT_MESSAGE);
                }
                var time = DateTime.Now;
                var irow = 7;
                int STT = 1;
                var path = Path.Combine(environment.WebRootPath, "Customer_template.xlsx");
                FileInfo fileInfo = new FileInfo(path);
                using (var package = new ExcelPackage(fileInfo, true))
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    workSheet.Cells["A1"].Value = "Công ty cổ phần ICHIBA Việt Nam";

                    foreach (var item in data.Data)
                    {

                        workSheet.Cells[irow, 1].Value = STT++;
                        workSheet.Cells[irow, 2].Value = item.IdName != null ? item.IdName : item.Fullname;
                        workSheet.Cells[irow, 3].Value = item.Username;
                        workSheet.Cells[irow, 4].Value = item.Email;
                        workSheet.Cells[irow, 5].Value = item.Phone;
                        workSheet.Cells[irow, 6].Value = item.AllowBid == true ? "active" : "un-active";
                        workSheet.Cells[irow, 7].Value = item.Empsupport;
                     
                        irow += 1;
                    }

                    workSheet.Cells["A7:G" + (data.Data.Count + 7)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:G" + (data.Data.Count + 7)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:G" + (data.Data.Count + 7)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A7:G" + (data.Data.Count + 7)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont(new Font("Times New Roman", 11));
                    workSheet.Cells["A6:H6"].Style.Font.Bold = true;
                    workSheet.Cells["D2:E2"].Style.Font.Bold = true;
                    package.Save();
                  

                    using (var buffer = package.Stream as MemoryStream)
                    {
                        return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer_template.xlsx");
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetListAll()
        {
            var response = await customerAppService.GetListAll();

            return Ok(response);
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
                var response = await orderAppService.GetSaler();
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(EmployessResponse))]
        public async Task<IActionResult> GetListEmployess(EmployessAddRequest request)
        {
            try
            {
                var response = await employessappservice.GetListEmployess(request);
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
                var response = await customerAppService.GetCustomerByAccountId(accountId);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> AddRechange(RechageAddRequest request)
        {
            var response = await orderAppService.CreateDeposite(request);
            if (response.Data != null)
            {
                var data = response.Data;
                var requestWf = new DepositWVModel()
                {
                    Id = data.Id,
                    Action = "DUYỆT CẤP 1",
                    Message = request.Note
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
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerDetailResponse))]
        public async Task<IActionResult> GetDetail(int id)
        {
            var response = await customerAppService.GetDetail(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerAddResponse))]
        public async Task<IActionResult> Add(CustomerAddRequest request)
        {
            var response = await customerAppService.Add(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerUpdateResponse))]
        public async Task<IActionResult> Update(CustomerUpdateRequest request)
        {
            var response = await customerAppService.Update(request);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerDeleteResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await customerAppService.Delete(id);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerActiveAllowBidResponse))]
        public async Task<IActionResult> ChangeAllowBid(int id)
        {
            var response = await customerAppService.ChangeAllowBid(id);

            return Ok(response);
        }

        [HttpPost("{AccountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ActiveTranCode(string AccountId)
        {
            var response = await customerAppService.ActiveTranCode(AccountId);

            return Ok(response);
        }

        [HttpPost("{AccountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UnActiveTranCode(string AccountId)
        {
            var response = await customerAppService.UnActiveTranCode(AccountId);

            return Ok(response);
        }


        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerActiveAllowBidResponse))]
        public async Task<IActionResult> MembershipLevelChangeAllowBid(MenbershipLevelRequest request)
        {
            var response = await customerAppService.MembershipLevelChangeAllowBid(request);

            return Ok(response);
        }


        [HttpPost("{AccountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomerProfile(string AccountId)
        {
            var response = await customerAppService.GetCustomerProfile(AccountId);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> GetMapSuccessfulBid()
        {
            var response = await customerAppService.GetMapSuccessful();

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LevelResponse))]
        public async Task<IActionResult> GetLevel()
        {
            var response = await levelAppService.GetLevelList();

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LevelDetailResponse))]
        public async Task<IActionResult> GetDetailLevel(int? id)
        {
            var response = await customerAppService.GetDetailLevel(id);

            return Ok(response);
        }

        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> TemporaryDepositVIP(string accountId)
        {
            var response = await freezeAppService.TemporaryDepositVIP(accountId);

            return Ok(response);
        }

        [HttpPost("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> TemporaryDepositVIPCancel(string accountId)
        {
            var response = await freezeAppService.TemporaryDepositVIPCancel(accountId);

            return Ok(response);
        }
        [HttpPost("{careBy}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetEmployeeDetail(string careBy)
        {
            var response = await employessappservice.GetEmployeeDetail(careBy);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerListResponse))]
        public async Task<IActionResult> ListTransportWaitForActivation(CustomerProfileByKeyJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var response = await customerAppService.GetListTransportWaitForActivation(appserviceRequest);
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
    }
}