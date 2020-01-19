using Core.AppModel.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.PrivateApi.AppModel.Request
{
   public class BankAccountListRequest : SortRequest
    {
        public string Keyword { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string Owner { get; set; }
        public bool Actives { get; set; }
    }
}
