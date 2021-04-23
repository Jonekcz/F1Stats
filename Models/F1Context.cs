using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

#nullable disable

namespace F1_Stats.Models
{
    public partial class F1Context : DbContext
    {
        public F1Context()
        {
        }

        public F1Context(DbContextOptions<F1Context> options)
            : base(options)
        {
        }

        // driver standings procedure
        public virtual DbSet<DriverStandings> DriversStandings { get; set; }
        // constructor standings procedure
        public virtual DbSet<TeamStandings> TeamsStandings { get; set; }

        public virtual DbSet<LapTime> CzasOkrazenia { get; set; }
        public virtual DbSet<Driver> Kierowcas { get; set; }
        public virtual DbSet<Continent> Kontynents { get; set; }
        public virtual DbSet<Country> Krajs { get; set; }
        public virtual DbSet<City> Miastos { get; set; }
        public virtual DbSet<Pitstop> Pitstops { get; set; }
        public virtual DbSet<ResultType> RodzajWynikus { get; set; }
        public virtual DbSet<Season> Sezons { get; set; }
        public virtual DbSet<Circuit> Tors { get; set; }
        public virtual DbSet<Event> Wydarzenies { get; set; }
        public virtual DbSet<QualifyingResult> WynikKwalifikacjis { get; set; }
        public virtual DbSet<Result> WynikWyscigus { get; set; }
        public virtual DbSet<Team> Zespols { get; set; }

        // debugging
        public static readonly Microsoft.Extensions.Logging.LoggerFactory _myLoggerFactory =
    new LoggerFactory(new[] {
        new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
    });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("F1Context"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");

            modelBuilder.Entity<LapTime>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.DriverId, e.Lap });

                entity.ToTable("Czas_okrazenia");

                entity.Property(e => e.RaceId).HasColumnName("id_wyscigu");

                entity.Property(e => e.DriverId).HasColumnName("id_kierowcy");

                entity.Property(e => e.Lap).HasColumnName("okrazenie");

                entity.Property(e => e.Time).HasColumnName("czas");

                entity.HasOne(d => d.DriverIdNavigation)
                    .WithMany(p => p.LapTimes)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Czas_okrazenia_Kierowca");

                entity.HasOne(d => d.RaceIdNavigation)
                    .WithMany(p => p.LapTimes)
                    .HasForeignKey(d => d.RaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Czas_okrazenia_Wydarzenie");
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.DriverId)
                    .HasName("PK__Kierowca__EE994F76AAEC5DCE");

                entity.ToTable("Kierowca");

                entity.HasIndex(e => e.CountryId, "id_kraju_idx");

                entity.Property(e => e.DriverId).HasColumnName("id_kierowcy");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("data_urodzenia");

                entity.Property(e => e.CountryId).HasColumnName("id_kraju");

                entity.Property(e => e.Name)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("imie");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("nazwisko");

