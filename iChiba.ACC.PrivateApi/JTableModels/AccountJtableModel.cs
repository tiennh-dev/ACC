using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.JTableModels
{
    public class AccountJtableModel : JTableModel
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public bool Actives { get; set; }
    }
}
