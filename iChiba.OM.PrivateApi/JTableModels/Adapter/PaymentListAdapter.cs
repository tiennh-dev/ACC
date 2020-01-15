
using iChiba.OM.PrivateApi.AppModel.Request.Payment;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class PaymentListAdapter
    {
        public static PaymentListRequest ToModel(this PaymentListJTableModel model)
        {
            PaymentListRequest _model = JTableModelAdapter.ToModel<PaymentListJTableModel, PaymentListRequest>(model);
            _model.AccountId = model.AccountId;
            _model.Description = model.Description;
            _model.StartTime = model.StartTime;
            _model.PaymentForm = model.PaymentForm;
            _model.PaymentType = model.PaymentType;
            _model.EndTime = model.EndTime;
            _model.Keyword = model.Search.Value;
            _model.State = model.State;
            _model.RefCode = model.RefCode;
            _model.Status = model.Status;
            return _model;
        }
    }
}
