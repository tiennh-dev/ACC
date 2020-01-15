using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class WithDrawalListAdapter
    {
        public static WithDrawalListRequest ToModel(this WithDrawalListJTableModel model)
        {
            WithDrawalListRequest _model = JTableModelAdapter.ToModel<WithDrawalListJTableModel, WithDrawalListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AcccountId = model.AccountId;
            _model.WithDrawalStatus = model.WithDrawalStatus;
            _model.BankNumber = model.BankNumber;
            _model.BankAccountName = model.BankAccountName;
            _model.state = model.state;

            return _model;
        }
    }
}
