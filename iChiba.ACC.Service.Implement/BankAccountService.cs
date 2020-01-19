using Core.Common;
using iChiba.ACC.Model;
using iChiba.ACC.Repository.Interface;
using iChiba.ACC.Service.Interface;
using iChiba.ACC.Specification.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iChiba.ACC.Service.Implement
{
   public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository bankAccountRepository;
        public BankAccountService(IBankAccountRepository bankAccountRepository)
        {
            this.bankAccountRepository = bankAccountRepository;
        }

        public IList<Bank_Account> GetBankAccounts(string Keyword, string BankAccount, string BankName, string Owner, bool Active, Sorts sorts, Paging paging)
        {
            return bankAccountRepository.Find(new BankAccountGetBy(Keyword, BankAccount, BankName, Owner, Active), sorts, paging).ToList();
        }

        public Bank_Account GetById(int Id)
        {
            return bankAccountRepository.FindById(Id);
        }

        public IList<Bank_Account> GetAllBankAccounts()
        {
            return bankAccountRepository.Find().ToList();
        }

        public void Add(Bank_Account bank_Account)
        {
            bankAccountRepository.Add(bank_Account);
        }

        public void Delete(Bank_Account bank_Account)
        {
            bankAccountRepository.Delete(bank_Account);
        }
    }
}
