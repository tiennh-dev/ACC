using iChiba.OM.PrivateApi.AppModel.Request.BankTransactionHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels.Adapter
{
    public static class BankTransactionHistoryListAdapter
    {
        public static BankTransactionHistoryListRequest ToModel(this BankTransactionHistoryListJTableModel model)
        {
            BankTransactionHistoryListRequest _model = JTableModelAdapter.ToModel<BankTransactionHistoryListJTableModel, BankTransactionHistoryListRequest>(model);
            _model.Keyword = model.Keyword;
            _model.AccountNumber = model.AccountNumber;
            _model.DebitAmount = model.DebitAmount;
            _model.CreditAmount = model.CreditAmount;
            _model.Description = model.Description;
            _model.StartTime = model.StartTime;
            _model.EndTime = model.EndTime;
            return _model;
        }
    }
}
