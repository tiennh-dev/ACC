using Core.Common.JTable;
using System;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class UserBidProductListJTableModel:JTableModel
    {
        public string AccountId { get; set; }
        public string ProductId { get; set; }
        public string YAUsername { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ProductName { get; set; }
    }
}
