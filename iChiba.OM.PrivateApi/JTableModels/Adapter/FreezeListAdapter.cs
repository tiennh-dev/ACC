using iChiba.OM.PrivateApi.AppModel.Request.Freeze;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class FreezeListAdapter
    {
        public static FreezeListRequest ToModel(this FreezeListJTableModel model)
        {
            FreezeListRequest _model = JTableModelAdapter.ToModel<FreezeListJTableModel, FreezeListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AccountId = model.AccountId;
            _model.Description = model.Description;
            _model.Type = model.Type;
            _model.Ref = model.Ref;
            return _model;
        }
    }
}
