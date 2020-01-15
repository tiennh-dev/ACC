using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using Ichiba.IS4.Api.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Utilities
{
    public class PermissionFilter : IAsyncActionFilter
    {
        private readonly AccessClient accessClient;
        private readonly AppConfig appConfig;

        public string ResourceKey { get; set; }
        public ActionPermission[] Actions { get; set; }

        public PermissionFilter(AccessClient accessClient,
            AppConfig appConfig,
            string resourceKey,
            params ActionPermission[] actions)
        {
            this.accessClient = accessClient;
            this.appConfig = appConfig;
            ResourceKey = resourceKey;
            Actions = actions;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var isAnonymousAction = context.ActionDescriptor
                    .FilterDescriptors
                    .Any(m => m.Filter.GetType() == typeof(AllowAnonymousFilter));

                if (isAnonymousAction)
                {
                    await next();

                    return;
                }

                var resourceKey = ResourceKey;

                if (string.IsNullOrWhiteSpace(resourceKey))
                {
                    var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
                    resourceKey = descriptor.ControllerName;
                }

                var actions = Actions.Select(item => item.ToString()).ToArray();
                var isAccessAllow = await accessClient.CheckPermission(appConfig.AppGroupResourceKey, resourceKey, actions);

                if (!isAccessAllow)
                {
                    throw new UnauthorizedAccessException();
                }

                await next();
            }
            catch (UnauthorizedAccessException)
            {
                context.Result = new ContentResult()
                {
                    Content = "Forbidden",
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
        }
    }

    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string resourceKey, params ActionPermission[] actions)
            : base(typeof(PermissionFilter))
        {
            Arguments = new object[] 
            {
                resourceKey,
                actions
            };
        }

        public PermissionAttribute(params ActionPermission[] actions)
            : this("", actions)
        {
        }
    }

    public enum ActionPermission
    {
        ACCESS,
        ACTIVE,
        UNACTIVE,
        ACTIVE_UNACTIVE,
        ACTIVE_BID_VIP,
        ADD,
        ADD_PAY_ORDER,
        APPROVE,
        APPROVE_L1,
        APPROVE_L2,
        APPROVE_L3,
        BAO_GIA,
        DELETE,
        DUYET_TAMUNG,
        DUYET_THANHTOAN,
        EDIT,
        EXPORT,
        HUY_DH,
        IMPORT,
        MUA_HANG,
        OPEN,
        REJECT,
        SET_PASSWORD,
        SORT,
        VIEW_ALL_CUSTOMER,
        YC_DUYET_TAMUNG,
        YC_HUY
    }
}
