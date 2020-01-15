using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Request.WarehouseEmp;
using iChiba.OM.PrivateApi.AppModel.Response;
using iChiba.OM.PrivateApi.AppModel.Response.Warehouse;
using iChiba.OM.PrivateApi.AppModel.Response.WarehouseEmp;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class WarehouseEmpController : BaseController
    {
        private readonly IWarehouseEmpAppService warehouseEmpAppService;

        public WarehouseEmpController(ILogger<WarehouseEmpController> logger,
            IWarehouseEmpAppService warehouseEmpAppService
            ) : base(logger)
        {
            this.warehouseEmpAppService = warehouseEmpAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WarehouserEmpListResponse))]
        public async Task<IActionResult> GetJTable(WarehouseEmpListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var response = await warehouseEmpAppService.GetAll(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WarehouseListResposne))]
        public async Task<IActionResult> GetWarehouseList()
        {
            try
            {
                var response = await warehouseEmpAppService.GetWarehouseList();

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(EmployeeListJTableModel))]
        public async Task<IActionResult> GetListEmployee(EmployeeListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var response = await warehouseEmpAppService.GetAllEmp(appserviceRequest);
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

        [HttpPost("{id}/{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WarehouseEmpAddReponse))]
        public async Task<IActionResult> Add(int id,string accountId)
        {
             var response = await warehouseEmpAppService.Add(id,accountId);
             return Ok(response);
        }
        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WarehouseEmpAddReponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await warehouseEmpAppService.Delete(id);
            return Ok(response);
        }
    }
}