using F1_Stats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace F1_Stats.Controllers
{
    public class StatsController : Controller
    {
        private readonly F1Context _context;
        private readonly IStringLocalizer<StatsController> _stringLocalizer;
        public StatsController(F1Context context, IStringLocalizer<StatsController> stringLocalizer)
        {
            _context = context;
            _stringLocalizer = stringLocalizer;
        }

        //[Route("/Stats/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            ViewData["ErrorCode"] = code;
            return View("~/Views/Shared/Error.cshtml");
        }

        // GET: Stats
        public async Task<IActionResult> Index()
        {
            int year = DateTime.Now.Year;

            var countries = await (_context.Countries.ToListAsync());
            var teams = await (_context.Teams.ToListAsync());
            var resultTypes = await (_context.ResultTypes.ToListAsync());
            var drivers = await (from d in _context.Drivers join r in _context.Results on d.DriverId equals r.DriverId join w in _context.Events on r.RaceId equals w.EventId join s in _context.Seasons on w.SeasonId equals s.SeasonId where s.Year == year select d).Distinct().ToListAsync();

            // last race
            var lastRace = _context.Events.OrderByDescending(w => w.DateTime).First(w => w.DateTime < DateTime.Now);

            // last race circuit name
            lastRace.CircuitIdNavigation = _context.Circuits.First(t => t.CircuitId == lastRace.CircuitId);

            var results = await (from r in _context.Results join w in _context.Events on r.RaceId equals w.EventId join s in _context.Seasons on w.SeasonId equals s.SeasonId join t in _context.Circuits on w.CircuitId equals t.CircuitId where w.EventId == lastRace.EventId orderby r.Position.HasValue descending, r.Position ascending select r).ToListAsync();
            var nextRace = _context.Events.OrderBy(w => w.DateTime).First(w => w.DateTime > DateTime.Now);

            // get a track
            nextRace.CircuitIdNavigation = _context.Circuits.Single(t => t.CircuitId == nextRace.CircuitId);

            // get a country
            foreach (Models.Driver d in drivers)
            {
                d.CountryIdNavigation = countries.Single(c => c.CountryId == d.CountryId);
            }

            // get a driver's team
            foreach (Result r in results)
            {
                r.TeamIdNavigation = teams.Single(t => t.TeamId == r.TeamId);
            }

            // Add a result type
            foreach (Result r in results)
            {
                if(r.ResultTypeId != null)
                r.ResultTypeIdNavigation = resultTypes.Single(t => t.ResultTypeId == r.ResultTypeId);
            }
            ViewBag.Drivers = drivers;
            ViewBag.Results = results;
            ViewBag.NextRace = nextRace;
            ViewBag.LastRaceCircuit = lastRace.CircuitIdNavigation.Name;

            return View();
        }

        public async Task<IActionResult> Teams()
        {
            int year = DateTime.Now.Year;

            var teams = await (from t in _context.Teams join r in _context.Results on t.TeamId equals r.TeamId join w in _context.Events on r.RaceId equals w.EventId join s in _context.Seasons on w.SeasonId equals s.SeasonId where s.Year == year select t).Distinct().ToListAsync();

            // get a country
            foreach (Team t in teams)
            {
                t.CountryIdNavigation = _context.Countries.Single(c => c.CountryId == t.CountryId);
            }

            ViewBag.Teams = teams;
            return View();
        }

        public async Task<IActionResult> Standings()
        {
            int year = DateTime.Now.Year;

            var driverStandings = await _context.DriversStandings.FromSqlRaw("EXEC dbo.driver_standings @sezon = {0}", year).ToListAsync();
            var teamsStandings = await (_context.TeamsStandings.FromSqlRaw("EXEC dbo.constructor_standings @sezon = {0}", year).ToListAsync());

            ViewBag.DriverStandings = driverStandings;
            ViewBag.TeamsStandings = teamsStandings;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Update()
        {
            int year = DateTime.Now.Year;
            int round = 1;

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT TOP 1 rok,round FROM (SELECT Wydarzenie.id_terminarza,Sezon.rok,ROW_NUMBER() OVER(ORDER BY Wydarzenie.data_czas) \"round\",Wydarzenie.data_czas FROM Wydarzenie INNER JOIN Sezon ON Sezon.id_sezonu = Wydarzenie.id_sezonu WHERE  Sezon.rok = YEAR(GETDATE())) races INNER JOIN Wynik_kwalifikacji ON id_terminarza = Wynik_kwalifikacji.id_kwalifikacji WHERE data_czas < GETDATE() ORDER BY data_czas DESC;";
                _context.Database.OpenConnection();
                // last race whose results are available in db
                var result = await command.ExecuteReaderAsync();
                while (result.Read())
                {
                    year = result.GetInt16(0);
                    // long to int
                    round = (int)result.GetInt64(1);
                }
            }

            // count of rounds in current year
            int roundsCount = (await _context.Events.Include(i => i.SeasonIdNavigation).Where(i => i.SeasonIdNavigation.Year == year).ToListAsync()).Count;

            if (++round <= roundsCount)
            {
                
                string url = $"http://ergast.com/api/f1/{year}/{round}/qualifying.json";
                APIHelper api = new(_context,year,round);
                api.SetQualifyingResults();
                api.SetRaceResults();
            }
            return RedirectToAction("Index");
        }

        // GET: Stats/Create
        [Authorize]
        [HttpGet]
        [Route("/Stats/Create/{page:int}")]
        [Route("/Stats/Create")]
        public IActionResult Create(int page = 1)
        {
            const int pageSize = 50;
            TempData["page"] = page;
            TempData.Keep("page");
            string currentTable = TableNames("");
            int count = _context.Set(currentTable).Count();
            int maxPage = ((count / pageSize) - (count % pageSize == 0 ? 1 : 0)) + 1;
            page = page > maxPage ? maxPage : page;

            var dynamicTable = GetDynamicTable(currentTable, page, pageSize);

            ViewBag.MaxPage = maxPage;
            ViewBag.Page = page;
            if (TempData.ContainsKey("Message"))
                ViewData["Message"] = TempData["Message"];
            return View(dynamicTable);
        }

        private IQueryable<object> GetDynamicTable(string currentTable, int page, int pageSize)
        {
            // class name with namespace
            string className = (string)TempData["currentTableClass"];
            TempData["currentTableClass"] = className;
            // db table class type
            Type objectType = Type.GetType(className);
            var properties = objectType.GetProperties().Where(i => i.Name.Contains("Navigation"));

            var dynamicTable = _context.Set(currentTable).Skip((page - 1) * pageSize).Take(pageSize);
            /*//List<string> navigationEntities = new();
            foreach (var property in properties)
            {
                dynamicTable = dynamicTable.Include(property.Name);
                //navigationEntities.Add(property.PropertyType.Name);
            }
            List<SelectList> selectLists = new();
            /*foreach(string item in navigationEntities)
            {
                //var key = _context.Model.GetEntityTypes().Where(i=>i.Name == item).First().FindPrimaryKey();
                var value = _context.Model.GetEntityTypes().Where(i => i.ClrType.Name == item).First().FindPrimaryKey().Properties[0].Name;
                
                selectLists.Add(new SelectList(_context.Set(item),,);
            }*/

            return dynamicTable;
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChooseTable(string tableName)
        {
            TempData["TableName"] = tableName;
            const int pageSize = 50;
            int page = (int)TempData["page"];
            // class name
            string correctTableName = TableNames(tableName);
            int count = _context.Set(correctTableName).Count();
            int maxPage = (count / pageSize) - (count % pageSize == 0 ? 1 : 0) + 1;

            // db records
            var dynamicTable = GetDynamicTable(correctTableName, page, pageSize);

            ViewBag.MaxPage = maxPage;
            ViewBag.Page = page;
            return Json(dynamicTable);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            var model = ModelState;
            string currentTableClass = (string)TempData["currentTableClass"];
            // db table class type
            Type objectType = Type.GetType(currentTableClass);
            TempData["currentTableClass"] = currentTableClass;
            // list of objects of a 'tableName' class
            Type listType = typeof(List<>).MakeGenericType(objectType);
            // list of records
            var list = Activator.CreateInstance(listType);
            // single record
            var singleRecord = Activator.CreateInstance(objectType);

            // entity properties
            PropertyInfo[] properties = singleRecord.GetType().GetProperties();
            // properties names
            String[] propertyNames = new string[properties.Length];
            for (var i = 0; i < propertyNames.Length; i++)
            {
                propertyNames[i] = properties[i].Name;
            }
            int recordsCount = (from key in Request.Form.Keys let value = Regex.Match(key, @"\d+").Value select Convert.ToInt32(value == string.Empty ? "0" : Regex.Match(key, @"\d+").Value) + 1).Concat(new[] { 0 }).Max();
            // row number
            int counter = 0;
            while (counter < recordsCount)
            {
                var record = Activator.CreateInstance(objectType);
                foreach (string propertyName in propertyNames)
                {
                    if (Request.Form.ContainsKey($"{propertyName}[{counter}]"))
                    {
                        record.GetType().GetProperty(propertyName).SetValue(record, CastItem(Request.Form[$"{propertyName}[{counter}]"].ToString(), record, propertyName));
                    }
                }
                list.GetType().GetMethod("Add").Invoke(list, new object[] { record });
                counter++;
            }

            // update database
            _context.UpdateRange(((IList)list)[0]);
            await _context.SaveChangesAsync();
            TempData["Message"] = _stringLocalizer["Message"].Value;
            return RedirectToAction("Create");
        }

        private static object CastItem(string item, object record, string currentProperty)
        {
            Type type = record.GetType().GetProperty(currentProperty).PropertyType;
            if (type == typeof(string))
                return item;
            if (type == typeof(int))
                return int.Parse(item);
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                if (item.Contains(",") && !item.Contains("."))
                {
                    item = item.Replace(",", ".");
                }
                return decimal.Parse(item, new CultureInfo("en"));
            }
            if (type == typeof(float))
            {
                if (item.Contains(",") && !item.Contains("."))
                {
                    item = item.Replace(",", ".");
                }
                return float.Parse(item, new CultureInfo("en"));
            }
            if (type == typeof(DateTime))
                return DateTime.Parse(item);
            return null;
        }

        // get db table names, pass them to the select list, get the current selected table
        private string TableNames(string currentTable)
        {
            if (TempData.ContainsKey("TableName"))
            {
                currentTable = TempData["TableName"].ToString();
                TempData["TableName"] = currentTable;
            }
            // current language
            string lang = CultureInfo.CurrentCulture.Name;
            // entities with a key
            var entities = _context.Model.GetEntityTypes().Where(i => i.FindPrimaryKey() != null);
            List<string> tableNames = new();
            foreach (var entity in entities)
            {
                if (lang == "pl")
                    tableNames.Add(entity.GetAnnotation("Relational:TableName").Value.ToString());
                if (lang == "en")
                    tableNames.Add(entity.ClrType.Name);
            }
            if (currentTable.Equals(""))
                currentTable = tableNames[0];

            TempData["TableName"] = currentTable;
            ViewData["Tables"] = new SelectList(tableNames, TempData.ContainsKey("TableName") ? TempData["TableName"] : currentTable);

            var entityType = entities.First();
            foreach (var entity in entities)
            {
                if (lang == "pl")
                    if (entity.GetAnnotation("Relational:TableName").Value.ToString().Equals(currentTable))
                    {
                        entityType = entity;
                        currentTable = entity.ClrType.Name;
                        break;
                    }
                if (lang == "en")
                    if (entity.ClrType.Name.Equals(currentTable))
                    {
                        entityType = entity;
                        break;
                    }
            }

            if(lang == "en")
            {
                currentTable = entityType.GetAnnotation("Relational:TableName") != null ? entityType.ClrType.Name : currentTable;
            }

            // with namespace
            TempData["currentTableClass"] = entityType.ClrType.FullName;
            // table name e.g Driver
            return currentTable;
        }

    }
}
