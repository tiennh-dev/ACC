using Core.Common.JTable;
using System;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class DepositsListJTableModel : JTableModel
    {
        public string Keyword { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string AccountId { get; set; }
        public string DepositSt { get; set; }
        public string BankNumber { get; set; }
        public string BankDescription { get; set; }
        public string[] state { get; set; }
        public string FtCode { get; set; }
        public int PayStatus { get; set; }
        public string DepositeType { get; set; }
    }
}
