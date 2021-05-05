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
        public static bool IsSimpleType(
   this Type type)
        {
            return
               type.IsValueType ||
               type.IsPrimitive ||
               new[]
               {
               typeof(String),
               typeof(Decimal),
               typeof(DateTime),
               typeof(DateTimeOffset),
               typeof(TimeSpan),
               typeof(Guid)
               }.Contains(type) ||
               (Convert.GetTypeCode(type) != TypeCode.Object);
        }

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                       "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
        public static bool PublicPropertiesEqual<T>(this T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                var type = typeof(T);
                var ignoreList = new List<string>(ignore);
                var unequalProperties =
                    from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where !ignoreList.Contains(pi.Name) && pi.GetUnderlyingType().IsSimpleType() && pi.GetIndexParameters().Length == 0
                    let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                    let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                    where selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue))
                    select selfValue;
                return !unequalProperties.Any();
            }
            return self == to;
        }
        public static List<EntityChange> Changes(this object oldEntity, object newEntity)
        {
            List<EntityChange> Changes(object oldEntry, object newEntry, string prefixname = "")
            {
                List<EntityChange> logs = new List<EntityChange>();
                if (newEntry == null || newEntry == null)
                    return logs;

                var oldType = oldEntry.GetType();
                var newType = newEntry.GetType();
                if (oldType != newType)
                    return logs;

                var oldProperties = oldType.GetProperties();
                var newProperties = newType.GetProperties();

                foreach (var oldProperty in oldProperties)
                {
                    var matchingProperty = newProperties.Where(x => x.Name == oldProperty.Name
                                                                    && x.PropertyType == oldProperty.PropertyType)
                                                        .FirstOrDefault();
                    if (matchingProperty == null)
                        continue;

                    if (oldProperty.PropertyType.IsClass && oldProperty.PropertyType != typeof(string) &&
                        matchingProperty.PropertyType.IsClass && matchingProperty.PropertyType != typeof(string))
                    {
                        var oldObj = oldProperty.GetValue(oldEntry);
                        var newObj = matchingProperty.GetValue(newEntry);
                        if (oldObj != null && newObj != null)
                        {
                            var changes = Changes(oldObj, newObj, matchingProperty.Name);
                            if (changes.Any() == true)
                                logs.AddRange(changes);
                        }
                    }
                    else
                    {
                        var oldValue = oldProperty.GetValue(oldEntry);
                        var newValue = matchingProperty.GetValue(newEntry);
                        if (matchingProperty != null && oldValue != newValue)
                        {
                            logs.Add(new EntityChange()
                            {
                                Property = string.IsNullOrEmpty(prefixname) ? matchingProperty.Name : string.Concat(prefixname, ".", matchingProperty.Name),
                                OldValue = oldProperty.GetValue(oldEntry),
                                NewValue = matchingProperty.GetValue(newEntry),
                                PropertyType = matchingProperty.PropertyType,
                                NestedProperty = !string.IsNullOrEmpty(prefixname)
                            });
                        }
                    }
                }
                return logs;
            }
            return Changes(oldEntity, newEntity);
        }

        public static void CopyProperties(this object source, object destination, out List<EntityChange> changes)
        {
            changes = destination.Changes(source);
            CopyProperties(source, destination);
        }

        public static void CopyProperties(this object source, object destination, out List<EntityChange> changes, params string[] ignore)
        {
            changes = destination.Changes(source);
            CopyProperties(source, destination, ignore);
        }

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
                    continue;
                var targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                    continue;
                if (!targetProperty.CanWrite)
                    continue;
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                    continue;
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                    continue;
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                    continue;
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }
    }
}
