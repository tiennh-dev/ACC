using Core.Common;
using iChiba.ACC.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Service.Interface
{
   public interface IBankAccountService
    {
        IList<Bank_Account> GetBankAccounts(string Keyword, string BankAccount, string BankName, string Owner,bool Active,Sorts sorts,Paging paging);
        IList<Bank_Account> GetAllBankAccounts();
        void Add(Bank_Account bank_Account);
        void Delete(Bank_Account bank_Account);
        Bank_Account GetById(int Id);
    }
}
