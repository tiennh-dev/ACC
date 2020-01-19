using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.PrivateApi.AppModel.Model
{
   public class BankAccountView
    {
        public int Id { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string Province { get; set; }
        public string Address { get; set; }
        public string Owner { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
    }
}
