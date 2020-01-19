using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.JTableModels
{
    public class BankAccountJtableModel : JTableModel
    {
        public string Keyword { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string Owner { get; set; }
        public bool Active { get; set; }
    }
}
