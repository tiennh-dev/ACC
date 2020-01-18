using iChiba.ACC.PrivateApi.AppModel.Request;
using iChiba.ACC.PrivateApi.AppModel.Response.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.AppService.Interface
{
  public interface IAccountAppService
    {
        Task<AccountListResponse> GetAccount(AccountListRequest request);
        Task<AccountResponse> GetAccountById(int Id);
    }
}
