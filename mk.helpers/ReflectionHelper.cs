using mk.helpers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace mk.helpers
{
    /// <summary>
    /// Provides utility methods for reflection-related operations.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Checks if the provided type is a simple type (value type, primitive type, or simple reference type).
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns><c>true</c> if the type is a simple type; otherwise, <c>false</c>.</returns>
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


        /// <summary>
        /// Retrieves the underlying type of a member (event, field, method, or property).
        /// </summary>
        /// <param name="member">The member to retrieve the underlying type for.</param>
        /// <returns>The underlying type of the member.</returns>
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

        /// <summary>
        /// Compares the public properties of two objects of the same type and checks if they are equal.
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <param name="self">The first object.</param>
        /// <param name="to">The second object.</param>
        /// <param name="ignore">A list of property names to ignore during comparison.</param>
        /// <returns><c>true</c> if the public properties are equal; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Determines the changes between two objects and returns a list of <see cref="EntityChange"/> instances.
        /// </summary>
        /// <param name="oldEntity">The old object.</param>
        /// <param name="newEntity">The new object.</param>
        /// <returns>A list of <see cref="EntityChange"/> instances representing the changes.</returns>
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


        /// <summary>
        /// Copies the properties from the source object to the destination object and returns the changes made.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destination">The destination object.</param>
        /// <param name="changes">The list of changes made during copying.</param>
        public static void CopyPropertiesFrom(this object source, object destination, out List<EntityChange> changes)
        {
            CopyProperties(destination, source, out changes);
        }

        /// <summary>
        /// Copies the properties from the source object to the destination object, ignoring specified properties.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destination">The destination object.</param>
        /// <param name="ignore">An array of property names to ignore during copying.</param>
        public static void CopyPropertiesFrom(this object source, object destination, params string[] ignore)
        {
            CopyProperties(destination, source, ignore);
        }

        /// <summary>
        /// Copies the properties from the source object to the destination object and captures the changes made.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destination">The destination object.</param>
        /// <param name="changes">The list of changes made during copying.</param>
        public static void CopyProperties(this object source, object destination, out List<EntityChange> changes)
        {
            changes = destination.Changes(source);
            CopyProperties(source, destination);
        }


        /// <summary>
        /// Copies the properties from the source object to the destination object, ignoring specified properties, and captures the changes made.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destination">The destination object.</param>
        /// <param name="changes">The list of changes made during copying.</param>
        /// <param name="ignore">An array of property names to ignore during copying.</param>
        public static void CopyProperties(this object source, object destination, out List<EntityChange> changes, params string[] ignore)
        {
            changes = destination.Changes(source);
            CopyProperties(source, destination, ignore);
        }


        /// <summary>
        /// Checks if the provided type is a primitive type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type is a primitive type; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }


        /// <summary>
        /// Creates a deep clone of the provided object using JSON serialization.
        /// </summary>
        /// <typeparam name="T">The type of the object to clone.</typeparam>
        /// <param name="obj">The object to be cloned.</param>
        /// <returns>The deep-cloned object.</returns>
        public static T DeepClone<T>(this T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false, // Set to true if you want indented JSON
            };

            var serializedObj = JsonSerializer.Serialize(obj, options);
            return JsonSerializer.Deserialize<T>(serializedObj, options);
        }


        /// <summary>
        /// Copies the properties from the source object to the destination object, ignoring specified properties.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destination">The destination object.</param>
        /// <param name="ignore">An array of property names to ignore during copying.</param>
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
