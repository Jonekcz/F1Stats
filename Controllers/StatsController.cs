﻿using F1_Stats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static F1_Stats.JSON.Qualifying;

namespace F1_Stats.Controllers
{
    public class StatsController : Controller
    {
        private readonly F1Context _context;

        public StatsController(F1Context context)
        {
            _context = context;
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

            var countries = await (_context.Krajs.ToListAsync());
            var teams = await (_context.Zespols.ToListAsync());
            var resultTypes = await (_context.RodzajWynikus.ToListAsync());
            var drivers = await (from d in _context.Kierowcas join r in _context.WynikWyscigus on d.DriverId equals r.DriverId join w in _context.Wydarzenies on r.RaceId equals w.EventId join s in _context.Sezons on w.SeasonId equals s.SeasonId where s.Year == year select d).ToListAsync();

            // last race
            var lastRace = _context.Wydarzenies.OrderByDescending(w => w.DateTime).First(w => w.DateTime < DateTime.Now);

            // last race circuit name
            lastRace.CircuitIdNavigation = _context.Tors.First(t => t.CircuitId == lastRace.CircuitId);

            var results = await (from r in _context.WynikWyscigus join w in _context.Wydarzenies on r.RaceId equals w.EventId join s in _context.Sezons on w.SeasonId equals s.SeasonId join t in _context.Tors on w.CircuitId equals t.CircuitId where w.EventId == lastRace.EventId orderby r.Position.HasValue descending, r.Position ascending select r).ToListAsync();
            var nextRace = _context.Wydarzenies.OrderBy(w => w.DateTime).First(w => w.DateTime > DateTime.Now);

            // get a track
            nextRace.CircuitIdNavigation = _context.Tors.Single(t => t.CircuitId == nextRace.CircuitId);

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

            var teams = await (from t in _context.Zespols join r in _context.WynikWyscigus on t.TeamId equals r.TeamId join w in _context.Wydarzenies on r.RaceId equals w.EventId join s in _context.Sezons on w.SeasonId equals s.SeasonId where s.Year == year select t).Distinct().ToListAsync();

            // get a country
            foreach (Team t in teams)
            {
                t.CountryIdNavigation = _context.Krajs.Single(c => c.CountryId == t.CountryId);
            }

            ViewBag.Teams = teams;
            return View();
        }

        public async Task<IActionResult> Standings()
        {
            int year = DateTime.Now.Year;

            var driverStandings = await (_context.DriversStandings.FromSqlRaw("EXEC dbo.driver_standings @sezon = {0}", year).ToListAsync());
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
                using var result = await command.ExecuteReaderAsync();
                while (result.Read())
                {
                    year = result.GetInt16(0);
                    // long to int
                    round = (int)result.GetInt64(1);
                }
            }

            // get data from ergast
            // qualifying
            string url = $"http://ergast.com/api/f1/{year}/{round + 1}/qualifying.json";
            WebClient webClient = new WebClient();
            // json
            string data = webClient.DownloadString(new Uri(url));
            Rootobject rootobject = JsonConvert.DeserializeObject<Rootobject>(data);
            return RedirectToAction("Index");
        }

        // GET: Stats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kierowca = await _context.Kierowcas
                .Include(k => k.CountryIdNavigation)
                .FirstOrDefaultAsync(m => m.DriverId == id);
            if (kierowca == null)
            {
                return NotFound();
            }

