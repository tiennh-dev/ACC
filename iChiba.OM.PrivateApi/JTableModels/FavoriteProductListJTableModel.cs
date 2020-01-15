
using Core.Common.JTable;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public class FavoriteProductListJTableModel:JTableModel
    {
        public string Keyword { get; set; }
        public string AccountId { get; set; }
    }
}
