using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Request.FavoriteProduct;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class FavoriteProductListAdapter
    {
        public static FavoriteProductListRequest ToModel(this FavoriteProductListJTableModel model)
        {
            FavoriteProductListRequest _model = JTableModelAdapter.ToModel<FavoriteProductListJTableModel, FavoriteProductListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.AccountId = model.AccountId;
            return _model;
        }
    }
}
