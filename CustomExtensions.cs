using F1_Stats.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.EntityFrameworkCore
{

    public static class CustomExtensions
    {
        static Dictionary<string, Type> TableTypeDictionary = new Dictionary<string, Type>()
    {
         //store the tables name and type.
          { "Circuit", typeof(Circuit) },
          { "Country", typeof(Country) },
          { "Continent", typeof(Continent) },
           { "Driver", typeof(Driver) },
          { "DriverStandings", typeof(DriverStandings) },
          { "Event", typeof(Event) },
          { "LapTime", typeof(LapTime) },
          { "Pitstop", typeof(Pitstop) },
          { "QualifyingResult", typeof(QualifyingResult) },
          { "Result", typeof(Result) },
          { "ResultType", typeof(ResultType) },
          { "Season", typeof(Season) },
          { "Team", typeof(Team) },
          { "TeamStandings", typeof(TeamStandings) },
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
