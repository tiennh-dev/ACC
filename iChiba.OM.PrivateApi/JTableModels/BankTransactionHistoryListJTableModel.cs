using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class BankTransactionHistoryListJTableModel:JTableModel
    {
        public string Keyword { get; set; }
        public string AccountNumber { get; set; }
        public long? DebitAmount { get; set; }
        public long? CreditAmount { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
