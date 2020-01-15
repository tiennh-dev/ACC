using iChiba.OM.PrivateApi.AppModel.Request.Order;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class OrderListAdapter
    {

        public static OrderListRequest ToModel(this OrderListJTableModel model)
        {
            OrderListRequest _model = JTableModelAdapter.ToModel<OrderListJTableModel, OrderListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AccountId = model.AccountId;
            _model.YauserName = model.YauserName;
            _model.AuctionId = model.AuctionId;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            _model.Status = model.Status;
            _model.OrderType = model.OrderType;
            _model.Code = model.Code;
            _model.State = model.State;
            _model.BidAccount = model.BidAccount;
            _model.Description = model.Description;
            _model.RefType = model.RefType;
            _model.ShippingFree = model.ShippingFree;
            _model.Paid = model.Paid;
            _model.Tracking = model.Tracking;
            _model.Saler = model.Saler;
            _model.PreState = model.PreState;
            _model.PreStatus = model.PreStatus;
            _model.PreTracking = model.PreTracking;
            _model.preProductType = model.preProductType;
            _model.Weight = model.Weight;
            _model.TrackingStatus = model.TrackingStatus;

            return _model;
        }
    }
}