                entity.HasOne(d => d.CountryIdNavigation)
                    .WithMany(p => p.Drivers)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("id_kraju");
            });

            modelBuilder.Entity<Continent>(entity =>
            {
                entity.HasKey(e => e.IdKontynentu)
                    .HasName("PK__Kontynen__786308534077C461");

                entity.ToTable("Kontynent");

                entity.Property(e => e.IdKontynentu).HasColumnName("id_kontynentu");

                entity.Property(e => e.Name)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CountryId)
                    .HasName("PK__Kraj__E2DD85FF74F854E0");

                entity.ToTable("Kraj");

                entity.HasIndex(e => e.ContinentId, "id__idx");

                entity.Property(e => e.CountryId).HasColumnName("id_kraju");

                entity.Property(e => e.ContinentId).HasColumnName("id_kontynentu");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.HasOne(d => d.ContinentIdNavigation)
                    .WithMany(p => p.Countries)
                    .HasForeignKey(d => d.ContinentId)
                    .HasConstraintName("id_kontynentu");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.ToTable("Miasto");

                entity.Property(e => e.CityId).HasColumnName("id_miasta");

                entity.Property(e => e.CountryId).HasColumnName("id_kraju");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.HasOne(d => d.CountryIdNavigation)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Miasto_Kraj");
            });

            modelBuilder.Entity<Pitstop>(entity =>
            {
                entity.HasKey(e => e.PitstopId);

                entity.ToTable("Pitstop");

                entity.Property(e => e.PitstopId).HasColumnName("id_pitstopu");

                entity.Property(e => e.Duration)
                    .HasColumnType("time(3)")
                    .HasColumnName("Czas_trwania");

                entity.Property(e => e.DriverId).HasColumnName("id_kierowcy");

                entity.Property(e => e.RaceId).HasColumnName("id_wyscigu");

                entity.HasOne(d => d.DriverIdNavigation)
                    .WithMany(p => p.Pitstops)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pitstop_Kierowca");

                entity.HasOne(d => d.RaceIdNavigation)
                    .WithMany(p => p.Pitstops)
                    .HasForeignKey(d => d.RaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pitstop_Wydarzenie");
            });

            modelBuilder.Entity<ResultType>(entity =>
            {
                entity.HasKey(e => e.ResultTypeId);

                entity.ToTable("Rodzaj_wyniku");

                entity.Property(e => e.ResultTypeId).HasColumnName("id_rodzaju_wyniku");

                entity.Property(e => e.Name)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.SeasonId);

                entity.ToTable("Sezon");

                entity.Property(e => e.SeasonId).HasColumnName("id_sezonu");

                entity.Property(e => e.Year).HasColumnName("rok");
            });

            modelBuilder.Entity<Circuit>(entity =>
            {
                entity.HasKey(e => e.CircuitId);

                entity.ToTable("Tor");

                entity.Property(e => e.CircuitId).HasColumnName("id_toru");

                entity.Property(e => e.Lng)
                    .HasColumnType("decimal(9, 6)")
                    .HasColumnName("dlugosc_geo");

                entity.Property(e => e.CityId).HasColumnName("id_miasta");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.Property(e => e.Lat)
                    .HasColumnType("decimal(8, 6)")
                    .HasColumnName("szerokosc_geo");

                entity.HasOne(d => d.CityIdNavigation)
                    .WithMany(p => p.Circuits)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tor_Miasto");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.ToTable("Wydarzenie");

                entity.Property(e => e.EventId).HasColumnName("id_terminarza");

                entity.Property(e => e.RedFlag).HasColumnName("czerwona_flaga");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("data_czas");

                entity.Property(e => e.SeasonId).HasColumnName("id_sezonu");

                entity.Property(e => e.CircuitId).HasColumnName("id_toru");

                entity.Property(e => e.Weather)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("pogoda");

                entity.HasOne(d => d.SeasonIdNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.SeasonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wydarzenie_Sezon");

                entity.HasOne(d => d.CircuitIdNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.CircuitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wydarzenie_Tor");
            });

            modelBuilder.Entity<QualifyingResult>(entity =>
            {
                entity.HasKey(e => new { e.QualifyingId, e.DriverId });

                entity.ToTable("Wynik_kwalifikacji");

                entity.Property(e => e.QualifyingId).HasColumnName("id_kwalifikacji");

                entity.Property(e => e.DriverId).HasColumnName("id_kierowcy");

                entity.Property(e => e.Q1Time)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas_q1");

                entity.Property(e => e.Q2Time)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas_q2");

                entity.Property(e => e.Q3Time)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas_q3");

                entity.Property(e => e.TeamId).HasColumnName("id_zespolu");

                entity.Property(e => e.Position).HasColumnName("pozycja");

                entity.HasOne(d => d.DriverIdNavigation)
                    .WithMany(p => p.QualifyingResults)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_kwalifikacji_Kierowca");

                entity.HasOne(d => d.QualifyingIdNavigation)
                    .WithMany(p => p.QualifyingResults)
                    .HasForeignKey(d => d.QualifyingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_kwalifikacji_Wydarzenie");

                entity.HasOne(d => d.TeamIdNavigation)
                    .WithMany(p => p.QualifyingResults)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_Wynik_kwalifikacji_Zespol");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.DriverId });

                entity.ToTable("Wynik_wyscigu");

                entity.Property(e => e.RaceId).HasColumnName("id_wyscigu");

                entity.Property(e => e.DriverId).HasColumnName("id_kierowcy");

                entity.Property(e => e.Time)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas");

                entity.Property(e => e.ResultTypeId).HasColumnName("id_rodzaju_wyniku");

                entity.Property(e => e.TeamId).HasColumnName("id_zespolu");

                entity.Property(e => e.BestLapTime)
                    .HasColumnType("time(3)")
                    .HasColumnName("najlepszy_czas_okrazenia");

                entity.Property(e => e.Position).HasColumnName("pozycja");

                entity.Property(e => e.Points).HasColumnName("punkty");

                entity.HasOne(d => d.DriverIdNavigation)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_wyscigu_Kierowca");

                entity.HasOne(d => d.ResultTypeIdNavigation)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.ResultTypeId)
                    .HasConstraintName("FK_Wynik_wyscigu_Rodzaj_wyniku");

                entity.HasOne(d => d.RaceIdNavigation)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.RaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_wyscigu_Wydarzenie");

                entity.HasOne(d => d.TeamIdNavigation)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_Wynik_wyscigu_Zespol");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.TeamId);

                entity.ToTable("Zespol");

                entity.Property(e => e.TeamId).HasColumnName("id_zespolu");

                entity.Property(e => e.EngineSupplier)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("dostawca_silnika");

                entity.Property(e => e.CountryId).HasColumnName("id_kraju");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.HasOne(d => d.CountryIdNavigation)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_Zespol_Kraj");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
