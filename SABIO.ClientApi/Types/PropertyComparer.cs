using System;
using System.Collections.Generic;

namespace SABIO.ClientApi.Types
{
    public sealed class PropertyComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> _propertySelectoFunc;

        public PropertyComparer(Func<T, object> propertySelectoFunc)
        {
            _propertySelectoFunc = propertySelectoFunc;
        }

        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            var xVal = _propertySelectoFunc(x);
            var yVal = _propertySelectoFunc(y);
            if (ReferenceEquals(xVal, yVal)) return true;
            if (ReferenceEquals(xVal, null)) return false;
            if (ReferenceEquals(yVal, null)) return false;
            if (xVal.GetType() != yVal.GetType()) return false;
            return xVal.Equals(yVal);
        }

        public int GetHashCode(T obj)
        {
            var v = _propertySelectoFunc(obj);
            return (v != null ? v.GetHashCode() : 0);
        }
    }
}