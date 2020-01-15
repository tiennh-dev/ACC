using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Request.WarehouseEmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class WarehouseEmpListAdapter
    {
        public static WarehouseEmpListRequest ToModel(this WarehouseEmpListJTableModel model)
        {
            WarehouseEmpListRequest _model = JTableModelAdapter.ToModel<WarehouseEmpListJTableModel, WarehouseEmpListRequest>(model);

            _model.Keyword = model.Search.Value;
            _model.WarehouseId = model.WarehouseId;

            return _model;
        }
    }
}
