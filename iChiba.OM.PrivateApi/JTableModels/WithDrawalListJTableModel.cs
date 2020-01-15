using Core.Common.JTable;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class WithDrawalListJTableModel : JTableModel
    {
        public string Keyword { get; set; }
        public string WithDrawalStatus { get; set; }
        public string AccountId { get; set; }
        public string BankNumber { get; set; }
        public string BankAccountName { get; set; }
        public string[] state { get; set; }
    }
}
