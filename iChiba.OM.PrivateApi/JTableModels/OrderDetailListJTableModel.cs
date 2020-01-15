using Core.Common.JTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class OrderDetailListJTableModel:JTableModel
    {
        public string Keyword { get; set; }
        public int Id { get; set; }
    }
}
