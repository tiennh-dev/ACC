using iChiba.OM.PrivateApi.AppModel.Request.PriceQuoteLastTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class PriceQuoteLastTimeListAdapter
    {
        public static PriceQuoteLastTimeListRequest ToModel(this PriceQuoteLastTimeListJTableModel model)
        {
            PriceQuoteLastTimeListRequest _model = JTableModelAdapter.ToModel<PriceQuoteLastTimeListJTableModel, PriceQuoteLastTimeListRequest>(model);
            _model.AccountId = model.AccountId;
            return _model;
        }
    }
}