            return View(kierowca);
        }

        // GET: Stats/Create
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            // current language
            string lang = CultureInfo.CurrentCulture.Name;
            var entities = _context.Model.GetEntityTypes();
            List<string> tableNames = new List<string>();
            foreach (var entity in entities)
            {
                if (lang == "pl")
                    tableNames.Add(entity.GetAnnotation("Relational:TableName").Value.ToString());
                if (lang == "en")
                    tableNames.Add(entity.ClrType.Name);
            }
            //ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa");
            string tableName = tableNames[0];
            ViewData["Tables"] = new SelectList(tableNames);
            var entityType = entities.First();
            foreach (var entity in entities)
            {
                if (lang == "pl")
                    if (entity.GetAnnotation("Relational:TableName").Value.ToString().Equals(tableName))
                    {
                        entityType = entity;
                        tableName = entity.ClrType.Name;
                    }
                if (lang == "en")
                    if (entity.ClrType.Name.Equals(tableName))
                    {
                        entityType = entity;
                    }
            }
            var dynamicTable = _context.Set(tableName);
            ViewBag.TableName = entityType.ClrType.FullName;
            return View(dynamicTable);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChooseTable(string tableName)
        {
            string tableClassName = tableName;
            string lang = CultureInfo.CurrentCulture.Name;
            var entities = _context.Model.GetEntityTypes();
            var entityType = entities.First();
            foreach (var entity in entities)
            {
                if (lang == "pl")
                    if (entity.GetAnnotation("Relational:TableName").Value.ToString().Equals(tableName))
                    {
                        entityType = entity;
                        tableClassName = entity.ClrType.Name;
                    }
                if (lang == "en")
                    if (entity.ClrType.Name.Equals(tableName))
                    {
                        entityType = entity;

                    }
            }

            // db records
            var dynamicTable = _context.Set(tableClassName);

            // select list
            List<string> tableNames = new();
            foreach (var entity in entities)
            {
                if (lang == "pl")
                    tableNames.Add(entity.GetAnnotation("Relational:TableName").Value.ToString());
                if (lang == "en")
                    tableNames.Add(entity.ClrType.Name);
            }
            ViewData["Tables"] = new SelectList(tableNames, tableName);
            ViewBag.TableName = entityType.ClrType.FullName;
            return View("~/Views/Stats/Create.cshtml", dynamicTable);
            //return Json(dynamicTable);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(string tableName)
        {
            // db table class type
            Type objectType = Type.GetType(tableName);
            // list of objects of a 'tableName' class
            Type listType = typeof(List<>).MakeGenericType( objectType);
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
            ViewBag.Message = "Zaaktualizowano pomyślnie";
            return RedirectToAction("Create");
        }

        private object CastItem(string item, object record, string currentProperty)
        {
            Type type = record.GetType().GetProperty(currentProperty).PropertyType;
            if (type == typeof(string))
                return item;
            if (type == typeof(int))
                return int.Parse(item);
            if (type == typeof(decimal) || type == typeof(decimal?))
                return decimal.Parse(item);
            if (type == typeof(float))
                return float.Parse(item);
            if (type == typeof(DateTime))
                return DateTime.Parse(item);
            return null;
        }

        // POST: Stats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "IdKierowcy,Imie,Nazwisko,IdKraju,DataUrodzenia")] Models.Driver kierowca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kierowca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.CountryId);
            return View(kierowca);
        }*/

        // GET: Stats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kierowca = await _context.Kierowcas.FindAsync(id);
            if (kierowca == null)
            {
                return NotFound();
            }
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.CountryId);
            return View(kierowca);
        }

        // POST: Stats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKierowcy,Imie,Nazwisko,IdKraju,DataUrodzenia")] Models.Driver kierowca)
        {
            if (id != kierowca.DriverId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kierowca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KierowcaExists(kierowca.DriverId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.CountryId);
            return View(kierowca);
        }

        // GET: Stats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kierowca = await _context.Kierowcas
                .Include(k => k.CountryIdNavigation)
                .FirstOrDefaultAsync(m => m.DriverId == id);
            if (kierowca == null)
            {
                return NotFound();
            }

            return View(kierowca);
        }

        // POST: Stats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kierowca = await _context.Kierowcas.FindAsync(id);
            _context.Kierowcas.Remove(kierowca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KierowcaExists(int id)
        {
            return _context.Kierowcas.Any(e => e.DriverId == id);
        }
    }
}
