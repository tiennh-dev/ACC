using Core.Common.JTable;
using iChiba.OM.PrivateApi.AppModel.Model.Orderdetail;
using System;
using System.Collections.Generic;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class OrderBuyForYouListJTableModel : JTableModel
    {
        public string Code { get; set; }
        public string RefType { get; set; }
        public string AccountId { get; set; }
        public string Keyword { get; set; }
        public string Ref { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int[] Status { get; set; }
        public string[] State { get; set; }
        public string Seller { get; set; }
        public string Saler { get; set; }
        public string Tracking { get; set; }
        public string PreState { get; set; }
        public string PreOrderType { get; set; }
        public int PreStatus { get; set; }
        public int? statusTracking { get; set; }
        public string orderNumber { get; set; }

    }
}
