using iChiba.OM.PrivateApi.AppModel.Request.Order;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class OrderBuyForYouAdapter
    {

        public static OrderBuyForYouListRequest ToModel(this OrderBuyForYouListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<OrderBuyForYouListJTableModel, OrderBuyForYouListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AccountId = model.AccountId;            
            _model.Ref = model.Ref;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            _model.RefType = model.RefType;
            _model.Status = model.Status;
            _model.State = model.State;
            _model.Code = model.Code;
            _model.Tracking = model.Tracking;
            _model.PreState = model.PreState;
            _model.Saler = model.Saler;
            _model.PreOrderType = model.PreOrderType;
            _model.PreStatus = model.PreStatus;
            _model.statusTracking = model.statusTracking;
            _model.orderNumber = model.orderNumber;

            return _model;
        }
    }
}
