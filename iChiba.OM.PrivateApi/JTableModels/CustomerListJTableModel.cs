using Core.Common.JTable;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class CustomerListJTableModel : JTableModel
    {
        public string Keyword { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        public bool? BidActive { get; set; }
        public string Code { get; set; }
        public string Saler { get; set; }
        public string[] Group { get; set; }
        public bool? PrePhone { get; set; }
        public bool? PreEmail { get; set; }
        public bool? PreCareBy { get; set; }
    }
}
