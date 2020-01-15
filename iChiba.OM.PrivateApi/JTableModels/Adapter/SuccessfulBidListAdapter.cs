using iChiba.OM.PrivateApi.AppModel.Request;
namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class SuccessfulBidListAdapter
    {
        public static SuccessfulBidListRequest ToModel(this SuccessfulBidListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<SuccessfulBidListJTableModel, SuccessfulBidListRequest>(model);

            _model.YauserName = model.YauserName;
            _model.SearchKeyword = model.Search.Value;
            _model.Keyword = model.Keyword;
            _model.AccountId = model.AccountId;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            _model.PaymentStatus = model.PaymentStatus;
            _model.PreCode = model.PreCode;
            _model.saler = model.saler;
            return _model;
        }
    }
}
