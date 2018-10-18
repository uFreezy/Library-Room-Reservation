﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LibRes.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LibRes.App.Data
{
    public class DbUserSeeder
    {
        private const string _adminRoleName = "administrator";
        private string _adminEmail = "admin@walkfido.local";
        private string _adminPassword = "Hellofido!1234";

        private string[] _defaultRoles = new string[] { _adminRoleName, "customer" };

        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public static async Task Run(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var instance = serviceScope.ServiceProvider.GetService<DbUserSeeder>();
                await instance.Initialize();
            }
        }

        public DbUserSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Initialize()
        {
            await EnsureRoles();
            await EnsureDefaultUser();
        }

        protected async Task EnsureRoles()
        {
            foreach (var role in _defaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }

        protected async Task EnsureDefaultUser()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync(_adminRoleName);

            if (!adminUsers.Any())
            {
                var adminUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = _adminEmail,
                    UserName = _adminEmail
                };

                var result = await _userManager.CreateAsync(adminUser, _adminPassword);
                await _userManager.AddToRoleAsync(adminUser, _adminRoleName);
            }
        }

    }
}
