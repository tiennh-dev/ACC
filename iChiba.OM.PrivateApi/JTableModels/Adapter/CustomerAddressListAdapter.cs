using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class CustomerAddressListAdapter
    {
        public static CustomerAddressListRequest ToModel(this CustomerAddressListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<CustomerAddressListJTableModel, CustomerAddressListRequest>(model);

            _model.CustomerId = model.CustomerId;
            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
