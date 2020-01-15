using iChiba.OM.PrivateApi.AppModel.Request;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class DepositsListAdapter
    {
        public static DepositsListRequest ToModel(this DepositsListJTableModel model)
        {
            DepositsListRequest _model = JTableModelAdapter.ToModel<DepositsListJTableModel, DepositsListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AccountId = model.AccountId;
            _model.DepositSt = model.DepositSt;
            _model.BankDescription = model.BankDescription;
            _model.BankNumber = model.BankNumber;
            _model.state = model.state;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            _model.FtCode = model.FtCode;
            _model.PayStatus = model.PayStatus;
            _model.DepositeType = model.DepositeType;
            return _model;
        }
    }
}
