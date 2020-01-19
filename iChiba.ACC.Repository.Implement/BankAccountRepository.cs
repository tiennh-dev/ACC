using Core.Repository.Abstract;
using iChiba.ACC.DbContext;
using iChiba.ACC.Model;
using iChiba.ACC.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Repository.Implement
{
   public class BankAccountRepository : BaseRepository<ACCDBContext, Bank_Account>, IBankAccountRepository
    {
        public BankAccountRepository(ACCDBContext dbContext) : base(dbContext)
        {
        }
    }
}
