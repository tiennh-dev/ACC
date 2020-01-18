using iChiba.ACC.PrivateApi.AppService.Implement;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi
{
    //public class DependencyApp
    //{
    //    private readonly ILogger<DependencyApp> _logger;
    //    private readonly OrderChangeHandle orderUpdateHandle;

    //    public DependencyApp(ILogger<DependencyApp> logger,
    //        OrderChangeHandle orderUpdateStatusHandle)
    //    {
    //        _logger = logger;
    //        orderUpdateHandle = orderUpdateStatusHandle;
    //    }

    //    public void Start()
    //    {
    //        try
    //        {
    //            _ = Task.Run(() => orderUpdateHandle.Start());
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e.Message);
    //        }
    //    }

    //    public void Stop()
    //    {
    //        try
    //        {
    //            orderUpdateHandle.Stop();
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e.Message);
    //        }
    //    }
    //}
}
