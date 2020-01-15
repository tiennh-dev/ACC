using iChiba.OM.PrivateApi.AppModel.Request.Empolyess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class EmployessListAdapter
    {
        public static EmployessAddRequest ToModel(this EmployessListJTableModel model)
        {
            var _model = JTableModelAdapter.ToModel<EmployessListJTableModel, EmployessAddRequest>(model);

            _model.Keyword = model.Search.Value;

            return _model;
        }
    }
}
