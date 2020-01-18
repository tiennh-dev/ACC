using Core.AppModel.Response;
using iChiba.ACC.PrivateApi.AppModel.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.PrivateApi.AppModel.Response.Account
{
   public class AccountListResponse : PagingResponse<IList<AccountView>>
    {
        public AccountListResponse()
        {
            Data = new List<AccountView>();
        }
    }

    public class AccountResponse : BaseEntityResponse<AccountView>
    {
        public AccountResponse()
        {
            Data = new AccountView();
        }
    }
}
