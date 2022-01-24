using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace SABIO.ClientApi.Extensions
{

    public static class CacheExtensions
    {
        public class CacheExecutionInfo<T>
        {
            public CacheExecutionInfo(T result, string key, bool isNewEntry)
            {
                Result = result;
                Key = key;
                IsNewEntry = isNewEntry;
            }

            public T Result { get; }
            public string Key { get; }
            public bool IsNewEntry { get; }
        }
        
        public static CacheExecutionInfo<T> ExecuteWithCache<TInstance, T>(this TInstance client, IMemoryCache cache, Expression<Func<TInstance, T>> expression, MemoryCacheEntryOptions entryOptions)
        {
            var cacheKey = GetCacheKey(expression);
            bool isNewEntry = false;
            return new CacheExecutionInfo<T>(cache.GetOrCreate(cacheKey, entry =>
            {
                isNewEntry = true;
                if (entryOptions != null)
                    entry.SetOptions(entryOptions);
                return expression.Compile()(client);
            }), cacheKey, isNewEntry);
        }

        private static string GetCacheKey<TInstance, T>(Expression<Func<TInstance, T>> expression)
        {
            var methodCallExpression = (MethodCallExpression)expression.Body;
            var objectName = methodCallExpression.Object?.ToString().Split('.').Last();
            var parameters = ReadParameters(methodCallExpression);
            var paramStr = string.Join(",", parameters.Select(x => x.Key + "=" + x.Value).ToArray());

            return $"{typeof(TInstance).FullName}_{typeof(T).FullName}={objectName}.{methodCallExpression.Method.Name}({paramStr})";
        }


        private static IDictionary<string, object> ReadParameters(MethodCallExpression methodCallExpr)
        {
            ParameterInfo[] actionParameters = methodCallExpr.Method.GetParameters();
            MethodInfo dictionaryAdd = ((MethodCallExpression)((Expression<Action<Dictionary<string, object>>>)(d => d.Add(string.Empty, null))).Body).Method;

            var result = new Dictionary<string, object>();
            if (actionParameters.Any())
            {
                var argExpression =
                    Expression.Lambda<Func<Dictionary<string, object>>>(
                        Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)),
                            methodCallExpr.Arguments.Select((a, i) => Expression.ElementInit(dictionaryAdd, Expression.Constant(actionParameters[i].Name), Expression.Convert(a, typeof(object))))));

                var parameterValueGetter = argExpression.Compile();
                result = parameterValueGetter();
            }
            return result;
        }
    }
}