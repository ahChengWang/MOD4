using AspectCore.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Helper;

namespace Utility.Attributes
{
    public class CacheAttribute : AbstractInterceptorAttribute
    {
        private int _expireSeconds = 300;
        private bool _isRefresh = true;

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var key = "";
            object result;
            var method = context.ImplementationMethod;
            try
            {
                var cacheOption = method.GetCustomAttribute<CacheOptionAttribute>();
                if (cacheOption != null)
                {
                    _expireSeconds = cacheOption.Second;
                    _isRefresh = cacheOption.Refresh;
                }

                key = CacheKeyFunction.GeneratMethodKey(method, context.Parameters);

                var value = CatchHelper.Get(key, expireSeconds: _expireSeconds, isResetTimeout: _isRefresh);
                if (value != null)
                {
                    context.ReturnValue = JsonConvert.DeserializeObject(value, method.ReturnType);
                    return;
                }
            }
            catch (Exception) { }

            await next.Invoke(context);
            result = context.IsAsync()
                ? await context.UnwrapAsyncReturnValue()
                : context.ReturnValue;

            try
            {
                var setValue = JsonConvert.SerializeObject(result);
                CatchHelper.Set(key, setValue, _expireSeconds);
            }
            catch (Exception) { }
        }
    }
}
