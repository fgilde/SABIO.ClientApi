using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;

namespace SABIO.ClientApi.Extensions
{
    public static class CoreExtensions
    {

        public static IEnumerable<T> Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            var result = enumerable.ToArray();
            foreach (var item in result)
                action(item);
            return result;
        }

        public static IEnumerable<TSource> Recursive<TSource>(this IEnumerable<TSource> children, Func<TSource, IEnumerable<TSource>> childDelegate)
        {
            return Recursive(children, childDelegate, source => true);
        }

        public static IEnumerable<TSource> Recursive<TSource>(this IEnumerable<TSource> children, Func<TSource, IEnumerable<TSource>> childDelegate, Func<TSource, bool> predicate)
        {
            foreach (var source in children)
            {
                var grandchildren = childDelegate(source);
                foreach (var grandchild in Recursive(grandchildren, childDelegate, predicate))
                    yield return grandchild;
                if (predicate(source))
                    yield return source;
            }
        }

        public static T SetProperties<T>(this T instance,  params Action<T>[] actions)
        {
            foreach (var action in actions)
                action(instance);
            return instance;
        }

        public static string ToQueryString(this object obj, string firstDelimiter = "")
        {
            var properties = from p in obj.GetType().GetProperties()
                let value = GetValue(obj, p)
                where value != null
                select GetKeyName(p) + "=" + HttpUtility.UrlEncode(value.ToString());

            return firstDelimiter + string.Join("&", properties.ToArray());
        }
        
        public static string GetKeyName(MemberInfo p)
        {            
            return (p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? p.Name).ToLower();
        }

        public static string GetMemberName<TParam>(this Expression<Func<TParam>> expression)
        {
            return MemberName((MemberExpression)expression.Body);
        }

        public static string GetMemberName<TParam, TResult>(this Expression<Func<TParam, TResult>> expression)
        {
            return MemberName((MemberExpression) expression.Body);
        }

        public static T ExposeField<T>(this object instance, string fieldName)
        {
            return Check.TryCatch<T>(() =>
            {
                BindingFlags flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                PropertyInfo info = instance.GetType().GetProperty(fieldName, flags);
                if (info != null)
                    return (T)info.GetValue(instance);
                return (T)instance.GetType().GetField(fieldName, flags)?.GetValue(instance);
            });
        }

        private static string MemberName(MemberExpression expression)
        {
            var memberExpression = expression;
            string name = memberExpression.Member.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ??
                          memberExpression.Member.Name.First().ToString().ToLower() + memberExpression.Member.Name.Substring(1);
            return name;
        }

        private static object GetValue(object obj, PropertyInfo p)
        {
            var res = p.GetValue(obj, null);
            if (res == null || p.PropertyType.IsValueType || p.PropertyType.IsAssignableFrom(typeof(string)))
                return res;
            return JsonConvert.SerializeObject(res);
        }
    }
}