﻿//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;


//namespace mk.helpers
//{
//    /// <summary>
//    /// Provides extension methods for working with <see cref="DbSet{TEntity}"/> collections.
//    /// </summary>
//    public static class DbSetExtension
//    {
//        /// <summary>
//        /// Adds or updates an entity in the <see cref="DbSet{TEntity}"/> based on primary key values.
//        /// </summary>
//        /// <typeparam name="T">The type of entity.</typeparam>
//        /// <param name="dbSet">The <see cref="DbSet{TEntity}"/> to work with.</param>
//        /// <param name="data">The entity data to add or update.</param>
//        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T data) where T : class
//        {
//            var context = dbSet.GetContext();
//            var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name);

//            var t = typeof(T);
//            List<PropertyInfo> keyFields = new List<PropertyInfo>();

//            foreach (var propt in t.GetProperties())
//            {
//                var keyAttr = ids.Contains(propt.Name);
//                if (keyAttr)
//                {
//                    keyFields.Add(propt);
//                }
//            }
//            if (keyFields.Count <= 0)
//            {
//                throw new Exception($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
//            }
//            var entities = dbSet.AsNoTracking().ToList();
//            foreach (var keyField in keyFields)
//            {
//                var keyVal = keyField.GetValue(data);
//                entities = entities.Where(p => p.GetType().GetProperty(keyField.Name).GetValue(p).Equals(keyVal)).ToList();
//            }
//            var dbVal = entities.FirstOrDefault();
//            if (dbVal != null)
//            {
//                context.Entry(dbVal).CurrentValues.SetValues(data);
//                context.Entry(dbVal).State = EntityState.Modified;
//                return;
//            }
//            dbSet.Add(data);
//        }

//        /// <summary>
//        /// Adds or updates an entity in the <see cref="DbSet{TEntity}"/> based on a key expression.
//        /// </summary>
//        /// <typeparam name="T">The type of entity.</typeparam>
//        /// <param name="dbSet">The <see cref="DbSet{TEntity}"/> to work with.</param>
//        /// <param name="key">The expression to extract the key object.</param>
//        /// <param name="data">The entity data to add or update.</param>
//        public static void AddOrUpdate<T>(this DbSet<T> dbSet, Expression<Func<T, object>> key, T data) where T : class
//        {
//            var context = dbSet.GetContext();
//            var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name);
//            var t = typeof(T);
//            var keyObject = key.Compile()(data);
//            PropertyInfo[] keyFields = keyObject.GetType().GetProperties().Select(p => t.GetProperty(p.Name)).ToArray();
//            if (keyFields == null)
//            {
//                throw new Exception($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
//            }
//            var keyVals = keyFields.Select(p => p.GetValue(data));
//            var entities = dbSet.AsNoTracking().ToList();
//            int i = 0;
//            foreach (var keyVal in keyVals)
//            {
//                entities = entities.Where(p => p.GetType().GetProperty(keyFields[i].Name).GetValue(p).Equals(keyVal)).ToList();
//                i++;
//            }
//            if (entities.Any())
//            {
//                var dbVal = entities.FirstOrDefault();
//                var keyAttrs =
//                    data.GetType().GetProperties().Where(p => ids.Contains(p.Name)).ToList();
//                if (keyAttrs.Any())
//                {
//                    foreach (var keyAttr in keyAttrs)
//                    {
//                        keyAttr.SetValue(data,
//                            dbVal.GetType()
//                                .GetProperties()
//                                .FirstOrDefault(p => p.Name == keyAttr.Name)
//                                .GetValue(dbVal));
//                    }
//                    context.Entry(dbVal).CurrentValues.SetValues(data);
//                    context.Entry(dbVal).State = EntityState.Modified;
//                    return;
//                }
//            }
//            dbSet.Add(data);
//        }
//    }

//    /// <summary>
//    /// Provides a hacky way to retrieve the <see cref="DbContext"/> associated with a <see cref="DbSet{TEntity}"/>.
//    /// </summary>
//    public static class HackyDbSetGetContextTrick
//    {
//        /// <summary>
//        /// Gets the <see cref="DbContext"/> associated with a <see cref="DbSet{TEntity}"/>.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of entity in the <see cref="DbSet{TEntity}"/>.</typeparam>
//        /// <param name="dbSet">The <see cref="DbSet{TEntity}"/> instance.</param>
//        /// <returns>The <see cref="DbContext"/> associated with the <see cref="DbSet{TEntity}"/>.</returns>
//        public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
//            where TEntity : class
//        {
//            return (DbContext)dbSet
//                .GetType().GetTypeInfo()
//                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
//                .GetValue(dbSet);
//        }
//    }
//}