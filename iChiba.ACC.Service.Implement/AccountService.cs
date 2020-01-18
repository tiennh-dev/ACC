using iChiba.ACC.Model;
using iChiba.ACC.Repository.Interface;
using iChiba.ACC.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iChiba.ACC.Specification.Implement;
 

namespace iChiba.ACC.Service.Implement
{
   public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public IList<Account> GetAccounts(string Keyword,string Name, int? Type, bool Active, Core.Common.Sorts sort, Core.Common.Paging paging)
        {
            return accountRepository.Find(new AccountGetBy(Keyword,Name,Type,Active),sort,paging).ToList();
        }

        public IList<Account> GetListAccountsByParent(int parent)
        {
            return accountRepository.Find(new AccountGetBy(parent)).ToList();
        }


        public Account GetAccountById(int Id)
        {
            return accountRepository.FindById(Id);
        }
    }
}
