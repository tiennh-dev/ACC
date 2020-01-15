using Core.AppModel.Response;
using Core.Resilience.Http;
using Ichiba.PurchaseReport.Api.Driver.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ichiba.PurchaseReport.Api.Driver
{
    public class PurchaseReportUrlConfig
    {
        public string PurchaseReportLoad { get; set; }
    }
    public class PurchaseReportUrlClient : BaseClient
    {
        private readonly PurchaseReportUrlConfig purchaseReportLoad;

        public PurchaseReportUrlClient(IHttpClient httpClient,
            IAuthorizeClient authorizeClient,
           PurchaseReportUrlConfig purchaseReportLoad)
            : base(httpClient, authorizeClient)
        {
            this.purchaseReportLoad = purchaseReportLoad;
        }

        public async Task<BaseResponse> PurchaseReportLoad(PurchaseReportLoadRequestRequest request)
        {
            var url = purchaseReportLoad.PurchaseReportLoad;
            var response = await PostAsync<BaseResponse, PurchaseReportLoadRequestRequest>(url, request);

            return response;
        }
    }
}
