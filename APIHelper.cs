﻿using F1_Stats.JSON;
using F1_Stats.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly int round;
        private readonly F1Context _context;
        // e.g qualifying, results, pitstops
        private string apiTable; 
        private string json;

        internal APIHelper(F1Context context, int year, int round)
        {
            _context = context;
            this.year = year;
            this.round = round;
        }

        internal void DownloadJSON()
        {
            // get data from ergast
            using WebClient webClient = new();
            json = webClient.DownloadString(new Uri(FormatUri()));
        }

        private string FormatUri()
        {
            return $"{baseUrl}/{year}/{round}/{apiTable}.json";
        }

        internal void SetQualifyingResults()
        {
            apiTable = "qualifying";
            DownloadJSON();
            Qualifying.Rootobject rootobject = JsonConvert.DeserializeObject<Qualifying.Rootobject>(json);
            List<QualifyingResult> qualifyingResults = new();
            int qualifyingId = _context.Events.Include(i => i.SeasonIdNavigation).Where(i => i.SeasonIdNavigation.Year == year).OrderBy(i => i.DateTime).Skip(round - 1).First().EventId;
            if (rootobject.MRData.RaceTable.Races != null)
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
                        qualifyingResult.Q1Time = new TimeSpan(0, 0, int.Parse(time.Substring(0, time.IndexOf(":"))), int.Parse(time.Substring(time.IndexOf(":") + 1, 2)), int.Parse(time.Substring(time.IndexOf(".") + 1)));
                    }
                    time = item.Q2;
                    if (time == null || time.Equals(""))
                    {
                        qualifyingResult.Q2Time = null;
                    }
                    else
                    {
                        qualifyingResult.Q2Time = new TimeSpan(0, 0, int.Parse(time.Substring(0, time.IndexOf(":"))), int.Parse(time.Substring(time.IndexOf(":") + 1, 2)), int.Parse(time.Substring(time.IndexOf(".") + 1)));
                    }
                    time = item.Q3;
                    if (time == null || time.Equals(""))
                    {
                        qualifyingResult.Q3Time = null;
                    }
                    else
                    {
                        qualifyingResult.Q3Time = new TimeSpan(0, 0, int.Parse(time.Substring(0, time.IndexOf(":"))), int.Parse(time.Substring(time.IndexOf(":") + 1, 2)), int.Parse(time.Substring(time.IndexOf(".") + 1)));
                    }
                    qualifyingResult.TeamId = _context.Teams.First(i => i.Name == item.Constructor.name).TeamId;
                    qualifyingResults.Add(qualifyingResult);
                }

                // update db
                _context.AddRange(qualifyingResults);
                _context.SaveChanges();
            }
        }

        internal void SetRaceResults()
        {
            apiTable = "results";
            DownloadJSON();
            Results.Rootobject rootobject = JsonConvert.DeserializeObject<Results.Rootobject>(json);
            List<Result> results = new();
            int raceId = _context.Events.Include(i => i.SeasonIdNavigation).Where(i => i.SeasonIdNavigation.Year == year).OrderBy(i => i.DateTime).Skip(round - 1).First().EventId;
            if (rootobject.MRData.RaceTable.Races != null)
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
                        result.BestLapTime = new TimeSpan(0, 0, int.Parse(time.Substring(0, time.IndexOf(":"))), int.Parse(time.Substring(time.IndexOf(":") + 1, 2)), int.Parse(time[(time.IndexOf(".") + 1)..]));
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
                    results.Add(result);
                }

                // update db
                _context.Results.AddRange(results);
                _context.SaveChanges();
            }
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