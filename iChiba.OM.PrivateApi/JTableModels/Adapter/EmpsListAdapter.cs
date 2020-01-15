using iChiba.OM.PrivateApi.AppModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class EmpsListAdapter
    {
        public static CustomerListRequest ToModel(this EmpsJTable model)
        {
            CustomerListRequest _model = JTableModelAdapter.ToModel<EmpsJTable, CustomerListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.careBy = model.careBy;

            return _model;
        }
    }
}
