using iChiba.OM.PrivateApi.AppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
   public static class BidExternalConfigListAdapter
    {
        public static BidExternalConfigListRequest ToModel(this BidExternalConfigListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<BidExternalConfigListJTableModel, BidExternalConfigListRequest>(model);
            _model.AccountId = model.AccountId;
            _model.YAUserName = model.YAUserName;
            _model.Status = model.Status;
            _model.Description = model.Description;
            _model.searchKeyword = model.Search.Value;
            return _model;
        }
    }
}
