using iChiba.OM.PrivateApi.AppModel.Request.UserBidProduct;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class UserBidProdcutListAdapter
    {
        public static UserBidProductListRequest ToModel(this UserBidProductListJTableModel model)
        {
            UserBidProductListRequest _model = JTableModelAdapter.ToModel<UserBidProductListJTableModel, UserBidProductListRequest>(model);

            _model.AccountId = model.AccountId;
            _model.ProductId = model.ProductId;
            _model.YAUsername = model.YAUsername;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            _model.ProductName = model.ProductName;
            return _model;
        }
    }
}
