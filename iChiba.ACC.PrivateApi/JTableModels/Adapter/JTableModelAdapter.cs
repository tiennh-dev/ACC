using Core.AppModel.Request;
using Core.Common;
using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.JTableModels.Adapter
{
    public static class JTableModelAdapter
    {
        public static TResult ToModel<TRequest, TResult>(this TRequest model)
          where TRequest : JTableModel
          where TResult : SortRequest, new()
        {
            return new TResult()
            {
                PageIndex = model.CurrentPage,
                PageSize = model.Length,
                Sorts = new Sorts(model.GetSortedColumns()
                    .Select(m => new Sort()
                    {
                        SortBy = m.PropertyName,
                        SortDirection = (m.Direction == SortingDirection.Descending ? Sort.SORT_DIRECTION_DESC : Sort.SORT_DIRECTION_ASC)
                    })
                    .ToList())
            };
        }
    }
}
