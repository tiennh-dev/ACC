using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class ProductTypeListAdapter
    {
        public static ProductTypeListRequest ToModel(this ProductTypeListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<ProductTypeListJTableModel, ProductTypeListRequest>(model);

            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
