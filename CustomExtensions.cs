using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using F1_Stats.Models;

namespace Microsoft.EntityFrameworkCore
{
    /*public static partial class CustomExtensions
    {
        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(context.Model.FindEntityType(entityName).ClrType);

        public static IQueryable Query(this DbContext context, Type entityType) =>
            (IQueryable)((IDbSetCache)context).GetOrAddSet(context.GetDependencies().SetSource, entityType);
    }*/

    public static class CustomExtensions
    {
        static Dictionary<string, Type> TableTypeDictionary = new Dictionary<string, Type>()
    {
          { "Driver", typeof(Driver) },//store the tables name and type.
          { "Circuit", typeof(Circuit) },
          { "Country", typeof(Country) }
          //...
    };

        public static IQueryable<Object> Set(this DbContext _context, Type t)
        {
            return (IQueryable<Object>)_context.GetType().GetMethod("Set", new System.Type[0]).MakeGenericMethod(t).Invoke(_context, null);
        }


        public static IQueryable<Object> Set(this DbContext _context, String table)
        {
            Type TableType = _context.GetType().Assembly.GetExportedTypes().FirstOrDefault(t => t.Name == table);
            IQueryable<Object> ObjectContext = _context.Set(TableTypeDictionary[table]);
            return ObjectContext;
        }
    }
}
