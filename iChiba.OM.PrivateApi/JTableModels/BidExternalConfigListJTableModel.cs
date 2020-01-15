using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class BidExternalConfigListJTableModel : JTableModel
    {
        public string AccountId { get; set; }
        public string YAUserName { get; set; }
        public string Description { get; set; }
        public int[] Status { get; set; }
    }
}
