using iChiba.OM.PrivateApi.AppModel.Request.BankInfo;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class BankInfoListAdapter
    {
        public static BankInfoListRequest ToModel(this BankInfoListJTableModel model)
        {
            BankInfoListRequest _model = JTableModelAdapter.ToModel<BankInfoListJTableModel, BankInfoListRequest>(model);

            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
