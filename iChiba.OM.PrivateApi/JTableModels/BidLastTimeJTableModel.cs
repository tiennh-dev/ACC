
using Core.Common.JTable;
using System;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class BidLastTimeJTableModel:JTableModel
    {
        public string Keyword { get; set; }
        public string ProductId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string AccountId { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
    }
}
