using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for object manipulation.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object into an <see cref="ExpandoObject"/>, copying all its properties.
        /// Complex types and collections are recursively converted.
        /// </summary>
        /// <param name="obj">The object to convert to <see cref="ExpandoObject"/>.</param>
        /// <returns>
        /// An <see cref="ExpandoObject"/> representing the original object's properties.
        /// Returns <c>null</c> if the input object is <c>null</c>.
        /// </returns>
        public static ExpandoObject ToExpandoObject(this object obj)
        {
            if (obj == null) return null;

            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in obj.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(obj);

                if (propertyValue != null && IsComplexType(propertyValue.GetType()))
                {
                    if (propertyValue is IEnumerable enumerable && !(propertyValue is string))
                    {
                        var itemList = new List<object>();
                        foreach (var item in enumerable)
                        {
                            itemList.Add(item.ToExpandoObject());
                        }
                        expando.Add(property.Name, itemList);
                    }
                    else
                    {
                        expando.Add(property.Name, propertyValue.ToExpandoObject());
                    }
                }
                else
                {
                    expando.Add(property.Name, propertyValue);
                }
            }

            return (ExpandoObject)expando;
        }

        /// <summary>
        /// Determines whether a given type is a complex type.
        /// A complex type is defined as a class that is not a string and is not an array.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the type is a complex type; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsComplexType(Type type)
        {
            return type.IsClass && type != typeof(string) && !type.IsArray;
        }

        /// <summary>
        /// Determines whether the specified object is contained in the given set of values.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <param name="values">The set of values to check against.</param>
        /// <returns><c>true</c> if the object is in the set of values; otherwise, <c>false</c>.</returns>
        public static bool IsIn<T>(this T obj, params T[] values)
        {
            return Array.IndexOf(values, obj) >= 0;
        }

        /// <summary>
        /// Converts an object's public properties to a dictionary.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>A dictionary containing the object's property names and values.</returns>
        public static IDictionary<string, object> AsDictionary(this object obj)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                dictionary.Add(property.Name, property.GetValue(obj));
            }
            return dictionary;
        }


        /// <summary>
        /// Determines whether the specified object is the default value for its type.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if the object is the default value; otherwise, <c>false</c>.</returns>
        public static bool IsDefault<T>(this T obj)
        {
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }


    }
}
