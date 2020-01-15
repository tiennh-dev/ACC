using Core.AppModel.Response;
using Core.CustomException;
using iChiba.ACC.CustomException;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.AppService.Implement
{
    public class BaseAppService
    {
        protected readonly ILogger logger;

        public BaseAppService(ILogger logger)
        {
            this.logger = logger;
        }

        protected virtual async Task<TResult> TryCatchAsync<TResult>(Func<Task<TResult>> tryFunction,
            BaseResponse response,
            Action finallyAction = null)
        {
            try
            {
                return await tryFunction.Invoke();
            }
            catch (ErrorCodeParameterException ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ex.ErrorCode, ex.Parameters);
            }
            catch (ErrorCodeException ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ex.ErrorCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ErrorCodeDefine.UNKNOW_ERROR);
            }
            finally
            {
                finallyAction?.Invoke();
            }

            return default(TResult);
        }

        protected virtual TResult TryCatchAsync<TResult>(Func<TResult> tryFunction,
            BaseResponse response,
            Action finallyAction = null)
        {
            try
            {
                return tryFunction.Invoke();
            }
            catch (ErrorCodeParameterException ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ex.ErrorCode, ex.Parameters);
            }
            catch (ErrorCodeException ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ex.ErrorCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ErrorCodeDefine.UNKNOW_ERROR);
            }
            finally
            {
                finallyAction?.Invoke();
            }

            return default(TResult);
        }

        protected virtual void TryCatch(Action tryAction,
            BaseResponse response,
            Action finallyAction = null)
        {
            try
            {
                tryAction.Invoke();
            }
            catch (ErrorCodeParameterException ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ex.ErrorCode, ex.Parameters);
            }
            catch (ErrorCodeException ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ex.ErrorCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                response.Fail(ErrorCodeDefine.UNKNOW_ERROR);
            }
            finally
            {
                finallyAction?.Invoke();
            }
        }

        protected virtual void EnsureStringIsNotNullOrWhiteSpace(string input, Action action)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                action.Invoke();
            }
        }

        protected virtual void EnsureStringIsNotNullOrWhiteSpace(string input, string errorCode)
        {
            EnsureStringIsNotNullOrWhiteSpace(input, () => throw new ErrorCodeException(errorCode));
        }

        protected virtual void EnsureStringIsNotNullOrWhiteSpace(params (string, string)[] values)
        {
            Array.ForEach(values, item => EnsureStringIsNotNullOrWhiteSpace(item.Item1, item.Item2));
        }

        protected virtual string ToDateTimeString(DateTime? dateTime) => dateTime != null ? dateTime.Value.ToString("MM/dd/yyyy HH:mm:ss") : string.Empty;
        protected virtual bool IsEquals(string left, string right) => string.CompareOrdinal(left, right) == 0;
        protected virtual string Join(char separator, params object[] inputs)
        {
            if (inputs == null)
            {
                return null;
            }

            var inputStandard = inputs.Select(m => m != null ? m.ToString() : string.Empty);

            return string.Join(separator, inputStandard);
        }
        protected virtual long Round(decimal input) => (long)Math.Round(input + 0.1m, MidpointRounding.AwayFromZero);
    }
}
