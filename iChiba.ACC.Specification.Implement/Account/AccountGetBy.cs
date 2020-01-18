using Core.Specification.Abstract;
using iChiba.ACC.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Specification.Implement
{
    public class AccountGetBy : SpecificationBase<Account>
    {
        public AccountGetBy(string keyword, string Name, int? Type, bool Active)
           : base(m => string.IsNullOrWhiteSpace(keyword) || m.Name.Contains(keyword)
                       && (Type == null || m.Type == Type)
                       && (Active == true ? m.Active == true : m.Active == false))
        {
        }

        public AccountGetBy(int parent)
          : base(m => m.Parent == parent)
        {
        }
    }
}
