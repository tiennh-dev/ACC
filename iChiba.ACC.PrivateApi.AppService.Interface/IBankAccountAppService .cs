using Core.AppModel.Response;
using iChiba.ACC.PrivateApi.AppModel.Request;
using iChiba.ACC.PrivateApi.AppModel.Response.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.AppService.Interface
{
  public interface IBankAccountAppService
    {
        Task<BankAccountListResponse> GetBankAccounts(BankAccountListRequest request);
        Task<BaseResponse> Add(BankAccountAddRequest request);
    }
}
