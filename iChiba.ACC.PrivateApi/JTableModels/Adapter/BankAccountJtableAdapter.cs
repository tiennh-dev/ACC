using iChiba.ACC.PrivateApi.AppModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.JTableModels.Adapter
{
    public static class BankAccountJtableAdapter
    {
        public static BankAccountListRequest ToModel(this BankAccountJtableModel model)
        {
            var _model = JTableModelAdapter.ToModel<BankAccountJtableModel, BankAccountListRequest>(model);
            _model.Keyword = model.Keyword;
            _model.BankAccount = model.BankAccount;
            _model.BankName = model.BankName;
            _model.Actives = model.Active;
            _model.Owner = model.Owner;

            return _model;
        }
    }
}
