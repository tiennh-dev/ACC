using Core.AppModel.Response;
using Ichiba.IS4.Api.Driver.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.AppService.Interface
{
    public interface IAccessAppService
    {
        Task<BaseResponse> CheckPermission(string resourceKey, string permission);
        Task<BaseEntityResponse<IList<Resource>>> GetResources();
    }
}
