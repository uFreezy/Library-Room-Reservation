using LibRes.App.DbModels;
using LibRes.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibRes.App.Data
{
    public class LibResDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SampleModel> SampleModels { get; set; }
        public LibResDbContext(
             DbContextOptions<LibResDbContext> options)
             : base(options)
        {
            this.Database.Migrate();
        }

        public LibResDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });
        }

    }
}
