namespace F1_Stats.JSON
{
    public class Qualifying
    {
        public class Rootobject
        {
            public Mrdata MRData { get; set; }
        }

        public class Mrdata
        {
            public string xmlns { get; set; }
            public string series { get; set; }
            public string url { get; set; }
            public string limit { get; set; }
            public string offset { get; set; }
            public string total { get; set; }
            public Racetable RaceTable { get; set; }
        }

        public class Racetable
        {
            public string season { get; set; }
            public string round { get; set; }
            public Race[] Races { get; set; }
        }

        public class Race
        {
            public string season { get; set; }
            public string round { get; set; }
            public string url { get; set; }
            public string raceName { get; set; }
            public Circuit Circuit { get; set; }
            public string date { get; set; }
            public string time { get; set; }
            public Qualifyingresult[] QualifyingResults { get; set; }
        }

        public class Circuit
        {
            public string circuitId { get; set; }
            public string url { get; set; }
            public string circuitName { get; set; }
            public Location Location { get; set; }
        }

        public class Location
        {
            public string lat { get; set; }
            public string _long { get; set; }
            public string locality { get; set; }
            public string country { get; set; }
        }

        public class Qualifyingresult
        {
            public string number { get; set; }
            public string position { get; set; }
            public Driver Driver { get; set; }
            public Constructor Constructor { get; set; }
            public string Q1 { get; set; }
            public string Q2 { get; set; }
            public string Q3 { get; set; }
        }

        public class Driver
        {
            public string driverId { get; set; }
            public string permanentNumber { get; set; }
            public string code { get; set; }
            public string url { get; set; }
            public string givenName { get; set; }
            public string familyName { get; set; }
            public string dateOfBirth { get; set; }
            public string nationality { get; set; }
        }

        public class Constructor
        {
            public string constructorId { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string nationality { get; set; }
        }

    }
}
