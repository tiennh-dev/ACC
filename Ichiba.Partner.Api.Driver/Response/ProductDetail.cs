using System.Collections.Generic;

namespace Ichiba.Partner.Api.Driver.Response
{
    public class ProductDetail
    {
        public string Domain { get; set; }
        public string SourceName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SellerId { get; set; }
        public long Price { get; set; }
        public int? Quantity { get; set; }
        public bool IsOutOfStock { get; set; }
        public IList<string> Images { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
    }
}
