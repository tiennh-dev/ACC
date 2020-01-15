using iChiba.OM.PrivateApi.AppModel.Request.Orderdetail;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class OrderDetailListAdapter
    {
        public static OrderDetailListRequest ToModel(this OrderDetailListJTableModel model)
        {
            OrderDetailListRequest _model = JTableModelAdapter.ToModel<OrderDetailListJTableModel, OrderDetailListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.OrderId = model.Id;

            return _model;
        }
    }
}
