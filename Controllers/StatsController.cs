using F1_Stats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            foreach (Driver d in drivers)
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
        public IActionResult Create()
        {
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa");
            return View();
        }

        // POST: Stats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "IdKierowcy,Imie,Nazwisko,IdKraju,DataUrodzenia")] Driver kierowca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kierowca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.CountryId);
            return View(kierowca);
        }

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
        public async Task<IActionResult> Edit(int id, [Bind("IdKierowcy,Imie,Nazwisko,IdKraju,DataUrodzenia")] Driver kierowca)
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
