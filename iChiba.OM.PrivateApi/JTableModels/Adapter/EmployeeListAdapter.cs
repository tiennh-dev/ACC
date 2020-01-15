using iChiba.OM.PrivateApi.AppModel.Request.WarehouseEmp;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class EmployeeListAdapter
    {
        public static EmployeeListRequest ToModel(this EmployeeListJTableModel model)
        {
            EmployeeListRequest _model = JTableModelAdapter.ToModel<EmployeeListJTableModel, EmployeeListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.WarehouseId = model.WarehouseId;
            _model.Email = model.Email;
            _model.Phone = model.Phone;
            _model.UserName = model.UserName;

            return _model;
        }
    }
}
