using LibRes.App.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibRes.App.Data
{
    public sealed class LibResDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibResDbContext(
            DbContextOptions options)
            : base(options)
        {
        }

        public LibResDbContext()
        {
        }

        public DbSet<ReservationModel> ReservationModels { get; set; }

        public DbSet<RoomModel> RoomModels { get; set; }

        public DbSet<EventOccurenceModel> EventOccurrences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole {Name = "Admin", NormalizedName = "Admin".ToUpper()});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=tcp:libreservation.database.windows.net,1433;Initial Catalog=LibReservation;Persist Security Info=False;User ID=libresmanager;Password=Libres12345!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;");
        }
    }
}