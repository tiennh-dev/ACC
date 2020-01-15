using iChiba.OM.PrivateApi.AppModel.Request.Warehouse;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class WarehouseListAdapter
    {
        public static WarehouseListRequest ToModel(this WarehouseListJTableModel model)
        {
            WarehouseListRequest _model = JTableModelAdapter.ToModel<WarehouseListJTableModel, WarehouseListRequest>(model);

            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
