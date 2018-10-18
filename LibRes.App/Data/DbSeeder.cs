using System.Collections.Generic;
using System.Linq;
using LibRes.App.DbModels;
using LibRes.App.Models;
using Microsoft.AspNetCore.Identity;

namespace LibRes.App.Data
{
    public static class DbSeeder
	{
        public static void CreateSeedData
        (this LibResDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.SampleModels.Any())
            {
                var movies = new List<SampleModel>()
               {
                new SampleModel()
                    {
                         Name = "Avengers: Infinity War",
                         Year = 2018
                    },
                    new SampleModel()
                    {
                         Name = "Thor: Ragnarock",
                         Year = 2017
                    },
                new SampleModel()
                    {
                         Name = "Black Panther",
                         Year = 2018
                    }
               };
                context.AddRange(movies);
            }

            if (userManager.FindByEmailAsync("abc@xyz.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "abc@xyz.com",
                    Email = "abc@xyz.com",
                };

                IdentityResult result = userManager.CreateAsync(user, "Tapaka2000!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            context.SaveChanges();
        }
    }
}
