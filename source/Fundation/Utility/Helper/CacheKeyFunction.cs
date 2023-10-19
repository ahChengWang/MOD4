using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility.Attributes;

namespace Utility.Helper
{
    public static class CacheKeyFunction
    {
        private static readonly int _userCacheDefaultExpireSecond = 300;
        private static readonly Dictionary<string, string> _cacheKeyDic = new Dictionary<string, string>
        {
            { "AccountInfo","accInfo"}
        };

        #region Generate Key
        public static string GeneratMethodKey(MethodInfo methodInfo, object[] argument = null)
        {
            if (methodInfo == null)
                return null;
            return GenerateMethodKey(methodInfo.ReflectedType.Name, methodInfo.Name, argument);
        }
        public static string GeneratMethodKey<T>(string methodName, IEnumerable<object> argument = null)
        {
            var type = argument?.Select(i => i.GetType())?.ToArray() ?? new Type[0];
            var methodInfo = typeof(T).GetMethod(methodName, type);

            return GeneratMethodKey(methodInfo, argument.ToArray());
        }
        public static string GenerateMethodKey(string className, string methodName, object[] argument = null)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName))
                return null;
            className = className.Replace("Repository", "").Replace("DomainService", "");
            //return $"CacheKeyFunction:{className}:{methodName}{GenerateArgumentKey(argument)}";
            return _cacheKeyDic.ContainsKey(className) ? _cacheKeyDic[className] : $"CacheKeyFunction:{className}:{methodName}{GenerateArgumentKey(argument)}";
        }
        private static string GenerateArgumentKey(object[] argument)
        {
            var result = "";
            if (argument != null && argument.Length > 0)
            {
                var sb = new StringBuilder();
                var idx = 0;
                foreach (var para in argument)
                {
                    sb.Append($"-{idx.ToString()}");
                    if (para == null)
                    {
                        idx++;
                        continue;
                    }

                    if (para.GetType() == typeof(string))
                    {
                        sb.Append($"_{para}");
                    }
                    else if (para is System.Collections.IList array)
                    {
                        foreach (var item in array)
                        {
                            if (item.GetType().IsClass && !(para.GetType() != typeof(string)))
                            {
                                foreach (var propertyInfo in item.GetType().GetProperties())
                                {
                                    sb.Append($"_{propertyInfo.GetValue(item)}");
                                }
                            }
                            else
                            {
                                sb.Append($"_{item}");
                            }
                        }
                    }
                    else if (para is System.Collections.IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            if (item.GetType().IsClass && !(para.GetType() != typeof(string)))
                            {
                                foreach (var propertyInfo in item.GetType().GetProperties())
                                {
                                    sb.Append($"_{propertyInfo.GetValue(item)}");
                                }
                            }
                            else
                            {
                                sb.Append($"_{item}");
                            }
                        }
                    }
                    else if (para.GetType().IsClass)
                    {
                        var idxClass = 0;
                        foreach (var propertyInfo in para.GetType().GetProperties())
                        {
                            sb.Append($"+{idxClass.ToString()}");
                            var item = propertyInfo.GetValue(para);
                            if (item is System.Collections.IList items)
                            {
                                foreach (var i in items)
                                {
                                    sb.Append($"_{i}");
                                }
                            }
                            else
                            {
                                sb.Append($"_{item}");
                            }
                            idxClass++;
                        }
                    }
                    else
                    {
                        sb.Append($"_{para}");
                    }
                    idx++;
                }
                result += $"-{sb.ToString()}";
            }
            return result;
        }

        #endregion

        public static Dictionary<TKey, TData> GetMethodCache<TClass, TKey, TData>(string methodName, IEnumerable<TKey> keyData)
        {
            try
            {
                //Get Data
                var methodInfo = typeof(TClass).GetMethod(methodName, new Type[] { typeof(TKey) });
                int expireSecond;
                var attr = methodInfo.GetCustomAttributes(typeof(CacheOptionAttribute), true);
                if (attr.Length > 0)
                {
                    expireSecond = (attr[0] as CacheOptionAttribute)?.Second ?? _userCacheDefaultExpireSecond;
                }
                else
                {
                    expireSecond = _userCacheDefaultExpireSecond;
                }

                var keyPair = keyData
                    .Select(i => new { i, k = GeneratMethodKey(methodInfo, new object[] { i }) })
                    .ToDictionary(x => x.k, x => x.i);
                var userCache = CatchHelper.MGet<TData>(keyPair.Keys, expireSeconds: expireSecond);

                // Key-Value 比對
                var result = userCache
                    .Select(u => new { k = keyPair[u.Key], v = u.Value })
                    .ToDictionary(x => x.k, x => x.v);

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<TKey, TData>();
            }
        }

        #region SetMethodCache
        /// <summary>
        /// add List Data to Cache
        /// </summary>
        /// <typeparam name="TClass">Class Type</typeparam>
        /// <typeparam name="TKey">Key(argument) Type</typeparam>
        /// <typeparam name="TData">Data Type</typeparam>
        /// <param name="methodName">Method Name</param>
        /// <param name="data">Cache Data</param>
        public static void SetMethodCache<TClass, TKey, TData>(string methodName, Dictionary<TKey, TData> data)
        {
            try
            {
                var f = data.FirstOrDefault();
                int expireSecond;
                Dictionary<string, string> keyValuePair;
                if (f.Key is System.Collections.IEnumerable)
                {
                    var methodInfo = typeof(TClass).GetMethod(methodName, (f.Key as object[]).Select(i => i.GetType()).ToArray());
                    expireSecond = ((CacheOptionAttribute)methodInfo.GetCustomAttributes(typeof(CacheOptionAttribute), true)[0])?.Second ?? _userCacheDefaultExpireSecond;

                    keyValuePair = data
                        .Select(u => new
                        {
                            k = GeneratMethodKey(methodInfo, u.Key as object[]),
                            v = JsonConvert.SerializeObject(u.Value)
                        })
                        .ToDictionary(x => x.k, x => x.v);
                }
                else
                {
                    var methodInfo = typeof(TClass).GetMethod(methodName, new Type[] { f.Key.GetType() });
                    var attr = methodInfo.GetCustomAttributes(typeof(CacheOptionAttribute), true);
                    if (attr.Length > 0)
                    {
                        expireSecond = (attr[0] as CacheOptionAttribute)?.Second ?? _userCacheDefaultExpireSecond;
                    }
                    else
                    {
                        expireSecond = _userCacheDefaultExpireSecond;
                    }

                    keyValuePair = data
                        .Select(u => new
                        {
                            k = GeneratMethodKey(methodInfo, new object[] { u.Key }),
                            v = JsonConvert.SerializeObject(u.Value)
                        })
                        .ToDictionary(x => x.k, x => x.v);
                }

                CatchHelper.MSet(keyValuePair, expireSecond);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// add Data to Cache
        /// </summary>
        /// <typeparam name="TClass">Class Type</typeparam>
        /// <typeparam name="TKey">Key(argument) Type</typeparam>
        /// <typeparam name="TData">Data Type</typeparam>
        /// <param name="methodName">Method Name</param>
        /// <param name="argument">Argument</param>
        /// <param name="data">Cache Data</param>
        public static void SetMethodCache<TClass, TKey, TData>(string methodName, TKey argument, TData data)
            => SetMethodCache<TClass, TKey, TData>(methodName, new Dictionary<TKey, TData> { { argument, data } });

        #endregion

        #region Remove Cache
        /// <summary>
        /// Remove List Key From Cache
        /// </summary>
        /// <typeparam name="TClass">Class Type</typeparam>
        /// <typeparam name="TKey">Key(argument) Type</typeparam>
        /// <param name="methodName">Method Name</param>
        /// <param name="argument">Key(argument) Data</param>
        public static void RemoveMethodCache<TClass, TKey>(string methodName, IEnumerable<TKey> argument)
        {
            try
            {
                var f = argument.FirstOrDefault();
                List<string> key;

                if (f is System.Collections.IEnumerable list)
                {
                    var t = new List<Type>();
                    foreach (var x in list)
                    {
                        t.Add(x.GetType());
                    }
                    var methodInfo = typeof(TClass).GetMethod(methodName, t.ToArray());

                    key = argument
                        .Select(u =>
                        {
                            if (u is System.Collections.IEnumerable array)
                            {
                                var v = new List<object>();
                                foreach (var x in array)
                                {
                                    v.Add(x);
                                }

                                return GeneratMethodKey(methodInfo, v.ToArray());
                            }
                            return null;
                        })
                        .Where(u => u != null)
                        .ToList();
                }
                else
                {
                    var methodInfo = typeof(TClass).GetMethod(methodName, new Type[] { f.GetType() });
                    key = argument
                        .Select(u => GeneratMethodKey(methodInfo, new object[] { u }))
                        .ToList();
                }

                key.ForEach(i =>
                {
                    CatchHelper.Delete(i);
                });
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Remove Key From Cache
        /// </summary>
        /// <typeparam name="TClass">Class Type</typeparam>
        /// <typeparam name="TKey">Key(argument) Type</typeparam>
        /// <param name="methodName">Method Name</param>
        /// <param name="argument">Key(argument) Data</param>
        public static void RemoveMethodCache<TClass, TKey>(string methodName, TKey argument)
            => RemoveMethodCache<TClass, TKey>(methodName, new TKey[] { argument });

        /// <summary>
        /// Remove List Key From Cache (no argument)
        /// </summary>
        /// <typeparam name="TClass">Class Type</typeparam>
        /// <param name="methodName">Method Name</param>
        //public static void RemoveMethodCache<TClass>(string methodName)
        //{
        //    try
        //    {
        //        CacheHelper.Delete(
        //            GeneratMethodKey(typeof(TClass).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        //            );
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        #endregion

        /// <summary>
        /// 組出 Exception 的 ErrorMessage
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private static string GetErrorMessage(Exception ex, string methodName)
        {
            return $"{methodName} Exception: \n\r {ex.Message} \n\r {ex.StackTrace}"
                + (ex.InnerException != null ? $"\n\r InnerException: \n\r {ex.InnerException.Message } \n\r {ex.InnerException.StackTrace }" : "");
        }
    }
}
