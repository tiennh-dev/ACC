using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ichiba.PurchaseReport.Api.Driver;

namespace iChiba.OM.PrivateApi.Driver
{
    public class AuthorizeClientImplementPurchaseReport : IAuthorizeClient
    {
        public AuthorizeClientImplementPurchaseReport()
        {
        }

        public Task<string> GetAuthorizeToken()
        {
            return Task.FromResult(string.Empty);
        }
    }
     
}
