using Core.Common.JTable;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class OrderPackageListJTableModel : JTableModel
    {
        public int? OrderId { get; set; }
        public string Keyword { get; set; }
    }
}
