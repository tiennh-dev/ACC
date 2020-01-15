using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class OrderServiceListAdapter
    {
        public static OrderServiceListRequest ToModel(this OrderServiceListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<OrderServiceListJTableModel, OrderServiceListRequest>(model);

            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
