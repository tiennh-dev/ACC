using iChiba.OM.PrivateApi.AppModel.Request.WalletTrans;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class WalletTransListAdapter
    {
        public static WalletTransListRequest ToModel(this WalletTransListJTableModel model)
        {
            WalletTransListRequest _model = JTableModelAdapter.ToModel<WalletTransListJTableModel, WalletTransListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AccountId = model.AccountId;
            _model.Description = model.Description;
            _model.Type = model.Type;
            _model.RefId = model.RefId;
            return _model;
        }
    }
}
