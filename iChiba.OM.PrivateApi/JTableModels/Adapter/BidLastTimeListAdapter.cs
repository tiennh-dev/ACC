using iChiba.OM.PrivateApi.AppModel.Request.BidLastTime;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class BidLastTimeListAdapter
    {
        public static BidLastTimeRequest ToModel(this BidLastTimeJTableModel model)
        {
            BidLastTimeRequest _model = JTableModelAdapter.ToModel<BidLastTimeJTableModel, BidLastTimeRequest>(model);
            _model.Keyword = model.Keyword;
            _model.ProductId = model.ProductId;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            _model.AccountId = model.AccountId;
            _model.Status = model.Status;
            _model.ProductName = model.ProductName;
            return _model;
        }
    }
}
