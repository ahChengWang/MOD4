using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.Helper
{
    public class CatchHelper
    {
        private static CSRedis.CSRedisClient _cacheService;
        private static int _expireSeconds;

        public CatchHelper(IConfiguration configuration)
        {
            _cacheService = new CSRedis.CSRedisClient($"{configuration.GetValue<string>("Redis:ConnectionString")}" +
                $"{configuration.GetValue<string>("Redis:OptionalSetting")}");
            _expireSeconds = configuration.GetValue<int>("Redis:DefaultTimeOut");
            RedisHelper.Initialization(_cacheService);
        }

        public static string Get(string key, int expireSeconds = 0, bool isResetTimeout = true)
            => PrivateGet<string>(key, expireSeconds, isResetTimeout);

        public static T Get<T>(string key, int expireSeconds = 0, bool isResetTimeout = true)
            => PrivateGet<T>(key, expireSeconds, isResetTimeout);

        private static T PrivateGet<T>(string key, int expireSeconds = 0, bool isResetTimeout = true, int retryCount = 1)
        {
            try
            {
                T result = RedisHelper.Get<T>(key);
                if (isResetTimeout && result != null)
                {
                    expireSeconds = expireSeconds == 0 ? _expireSeconds : expireSeconds;
                    RedisHelper.Expire(key, TimeSpan.FromSeconds(expireSeconds));
                }
                return result;
            }
            catch
            {
                if (retryCount > 0)
                    return PrivateGet<T>(key, expireSeconds, isResetTimeout, retryCount - 1);
                else
                    throw;
            }
        }

        public static void Set(string key, string data, int expireSeconds = 0)
            => PrivateSet(key, data, expireSeconds);

        public static void PrivateSet(string key, string data, int expireSeconds = 0, int retryCount = 1)
        {
            try
            {
                expireSeconds = expireSeconds == 0 ? _expireSeconds : expireSeconds;
                RedisHelper.Set(key, data, expireSeconds);
            }
            catch
            {
                if (retryCount > 0)
                    PrivateSet(key, data, expireSeconds, retryCount - 1);
                else
                    throw;
            }
        }

        public static bool SetLockKey(string key, string data, int expireSeconds = 0)
            => PrivateSetLockKey(key, data, expireSeconds);

        private static bool PrivateSetLockKey(string key, string data, int expireSeconds = 0, int retryCount = 1)
        {
            try
            {
                expireSeconds = expireSeconds == 0 ? _expireSeconds : expireSeconds;
                return RedisHelper.Set(key, data, expireSeconds, CSRedis.RedisExistence.Nx);
            }
            catch
            {
                if (retryCount > 0)
                    return PrivateSetLockKey(key, data, expireSeconds, retryCount - 1);
                else
                    throw;
            }
        }

        public static bool ResetExpire(string key, int expireSeconds = 0)
            => PrivateResetExpire(key, expireSeconds);

        private static bool PrivateResetExpire(string key, int expireSeconds = 0, int retryCount = 1)
        {
            try
            {
                return RedisHelper.Expire(key, expireSeconds);
            }
            catch
            {
                if (retryCount > 0)
                    return PrivateResetExpire(key, expireSeconds, retryCount - 1);
                else
                    throw;
            }
        }

        public static long Delete(params string[] key)
            => PrivateDelete(key);

        private static long PrivateDelete(string[] key, int retryCount = 1)
        {
            try { return RedisHelper.Del(key); }
            catch
            {
                if (retryCount > 0)
                    return PrivateDelete(key, retryCount - 1);
                else
                    throw;
            }
        }

        public static void MSet(Dictionary<string, string> keyValue, int expireSeconds = 0)
            => PrivateMSet(keyValue, expireSeconds);

        private static void PrivateMSet(Dictionary<string, string> keyValue, int expireSeconds = 0, int retryCount = 1)
        {
            try
            {
                var data = keyValue.SelectMany(i => new List<string> { i.Key, i.Value }).ToArray();
                _ = RedisHelper.MSet(data);

                var key = keyValue.Keys;
                expireSeconds = expireSeconds == 0 ? _expireSeconds : expireSeconds;
                Expire(key, expireSeconds);
            }
            catch
            {
                if (retryCount > 0)
                    PrivateMSet(keyValue, expireSeconds, retryCount - 1);
                else
                    throw;
            }
        }

        public static Dictionary<string, T> MGet<T>(IEnumerable<string> key, int expireSeconds = 0, bool isResetTimeout = true)
            => PrivateMGet<T>(key, expireSeconds, isResetTimeout);

        private static Dictionary<string, T> PrivateMGet<T>(IEnumerable<string> key, int expireSeconds = 0, bool isResetTimeout = true, int retryCount = 1)
        {
            try
            {
                var value = RedisHelper.MGet<T>(key.ToArray());
                var result = key
                    .Select((k, i) => new { k, v = value[i] })
                    .Where(kv => kv.v != null)
                    .ToDictionary(x => x.k, x => x.v);

                if (isResetTimeout && value.Any())
                {
                    expireSeconds = expireSeconds == 0 ? _expireSeconds : expireSeconds;
                    Expire(result.Keys, expireSeconds);
                }
                return result;
            }
            catch
            {
                if (retryCount > 0)
                    return PrivateMGet<T>(key, expireSeconds, isResetTimeout, retryCount - 1);
                else
                    throw;
            }
        }

        public static string[] Keys(string prefix)
            => PrivateKeys(prefix);

        private static string[] PrivateKeys(string prefix, int retryCount = 1)
        {
            try
            {
                return RedisHelper.Keys(pattern: $"{prefix}*");
            }
            catch
            {
                if (retryCount > 0)
                    return PrivateKeys(prefix, retryCount - 1);
                else
                    throw;
            }
        }

        private static void Expire(IEnumerable<string> key, int expireSeconds)
        {
            var t = TimeSpan.FromSeconds(expireSeconds);
            var pipe = RedisHelper.StartPipe();
            foreach (var k in key)
            {
                pipe.Expire(k, t);
            }
            pipe.EndPipe();
        }
    }
}
