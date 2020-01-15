using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using iChiba.OM.PrivateApi.Utilities;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class UserBidProductController : BaseController
    {
        private readonly IUserBidProductAppService userBidProductAppService;

        public UserBidProductController(ILogger<UserBidProductController> logger,
            IUserBidProductAppService userBidProductAppService, AccessClient accessClient, AppConfig appConfig)
            : base(logger, accessClient, appConfig)
        {
            this.userBidProductAppService = userBidProductAppService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserBidProductListResponse))]
        public async Task<IActionResult> GetJTable(UserBidProductListJTableModel request)
        {
            try
            {
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                { 
                    var appserviceRequest = request.ToModel();
                    var response = await userBidProductAppService.GetJTable(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(), request.Draw, response.Total); return Ok(responseJTable);
                }
                else
                {

                    var appserviceRequest = request.ToModel();
                    var response = await userBidProductAppService.GetJTableBuyCare(appserviceRequest);
                    var responseJTable = JTableHelper.JObjectTable(response.Data.ToList(),  request.Draw,   response.Total); return Ok(responseJTable);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserBidProductListResponse))]
        public async Task<IActionResult> GetListAll()
        {
            try
            {
                var response = await userBidProductAppService.GetListAll();
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
                var isPerm = await base.CheckPermission(ActionPermission.VIEW_ALL_CUSTOMER.ToString());
                if (isPerm)
                {
                    var response = await userBidProductAppService.GetListCustomer();
                    return Ok(response);
                }
                else
                {

                    var response = await userBidProductAppService.GetListCustomerByCare();
                    return Ok(response);
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