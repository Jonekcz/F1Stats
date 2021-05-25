using F1_Stats.JSON;
using F1_Stats.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace F1_Stats
{
    public class APIHelper : IDisposable
    {
        private const string baseUrl = "http://ergast.com/api/f1";
        private readonly int year;
        internal int Round { get; set; }
        private readonly F1Context _context;
        // e.g qualifying, results, pitstops
        private string apiTable; 
        private string json;

        internal APIHelper(F1Context context, int year, int round)
        {
            _context = context;
            this.year = year;
            this.Round = round;
        }

        internal void DownloadJSON()
        {
            // get data from ergast
            using WebClient webClient = new();
            try
            {
                json = webClient.DownloadString(new Uri(FormatUri()));
            }
            catch (Exception)
            {
            }
        }

        private string FormatUri()
        {
            return $"{baseUrl}/{year}/{Round}/{apiTable}.json";
        }

        public void SetQualifyingResults()
        {
            apiTable = "qualifying";
            DownloadJSON();
            if (json != null && json != "")
            {
                Qualifying.Rootobject rootobject = JsonConvert.DeserializeObject<Qualifying.Rootobject>(json);
                List<QualifyingResult> qualifyingResults = new();
                int qualifyingId = _context.Events.Include(i => i.SeasonIdNavigation).Where(i => i.SeasonIdNavigation.Year == year).OrderBy(i => i.DateTime).Skip(Round - 1).First().EventId;
                int length = rootobject.MRData.RaceTable.Races.Length;
                if (length > 0)
                {
                    foreach (var item in rootobject.MRData.RaceTable.Races[0].QualifyingResults)
                    {
                        QualifyingResult qualifyingResult = new();
                        qualifyingResult.QualifyingId = qualifyingId;
                        qualifyingResult.DriverId = _context.Drivers.First(i => i.Name == item.Driver.givenName && i.Lastname == item.Driver.familyName).DriverId;
                        qualifyingResult.Position = (byte?)int.Parse(item.position);
                        string time = item.Q1;

                        if (time == null || time.Equals(""))
                        {
                            qualifyingResult.Q1Time = null;
                        }
                        else
                        {

                            string[] SingleTimeArray = time.Split(':');

                            qualifyingResult.Q1Time = TimeSpan.Parse("00:" + time);
                        }
                        time = item.Q2;
                        if (time == null || time.Equals(""))
                        {
                            qualifyingResult.Q2Time = null;
                        }
                        else
                        {
                            qualifyingResult.Q2Time = TimeSpan.Parse("00:" + time);
                        }
                        time = item.Q3;
                        if (time == null || time.Equals(""))
                        {
                            qualifyingResult.Q3Time = null;
                        }
                        else
                        {
                            qualifyingResult.Q3Time = TimeSpan.Parse("00:" + time);
                        }
                        qualifyingResult.TeamId = _context.Teams.First(i => i.Name == item.Constructor.name).TeamId;
                        if (!_context.QualifyingResults.Contains(qualifyingResult))
                            qualifyingResults.Add(qualifyingResult);
                    }

                    // update db
                    try
                    {
                        _context.AddRange(qualifyingResults);
                        _context.SaveChanges();
                    }
                    catch(Exception e) { }
                    
                }
            }
        }

        internal void SetRaceResults()
        {
            apiTable = "results";
            DownloadJSON();
            if (json != null && json != "")
            {
                Results.Rootobject rootobject = JsonConvert.DeserializeObject<Results.Rootobject>(json);
                List<Result> results = new();
                int raceId = _context.Events.Include(i => i.SeasonIdNavigation).Where(i => i.SeasonIdNavigation.Year == year).OrderBy(i => i.DateTime).Skip(Round - 1).First().EventId;
                int length = rootobject.MRData.RaceTable.Races.Length;
                if (length > 0)
                {
                    foreach (var item in rootobject.MRData.RaceTable.Races[0].Results)
                    {
                        Result result = new();
                        result.RaceId = raceId;
                        result.DriverId = _context.Drivers.First(i => i.Name == item.Driver.givenName && i.Lastname == item.Driver.familyName).DriverId;
                        result.Position = (byte?)int.Parse(item.position);
                        result.Points = (byte?)int.Parse(item.points);

                        if (item.FastestLap == null || item.FastestLap.Time.time.Equals(""))
                        {
                            result.BestLapTime = null;
                        }
                        else
                        {
                            string time = item.FastestLap.Time.time;
                            // add hours
                            result.BestLapTime = TimeSpan.Parse("00:" + time);
                        }
                        if (item.Time == null)
                        {
                            result.Time = null;
                        }
                        else
                        {
                            result.Time = TimeSpan.FromMilliseconds(double.Parse(item.Time.millis));
                        }
                        result.TeamId = _context.Teams.First(i => i.Name == item.Constructor.name).TeamId;
                        if (!_context.Results.Contains(result))
                            results.Add(result);
                    }

                    // update db
                    try
                    {
                        _context.Results.AddRange(results);
                        _context.SaveChanges();
                    }
                    catch (Exception) { }
                }
            }
        }

        public void SetPitstops()
        {
            apiTable = "pitstops";
            DownloadJSON();
            if (json != null && json != "")
            {
                Pitstops.Rootobject rootobject = JsonConvert.DeserializeObject<Pitstops.Rootobject>(json);
                List<Pitstop> pitstops = new();
                int raceId = _context.Events.Include(i => i.SeasonIdNavigation).Where(i => i.SeasonIdNavigation.Year == year).OrderBy(i => i.DateTime).Skip(Round - 1).First().EventId;
                int length = rootobject.MRData.RaceTable.Races.Length;
                if (length > 0)
                {
                    foreach (var item in rootobject.MRData.RaceTable.Races[0].PitStops)
                    {
                        Pitstop pitstop = new();
                        pitstop.RaceId = raceId;
                        pitstop.DriverId = GetDriverId(item.driverId);
                        pitstop.Lap = (byte?)int.Parse(item.lap);

                        if (item.duration == null || item.duration.Equals(""))
                        {
                            pitstop.Duration = null;
                        }
                        else
                        {
                            // minutes
                            if (item.duration.Contains(":"))
                            {
                                pitstop.Duration = TimeSpan.Parse("00:" + item.duration);
                            }
                            // seconds
                            else
                                pitstop.Duration = TimeSpan.Parse("00:00:" + item.duration);
                        }
                        pitstops.Add(pitstop);
                    }
                }

                // update db
                try
                {
                    _context.Pitstops.AddRange(pitstops);
                    _context.SaveChanges();
                }
                catch (Exception) { }
            }
        }

        private int GetDriverId(string driverId)
        {
            int id = 0;
            // id contains a name and a last name
            if (driverId.Contains("_"))
            {
                string name = driverId.Split("_")[0];
                string lastname = driverId.Split("_")[1];
                foreach (var driver in _context.Drivers)
                {
                    if (String.Compare(driver.Name.ToLower(), name, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0 && String.Compare(driver.Lastname.ToLower(), lastname, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0)
                    {
                        id = driver.DriverId;
                    }
                }
            }
            // id contains a last name
            else
            {
                foreach(var driver in _context.Drivers)
                {
                    if(String.Compare(driver.Lastname.ToLower(), driverId, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0){
                        id = driver.DriverId;
                    }
                }
            }
            return id;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
