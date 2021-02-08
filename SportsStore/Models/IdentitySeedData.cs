using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";


        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            var userManager = (UserManager<IdentityUser>)app.ApplicationServices
                .GetService(typeof(UserManager<IdentityUser>));

            var roleManager = (RoleManager<IdentityRole>)app.ApplicationServices
                .GetService(typeof(RoleManager<IdentityRole>));

            IdentityUser user = await userManager.FindByIdAsync(adminUser);

            if (user == null)
            {
                user = new IdentityUser(adminUser);
                await userManager.CreateAsync(user, adminPassword);

                var role = await roleManager.CreateAsync(new IdentityRole("admins"));

                user = await userManager.FindByNameAsync(adminUser);
                var a = await userManager.AddToRoleAsync(user, "admins");
            }

        }
    }
}
