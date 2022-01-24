using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using SABIO.ClientApi.Extensions;

namespace SABIO.ClientApi
{
    public static class Check
    {
        private const string argumentNullExceptionMessage = "Parameter '{0}' in method '{1}' is null";

        public static void TryCatch(Action action)
        {
            TryCatch<object, Exception>(() =>
                {
                    action();
                    return null;
                }
            );
        }
        public static TResult TryCatch<TResult>(Func<TResult> action)
        {
            return TryCatch<TResult, Exception>(action);
        }

        public static TResult TryCatch<TResult, TException>(Func<TResult> action) where TException : Exception
        {
            try
            {
                return action();
            }
            catch (TException e)
            {
                return default(TResult);
            }
        }

        public static void Requires(bool condition, Func<Exception> exceptionCreateFactory)
        {
            if (!condition)
                throw exceptionCreateFactory();
        }

        public static void Requires<TException>(bool condition)
            where TException : Exception, new()
        {
            if (!condition)
                throw new TException();
        }

        public static void Requires<TException>(bool condition, string message)
            where TException : Exception
        {
            if (!condition)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        public static T NotNull<T>(T instance, string parameterName, PropertyInfo[] properties, [CallerMemberName] string method = "")
        {
            foreach (var property in properties)
                NotNull(property.GetValue(NotNull(instance)), $"{parameterName}.{property.Name}", method);
            return instance;
        }
        
        public static T NotNull<T, TAttribute>(T instance, string parameterName, [CallerMemberName] string method = "")
            where TAttribute : Attribute
        {
            return NotNull(instance, parameterName, typeof(T).GetProperties().Where(info => info.GetCustomAttributes<TAttribute>().Any()).ToArray(), method);
        }

        public static T NotNull<T, TAttribute>(Expression<Func<T>> instance, [CallerMemberName] string method = "")
            where TAttribute : Attribute
        {
            return NotNull<T, TAttribute>(instance.Compile()(), instance.GetMemberName(), method);
        }

        public static T NotNull<T>(T instance, params Expression<Func<T, object>>[] propertiesToCheck)
        {
            return propertiesToCheck != null && propertiesToCheck.Any()
                ? propertiesToCheck.All(expression => NotNull(expression) != null) ? instance : throw new ArgumentException()
                : NotNull(instance, "instance");
        }

        public static T NotNull<T>(Expression<Func<T>> parameter, [CallerMemberName] string method = "")
        {
            var parameterToCheck = parameter.Compile()();
            return NotNull(parameterToCheck, parameter.GetMemberName(), method);
        }

        public static T NotNull<T>(T parameter, string parameterName, [CallerMemberName] string method = "")
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName, string.Format(argumentNullExceptionMessage, parameterName, method));
            return parameter;
        }

        public static Guid NotNull(Guid parameter, string parameterName, [CallerMemberName] string method = "")
        {
            if (parameter == Guid.Empty)
                throw new ArgumentNullException(parameterName, string.Format(argumentNullExceptionMessage, parameterName, method));
            return parameter;
        }

        public static string NotNullOrEmpty(string parameter, string parameterName, [CallerMemberName] string method = "")
        {
            if (string.IsNullOrEmpty(parameter))
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, parameterName, method));
            return parameter;
        }
    }
}