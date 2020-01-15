using iChiba.OM.PrivateApi.AppModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class FavoriteSellerListAdapter
    {
        public static FavoriteSellerListRequest ToModel(this FavoriteSellerListJTableModel model)
        {
            FavoriteSellerListRequest _model = JTableModelAdapter.ToModel<FavoriteSellerListJTableModel, FavoriteSellerListRequest>(model);
            _model.AccountId = model.AccountId;
            return _model;
        }
    }
}
