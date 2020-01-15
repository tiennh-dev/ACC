using Core.Common.JTable;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class FreezeListJTableModel:JTableModel
    {
        public string Keyword { get; set; }
        public string Ref { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string AccountId { get; set; }
    }
}
