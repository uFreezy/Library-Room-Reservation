using System.Collections.Generic;
using LibRes.App.DbModels;
using LibRes.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibRes.App.Data
{
    public class LibResDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<ReservationModel> ReservationModels { get; set; }

        public virtual DbSet<RoomModel> RoomModels { get; set; }

        public virtual DbSet<EventOccuranceModel> EventOccurances { get; set; }

        public LibResDbContext(
             DbContextOptions<LibResDbContext> options)
             : base(options)
        {

        }

        public LibResDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                   .HasMany<ReservationModel>(a => a.Reservations)
                   .WithOne(r => r.ReservationOwner);


            builder.Entity<ReservationModel>()
                   .HasMany<EventOccuranceModel>(r => r.EventDates)
                   .WithOne(e => e.Reservation);

           /*modelBuilder.Entity<Contest>()
                  .HasMany<User>(c => c.Participants)
                  .WithMany(p => p.ContestsParticipated)
                  .Map(pc =>
                  {
                      pc.MapLeftKey("ContestId");
                      pc.MapRightKey("UserId");
                      pc.ToTable("ContestsParticipants");
                  });*/

            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        optionsBuilder.UseSqlServer("Server=localhost;Database=LibRes;User Id=sa;Password=Libres123456!");
            
        }

    }
}
