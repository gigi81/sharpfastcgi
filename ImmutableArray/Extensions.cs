using System;
using System.Reflection;

namespace Grillisoft.ImmutableArray
{
    internal static class Extensions
    {
#if !NET40
        internal static bool IsInstanceOfType(this Type type, object obj)
        {
            return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
        }
#endif
    }
}
