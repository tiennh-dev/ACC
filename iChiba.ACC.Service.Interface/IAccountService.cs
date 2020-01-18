using iChiba.ACC.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Service.Interface
{
   public interface IAccountService
    {
        IList<Account> GetAccounts(string Keyword, string Name, int? Type, bool Active, Core.Common.Sorts sort, Core.Common.Paging paging);
        IList<Account> GetListAccountsByParent(int parent);
        Account GetAccountById(int Id);
    }
}
