
using Microsoft.EntityFrameworkCore;
using RaceDay.Models;

namespace RaceDay.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<RaceDay.Models.Route> Routes { get; set; }
        public DbSet<Weather> WeatherRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
         new Role
        {
        RoleID = 1,
        RoleName = "Organiser",
        Description = "Manages race events and participants."
         },
        new Role
        {
        RoleID = 2,
        RoleName = "Participant",
        Description = "Registers for race events and views results."
         }
            );

            modelBuilder.Entity<Category>()
            .Property(c => c.DistanceKM)
            .HasPrecision(10, 2);

            modelBuilder.Entity<EventCategory>()
                .Property(ec => ec.EntryFee)
                .HasPrecision(10, 2);

            modelBuilder.Entity<RaceDay.Models.Route>()
                .Property(r => r.DistanceKM)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.Temperature)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.FeelsLike)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.Humidity)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.WindSpeed)
                .HasPrecision(6, 2);

            modelBuilder.Entity<UserProfile>()
                .HasKey(p => p.ProfileID);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(p => p.UserID);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.OrganisedEvents)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventCategory>()
                .HasOne(ec => ec.Event)
                .WithMany(e => e.EventCategories)
                .HasForeignKey(ec => ec.EventID);

            modelBuilder.Entity<EventCategory>()
                .HasOne(ec => ec.Category)
                .WithMany(c => c.EventCategories)
                .HasForeignKey(ec => ec.CategoryID);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.EventCategory)
                .WithMany(ec => ec.Enrolments)
                .HasForeignKey(e => e.EventCategoryID);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID);

            modelBuilder.Entity<RaceDay.Models.Route>()
                .HasOne(r => r.Event)
                .WithOne(e => e.Route)
                .HasForeignKey<RaceDay.Models.Route>(r => r.EventID);

            modelBuilder.Entity<Weather>()
                .HasOne(w => w.Event)
                .WithMany(e => e.WeatherRecords)
                .HasForeignKey(w => w.EventID);
        }
    }
}
