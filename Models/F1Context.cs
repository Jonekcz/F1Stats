using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        public virtual DbSet<KlasyfikacjaKierowca> DriversStandings { get; set; }
        // constructor standings procedure
        public virtual DbSet<KlasyfikacjaZespol> TeamsStandings { get; set; }

        public virtual DbSet<CzasOkrazenium> CzasOkrazenia { get; set; }
        public virtual DbSet<Kierowca> Kierowcas { get; set; }
        public virtual DbSet<Kontynent> Kontynents { get; set; }
        public virtual DbSet<Kraj> Krajs { get; set; }
        public virtual DbSet<Miasto> Miastos { get; set; }
        public virtual DbSet<Pitstop> Pitstops { get; set; }
        public virtual DbSet<PunktyZaWyscig> PunktyZaWyscigs { get; set; }
        public virtual DbSet<RodzajWyniku> RodzajWynikus { get; set; }
        public virtual DbSet<Sezon> Sezons { get; set; }
        public virtual DbSet<Tor> Tors { get; set; }
        public virtual DbSet<Wydarzenie> Wydarzenies { get; set; }
        public virtual DbSet<WynikKwalifikacji> WynikKwalifikacjis { get; set; }
        public virtual DbSet<WynikWyscigu> WynikWyscigus { get; set; }
        public virtual DbSet<Zespol> Zespols { get; set; }
        
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

            modelBuilder.Entity<CzasOkrazenium>(entity =>
            {
                entity.HasKey(e => new { e.IdWyscigu, e.IdKierowcy, e.Okrazenie });

                entity.ToTable("Czas_okrazenia");

                entity.Property(e => e.IdWyscigu).HasColumnName("id_wyscigu");

                entity.Property(e => e.IdKierowcy).HasColumnName("id_kierowcy");

                entity.Property(e => e.Okrazenie).HasColumnName("okrazenie");

                entity.Property(e => e.Czas).HasColumnName("czas");

                entity.HasOne(d => d.IdKierowcyNavigation)
                    .WithMany(p => p.CzasOkrazenia)
                    .HasForeignKey(d => d.IdKierowcy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Czas_okrazenia_Kierowca");

                entity.HasOne(d => d.IdWysciguNavigation)
                    .WithMany(p => p.CzasOkrazenia)
                    .HasForeignKey(d => d.IdWyscigu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Czas_okrazenia_Wydarzenie");
            });

            modelBuilder.Entity<Kierowca>(entity =>
            {
                entity.HasKey(e => e.IdKierowcy)
                    .HasName("PK__Kierowca__EE994F76AAEC5DCE");

                entity.ToTable("Kierowca");

                entity.HasIndex(e => e.IdKraju, "id_kraju_idx");

                entity.Property(e => e.IdKierowcy).HasColumnName("id_kierowcy");

                entity.Property(e => e.DataUrodzenia)
                    .HasColumnType("date")
                    .HasColumnName("data_urodzenia");

                entity.Property(e => e.IdKraju).HasColumnName("id_kraju");

                entity.Property(e => e.Imie)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("imie");

                entity.Property(e => e.Nazwisko)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("nazwisko");

                entity.HasOne(d => d.IdKrajuNavigation)
                    .WithMany(p => p.Kierowcas)
                    .HasForeignKey(d => d.IdKraju)
                    .HasConstraintName("id_kraju");
            });

            modelBuilder.Entity<Kontynent>(entity =>
            {
                entity.HasKey(e => e.IdKontynentu)
                    .HasName("PK__Kontynen__786308534077C461");

                entity.ToTable("Kontynent");

                entity.Property(e => e.IdKontynentu).HasColumnName("id_kontynentu");

                entity.Property(e => e.Nazwa)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");
            });

            modelBuilder.Entity<Kraj>(entity =>
            {
                entity.HasKey(e => e.IdKraju)
                    .HasName("PK__Kraj__E2DD85FF74F854E0");

                entity.ToTable("Kraj");

                entity.HasIndex(e => e.IdKontynentu, "id__idx");

                entity.Property(e => e.IdKraju).HasColumnName("id_kraju");

                entity.Property(e => e.IdKontynentu).HasColumnName("id_kontynentu");

                entity.Property(e => e.Nazwa)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.HasOne(d => d.IdKontynentuNavigation)
                    .WithMany(p => p.Krajs)
                    .HasForeignKey(d => d.IdKontynentu)
                    .HasConstraintName("id_kontynentu");
            });

            modelBuilder.Entity<Miasto>(entity =>
            {
                entity.HasKey(e => e.IdMiasta);

                entity.ToTable("Miasto");

                entity.Property(e => e.IdMiasta).HasColumnName("id_miasta");

                entity.Property(e => e.IdKraju).HasColumnName("id_kraju");

                entity.Property(e => e.Nazwa)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.HasOne(d => d.IdKrajuNavigation)
                    .WithMany(p => p.Miastos)
                    .HasForeignKey(d => d.IdKraju)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Miasto_Kraj");
            });

            modelBuilder.Entity<Pitstop>(entity =>
            {
                entity.HasKey(e => e.IdPitstopu);

                entity.ToTable("Pitstop");

                entity.Property(e => e.IdPitstopu).HasColumnName("id_pitstopu");

                entity.Property(e => e.CzasTrwania)
                    .HasColumnType("time(3)")
                    .HasColumnName("Czas_trwania");

                entity.Property(e => e.IdKierowcy).HasColumnName("id_kierowcy");

                entity.Property(e => e.IdWyscigu).HasColumnName("id_wyscigu");

                entity.HasOne(d => d.IdKierowcyNavigation)
                    .WithMany(p => p.Pitstops)
                    .HasForeignKey(d => d.IdKierowcy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pitstop_Kierowca");

                entity.HasOne(d => d.IdWysciguNavigation)
                    .WithMany(p => p.Pitstops)
                    .HasForeignKey(d => d.IdWyscigu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pitstop_Wydarzenie");
            });

            modelBuilder.Entity<PunktyZaWyscig>(entity =>
            {
                entity.ToTable("Punkty_za_wyscig");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Punkty).HasColumnName("punkty");
            });

            modelBuilder.Entity<RodzajWyniku>(entity =>
            {
                entity.HasKey(e => e.IdRodzajuWyniku);

                entity.ToTable("Rodzaj_wyniku");

                entity.Property(e => e.IdRodzajuWyniku).HasColumnName("id_rodzaju_wyniku");

                entity.Property(e => e.Nazwa)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");
            });

            modelBuilder.Entity<Sezon>(entity =>
            {
                entity.HasKey(e => e.IdSezonu);

                entity.ToTable("Sezon");

                entity.Property(e => e.IdSezonu).HasColumnName("id_sezonu");

                entity.Property(e => e.Rok).HasColumnName("rok");
            });

            modelBuilder.Entity<Tor>(entity =>
            {
                entity.HasKey(e => e.IdToru);

                entity.ToTable("Tor");

                entity.Property(e => e.IdToru).HasColumnName("id_toru");

                entity.Property(e => e.DlugoscGeo)
                    .HasColumnType("decimal(9, 6)")
                    .HasColumnName("dlugosc_geo");

                entity.Property(e => e.IdMiasta).HasColumnName("id_miasta");

                entity.Property(e => e.Nazwa)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.Property(e => e.SzerokoscGeo)
                    .HasColumnType("decimal(8, 6)")
                    .HasColumnName("szerokosc_geo");

                entity.HasOne(d => d.IdMiastaNavigation)
                    .WithMany(p => p.Tors)
                    .HasForeignKey(d => d.IdMiasta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tor_Miasto");
            });

            modelBuilder.Entity<Wydarzenie>(entity =>
            {
                entity.HasKey(e => e.IdTerminarza);

                entity.ToTable("Wydarzenie");

                entity.Property(e => e.IdTerminarza).HasColumnName("id_terminarza");

                entity.Property(e => e.CzerwonaFlaga).HasColumnName("czerwona_flaga");

                entity.Property(e => e.DataCzas)
                    .HasColumnType("datetime")
                    .HasColumnName("data_czas");

                entity.Property(e => e.IdSezonu).HasColumnName("id_sezonu");

                entity.Property(e => e.IdToru).HasColumnName("id_toru");

                entity.Property(e => e.Pogoda)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("pogoda");

                entity.HasOne(d => d.IdSezonuNavigation)
                    .WithMany(p => p.Wydarzenies)
                    .HasForeignKey(d => d.IdSezonu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wydarzenie_Sezon");

                entity.HasOne(d => d.IdToruNavigation)
                    .WithMany(p => p.Wydarzenies)
                    .HasForeignKey(d => d.IdToru)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wydarzenie_Tor");
            });

            modelBuilder.Entity<WynikKwalifikacji>(entity =>
            {
                entity.HasKey(e => new { e.IdKwalifikacji, e.IdKierowcy });

                entity.ToTable("Wynik_kwalifikacji");

                entity.Property(e => e.IdKwalifikacji).HasColumnName("id_kwalifikacji");

                entity.Property(e => e.IdKierowcy).HasColumnName("id_kierowcy");

                entity.Property(e => e.CzasQ1)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas_q1");

                entity.Property(e => e.CzasQ2)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas_q2");

                entity.Property(e => e.CzasQ3)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas_q3");

                entity.Property(e => e.IdZespolu).HasColumnName("id_zespolu");

                entity.Property(e => e.Pozycja).HasColumnName("pozycja");

                entity.HasOne(d => d.IdKierowcyNavigation)
                    .WithMany(p => p.WynikKwalifikacjis)
                    .HasForeignKey(d => d.IdKierowcy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_kwalifikacji_Kierowca");

                entity.HasOne(d => d.IdKwalifikacjiNavigation)
                    .WithMany(p => p.WynikKwalifikacjis)
                    .HasForeignKey(d => d.IdKwalifikacji)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_kwalifikacji_Wydarzenie");

                entity.HasOne(d => d.IdZespoluNavigation)
                    .WithMany(p => p.WynikKwalifikacjis)
                    .HasForeignKey(d => d.IdZespolu)
                    .HasConstraintName("FK_Wynik_kwalifikacji_Zespol");
            });

            modelBuilder.Entity<WynikWyscigu>(entity =>
            {
                entity.HasKey(e => new { e.IdWyscigu, e.IdKierowcy });

                entity.ToTable("Wynik_wyscigu");

                entity.Property(e => e.IdWyscigu).HasColumnName("id_wyscigu");

                entity.Property(e => e.IdKierowcy).HasColumnName("id_kierowcy");

                entity.Property(e => e.Czas)
                    .HasColumnType("time(3)")
                    .HasColumnName("czas");

                entity.Property(e => e.IdRodzajuWyniku).HasColumnName("id_rodzaju_wyniku");

                entity.Property(e => e.IdZespolu).HasColumnName("id_zespolu");

                entity.Property(e => e.NajlepszyCzasOkrazenia)
                    .HasColumnType("time(3)")
                    .HasColumnName("najlepszy_czas_okrazenia");

                entity.Property(e => e.Pozycja).HasColumnName("pozycja");

                entity.Property(e => e.Punkty).HasColumnName("punkty");

                entity.HasOne(d => d.IdKierowcyNavigation)
                    .WithMany(p => p.WynikWyscigus)
                    .HasForeignKey(d => d.IdKierowcy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_wyscigu_Kierowca");

                entity.HasOne(d => d.IdRodzajuWynikuNavigation)
                    .WithMany(p => p.WynikWyscigus)
                    .HasForeignKey(d => d.IdRodzajuWyniku)
                    .HasConstraintName("FK_Wynik_wyscigu_Rodzaj_wyniku");

                entity.HasOne(d => d.IdWysciguNavigation)
                    .WithMany(p => p.WynikWyscigus)
                    .HasForeignKey(d => d.IdWyscigu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wynik_wyscigu_Wydarzenie");

                entity.HasOne(d => d.IdZespoluNavigation)
                    .WithMany(p => p.WynikWyscigus)
                    .HasForeignKey(d => d.IdZespolu)
                    .HasConstraintName("FK_Wynik_wyscigu_Zespol");
            });

            modelBuilder.Entity<Zespol>(entity =>
            {
                entity.HasKey(e => e.IdZespolu);

                entity.ToTable("Zespol");

                entity.Property(e => e.IdZespolu).HasColumnName("id_zespolu");

                entity.Property(e => e.DostawcaSilnika)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("dostawca_silnika");

                entity.Property(e => e.IdKraju).HasColumnName("id_kraju");

                entity.Property(e => e.Nazwa)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("nazwa");

                entity.HasOne(d => d.IdKrajuNavigation)
                    .WithMany(p => p.Zespols)
                    .HasForeignKey(d => d.IdKraju)
                    .HasConstraintName("FK_Zespol_Kraj");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
