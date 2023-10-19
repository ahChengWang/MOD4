using AspectCore.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Helper;

namespace Utility.Attributes
{
    public class CacheListAttribute : AbstractInterceptorAttribute
    {
        private int _expireSeconds = 300;
        private bool _isRefresh = true;
        private string _identityItem = "Id";

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var method = context.ImplementationMethod;
            try
            {
                IList result = (IList)Activator.CreateInstance(method.ReturnType);
                var cacheOption = method.GetCustomAttribute<CacheOptionAttribute>();
                if (cacheOption != null)
                {
                    _expireSeconds = cacheOption.Second;
                    _isRefresh = cacheOption.Refresh;
                    _identityItem = cacheOption.IdentityItem;
                }

                Dictionary<string, string> missCacheList = new Dictionary<string, string>();
                Dictionary<string, string> identityAndKey = new Dictionary<string, string>();
                if (context.Parameters != null && context.Parameters[0] is IEnumerable array)
                {
                    foreach (var i in array)
                    {
                        var k = i.ToString();
                        if (!identityAndKey.ContainsKey(k))
                        {
                            identityAndKey.Add(k, CacheKeyFunction.GeneratMethodKey(method, new object[] { i }));
                        }
                    }

                    var valueList = CatchHelper
                        .MGet<string>(identityAndKey.Select(i => i.Value), _expireSeconds, _isRefresh);
                    if (valueList != null && valueList.Any())
                    {
                        foreach (var t in identityAndKey)
                        {
                            var temp = valueList.Where(i => i.Key == t.Value);
                            if (temp.Any())
                            {
                                var tempResult = JsonConvert.DeserializeObject(temp.FirstOrDefault().Value
                                    , method.ReturnType.GetGenericArguments()[0]);
                                if (tempResult is IList tempArray)
                                {
                                    foreach (var i in tempArray)
                                        result.Add(i);
                                }
                                else
                                    result.Add(tempResult);
                            }
                            else
                                missCacheList.Add(t.Key, t.Value);
                        }
                    }
                    else
                        missCacheList = identityAndKey;

                    if (missCacheList.Any())
                    {
                        await next.Invoke(context);
                        var methodResult = context.IsAsync()
                            ? await context.UnwrapAsyncReturnValue()
                            : context.ReturnValue;
                        if (methodResult != null && methodResult is IList tempArray)
                        {
                            Dictionary<string, string> keyValue = new Dictionary<string, string>();
                            foreach (var i in tempArray)
                            {
                                var methodResultIdentity = i.GetType().GetProperty(_identityItem).GetValue(i);
                                if (missCacheList.TryGetValue(methodResultIdentity.ToString(), out string tempKey))
                                {
                                    var setValue = JsonConvert.SerializeObject(i);
                                    keyValue.Add(tempKey, setValue);
                                    result.Add(i);
                                }
                            }
                            CatchHelper.MSet(keyValue, _expireSeconds);
                        }
                    }
                }
                else
                {
                    await next.Invoke(context);
                }
            }
            catch (Exception) { }
        }
    }
}
