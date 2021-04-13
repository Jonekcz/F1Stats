using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using F1_Stats.Models;
using Microsoft.AspNetCore.Authorization;

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

            var countries = _context.Krajs;
            var teams = _context.Zespols;
            var resultTypes = _context.RodzajWynikus;
            var drivers = await(from d in _context.Kierowcas join r in _context.WynikWyscigus on d.IdKierowcy equals r.IdKierowcy join w in _context.Wydarzenies on r.IdWyscigu equals w.IdTerminarza join s in _context.Sezons on w.IdSezonu equals s.IdSezonu where s.Rok == year select d).ToListAsync();
            //drivers = (List<Kierowca>)drivers.Distinct();

            // last race
            var lastRace = _context.Wydarzenies.OrderBy(w=>w.DataCzas).Reverse().First(w=>w.DataCzas < DateTime.Now);

            // last race circuit name
            lastRace.IdToruNavigation = _context.Tors.First(t => t.IdToru == lastRace.IdToru);

            var results = await(from r in  _context.WynikWyscigus join w in _context.Wydarzenies on r.IdWyscigu equals w.IdTerminarza join s in _context.Sezons on w.IdSezonu equals s.IdSezonu join t in _context.Tors on w.IdToru equals t.IdToru where w.IdTerminarza == lastRace.IdTerminarza orderby r.Pozycja.HasValue descending,r.Pozycja ascending select r).ToListAsync();
            var nextRace = _context.Wydarzenies.OrderBy(w=>w.DataCzas).First(w => w.DataCzas > DateTime.Now);

            // get a track
            nextRace.IdToruNavigation = _context.Tors.Single(t => t.IdToru == nextRace.IdToru);
            
            // get a country
            foreach (Kierowca d in drivers)
            {
                d.IdKrajuNavigation = countries.Single(c => c.IdKraju == d.IdKraju);
            }

            // get a driver's team
            foreach (WynikWyscigu r in results)
            {
                r.IdZespoluNavigation = teams.Single(t => t.IdZespolu == r.IdZespolu);
            }

            // Add a result type
            foreach (WynikWyscigu r in results)
            {
                r.IdRodzajuWynikuNavigation = resultTypes.Single(t => t.IdRodzajuWyniku == r.IdRodzajuWyniku); 
            }

            ViewBag.Drivers = drivers;
            ViewBag.Results = results;
            ViewBag.NextRace = nextRace;
            ViewBag.LastRaceCircuit = lastRace.IdToruNavigation.Nazwa;

            return View();
        }

        public async Task<IActionResult> Teams()
        {
            int year = DateTime.Now.Year;

            var teams = from t in _context.Zespols join r in _context.WynikWyscigus on t.IdZespolu equals r.IdZespolu join w in _context.Wydarzenies on r.IdWyscigu equals w.IdTerminarza join s in _context.Sezons on w.IdSezonu equals s.IdSezonu where s.Rok == year select t;

            teams = teams.Distinct();

            // get a country
            foreach(Zespol t in teams)
            {
                t.IdKrajuNavigation = _context.Krajs.Single(c => c.IdKraju == t.IdKraju);
            }

            ViewBag.Teams = teams;
            return View();
        }

        public async Task<IActionResult> Standings(){
            int year = DateTime.Now.Year;

            var driverStandings = _context.DriversStandings.FromSqlRaw("EXEC dbo.driver_standings @sezon = {0}",year);
            var teamsStandings = _context.TeamsStandings.FromSqlRaw("EXEC dbo.constructor_standings @sezon = {0}", year);

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
                .Include(k => k.IdKrajuNavigation)
                .FirstOrDefaultAsync(m => m.IdKierowcy == id);
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
        public async Task<IActionResult> Create([Bind(include:"IdKierowcy,Imie,Nazwisko,IdKraju,DataUrodzenia")] Kierowca kierowca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kierowca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.IdKraju);
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
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.IdKraju);
            return View(kierowca);
        }

        // POST: Stats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKierowcy,Imie,Nazwisko,IdKraju,DataUrodzenia")] Kierowca kierowca)
        {
            if (id != kierowca.IdKierowcy)
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
                    if (!KierowcaExists(kierowca.IdKierowcy))
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
            ViewData["IdKraju"] = new SelectList(_context.Krajs, "IdKraju", "Nazwa", kierowca.IdKraju);
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
                .Include(k => k.IdKrajuNavigation)
                .FirstOrDefaultAsync(m => m.IdKierowcy == id);
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
            return _context.Kierowcas.Any(e => e.IdKierowcy == id);
        }
    }
}
