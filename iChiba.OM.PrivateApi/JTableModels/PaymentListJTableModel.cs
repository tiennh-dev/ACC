using Core.Common.JTable;
using System;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class PaymentListJTableModel : JTableModel
    {
        public string Keyword { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }
        public string PaymentForm { get; set; }
        public string PaymentType { get; set; }
        public DateTime? StartTime{ get; set; }
        public DateTime? EndTime { get; set; }
        public string[] State { get; set; }
        public string RefCode { get; set; }
        public int? Status { get; set; }
    }
}
