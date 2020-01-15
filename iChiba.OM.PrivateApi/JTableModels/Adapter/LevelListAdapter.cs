using iChiba.OM.PrivateApi.AppModel.Request.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class LevelListAdapter
    {
        public static LevelListRequest ToModel(this LevelListJTableModel model)
        {
            LevelListRequest _model = JTableModelAdapter.ToModel<LevelListJTableModel, LevelListRequest>(model);
            _model.Keyword = model.Keyword;
            return _model;
        }
    }
}
