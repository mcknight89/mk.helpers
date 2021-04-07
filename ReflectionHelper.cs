using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
    public static class ReflectionHelper
    {
        public static void CopyProperties(this object source, object destination, params string[] ignore)
        {
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            var typeDest = destination.GetType();
            var typeSrc = source.GetType();
            var srcProps = typeSrc.GetProperties();
            foreach (var srcProp in srcProps.Where(p => !ignore.Any(i => string.Equals(i, p.Name, StringComparison.CurrentCultureIgnoreCase))))
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                var targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }
    }
}
