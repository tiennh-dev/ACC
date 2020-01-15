using Core.Common.JTable;
using System;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class SuccessfulBidListJTableModel:JTableModel
    {
        public string AccountId { get; set; }
        public string YauserName { get; set; }
        public string Keyword { get; set; }
        public string SearchKeyword { get; set; }
        public DateTime? StartTime{ get; set; }
        public DateTime? EndTime { get; set; }
        public int? PaymentStatus { get; set; }
        public string PreCode { get; set; }
        public string saler { get; set; }
    }
}
