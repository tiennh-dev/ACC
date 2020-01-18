using iChiba.ACC.PrivateApi.AppModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.JTableModels.Adapter
{
    public static class AccountJtableAdapter
    {
        public static AccountListRequest ToModel(this AccountJtableModel model)
        {
            var _model = JTableModelAdapter.ToModel<AccountJtableModel, AccountListRequest>(model);
            _model.Keyword = model.Keyword;
            _model.Name = model.Name;
            _model.Type = model.Type;
            _model.Actives = model.Actives;

            return _model;
        }
    }
}
