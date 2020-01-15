using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class CustomerProfileAdapter
    {
        public static CustomerProfileByKeyRequest ToModel(this CustomerProfileByKeyJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<CustomerProfileByKeyJTableModel, CustomerProfileByKeyRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.Value = model.Value;
            return _model;
        }
    }
}
