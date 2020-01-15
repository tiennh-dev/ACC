using Core.Common.JTable;
using System;
using System.Collections.Generic;

namespace iChiba.OM.PrivateApi.JTableModels
{
    public class OrderListJTableModel : JTableModel
    {
        public string OrderType { get; set; }
        public string RefType { get; set; }
        public string Keyword { get; set; }
        public string AccountId { get; set; }
        public string YauserName { get; set; }
        public string AuctionId { get; set; }
        public int[] Status { get; set; }
        public int[] TrackingStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Code { get; set; }
        public string PreCode { get; set; }
        public string Description { get; set; }
        public string[] State { get; set; }
        public string BidAccount { get; set; }
        public int? ShippingFree { get; set; }
        public int? Paid { get; set; }
        public string Tracking { get; set; }
        public string Saler { get; set; }
        public string PreState { get; set; }
        public int PreStatus { get; set; }
        public int? PreTracking { get; set; }
        public int? preProductType { get; set; }
        public string preOrderType { get; set; }
        public int? Weight { get; set; }
    }
}
