using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class OrderPackageListAdapter
    {
        public static OrderPackageListRequest ToModel(this OrderPackageListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<OrderPackageListJTableModel, OrderPackageListRequest>(model);

            _model.OrderId = model.OrderId;
            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
