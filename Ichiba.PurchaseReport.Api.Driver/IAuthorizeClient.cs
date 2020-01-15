using System.Threading.Tasks;

namespace Ichiba.PurchaseReport.Api.Driver
{
    public interface IAuthorizeClient
    {
        Task<string> GetAuthorizeToken();
    }
}
