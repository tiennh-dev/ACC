using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class CustomerListAdapter
    {
        public static CustomerListRequest ToModel(this CustomerListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<CustomerListJTableModel, CustomerListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.UserName = model.UserName;
            _model.Email = model.Email;
            _model.Phone = model.Phone;
            _model.CustomerName = model.CustomerName;
            _model.BidActive = model.BidActive;
            _model.Code = model.Code;
            _model.Saler = model.Saler;
            _model.Group = model.Group;
            _model.PrePhone = model.PrePhone;
            _model.PreEmail = model.PreEmail;
            _model.PreCareBy = model.PreCareBy;
            return _model;
        }
    }
}
