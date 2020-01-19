using Core.Specification.Abstract;
using iChiba.ACC.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Specification.Implement
{
   public class BankAccountGetBy : SpecificationBase<Bank_Account>
    {
        public BankAccountGetBy(string Keyword, string BankAccount, string BankName, string Owner, bool Active)
         : base(m=> (m.BankAccount.Contains(Keyword) || m.BankName.Contains(Keyword) || m.Owner.Contains(Keyword))
                    && (string.IsNullOrWhiteSpace(BankAccount) || m.BankAccount.Contains(BankAccount))
                    && (string.IsNullOrWhiteSpace(BankName) || m.BankName.Contains(BankName))
                    && (string.IsNullOrWhiteSpace(Owner) || m.Owner.Contains(Owner))
                    && (Active==true ? m.Active==true : m.Active==false))
        {
        }
    }
}
