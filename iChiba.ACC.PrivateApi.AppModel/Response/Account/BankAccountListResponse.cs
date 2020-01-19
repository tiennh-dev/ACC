using Core.AppModel.Response;
using iChiba.ACC.PrivateApi.AppModel.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.PrivateApi.AppModel.Response.Account
{
   public class BankAccountListResponse : PagingResponse<IList<BankAccountView>>
    {
        public BankAccountListResponse()
        {
            Data = new List<BankAccountView>();
        }
    }
}
