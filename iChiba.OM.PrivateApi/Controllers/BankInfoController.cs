using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.AppModel.Response;
using Core.Common.JTable;
using iChiba.OM.Model;
using iChiba.OM.PrivateApi.AppModel.Model.BankInfo;
using iChiba.OM.PrivateApi.AppModel.Request.BankInfo;
using iChiba.OM.PrivateApi.AppModel.Response.BankInfo;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.JTableModels;
using iChiba.OM.PrivateApi.JTableModels.Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class BankInfoController : BaseController
    {
        private readonly IBankInfoAppService bankInfoAppService;

        public BankInfoController(ILogger<BankInfoController> logger,
            IBankInfoAppService bankInfoAppService
            )
            : base(logger)
        {
            this.bankInfoAppService = bankInfoAppService;
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingResponse<IList<Bankinfo>>))]
        public async Task<IActionResult> GetJTable(BankInfoListJTableModel request)
        {
            try
            {
                var appserviceRequest = request.ToModel();
                var response = await bankInfoAppService.GetListJTable(appserviceRequest);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Add(BankInfoAddRequest request)
        {
            var response = await bankInfoAppService.Add(request);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Update(BankInfoUpdateRequest request)
        {
            var response = await bankInfoAppService.Update(request);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await bankInfoAppService.Delete(id);

            return Ok(response);
        }
        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BaseEntityResponse<BankInfoDetail>))]
        public async Task<IActionResult> GetDetail(int id)
        {
            var response = await bankInfoAppService.GetDetail(id);

            return Ok(response);
        }
        [HttpPost("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankInfoChangeStatusResponse))]
        public async Task<IActionResult> OnActive(int id)
        {
            var response = await bankInfoAppService.OnActive(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankInfoChangeStatusResponse))]
        public async Task<IActionResult> ForDiposite(int id,int? value)
        {
            var response = await bankInfoAppService.ForDeposite(id,value);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankInfoChangeStatusResponse))]
        public async Task<IActionResult> ForWITHDRAWAL(int id,int? value)
        {
            var response = await bankInfoAppService.ForWithDrawal(id,value);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BankInfoChangeStatusResponse))]
        public async Task<IActionResult> UpdateBankType(int id, string banktype)
        {
            if(banktype== "Chọn")
            {
                banktype = null;
            }
            var response = await bankInfoAppService.UpdateBankType(id, banktype);

            return Ok(response);
        }
    }
}