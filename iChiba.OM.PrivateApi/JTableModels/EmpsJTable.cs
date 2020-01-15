using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class EmpsJTable : JTableModel
    {
        public string Keyword { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        public string careBy { get; set; }
    }
}
