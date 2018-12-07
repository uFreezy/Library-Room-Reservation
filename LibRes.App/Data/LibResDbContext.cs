using LibRes.App.DbModels;
using LibRes.App.Models;
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

        public DbSet<EventOccurenceModel> EventOccurances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole {Name = "Admin", NormalizedName = "Admin".ToUpper()});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=tcp:librestest.database.windows.net,1433;Initial Catalog=LibResTest;Persist Security Info=False;User ID=libresmanager;Password=Libres123456!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
}