using System.Collections.Generic;
 using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed // seed that we have generated using json
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) // UserManager: we get this from Identity to manage users
        {
            if ( await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json"); //Update the Seed Users so the initial photo is approved for seeded users 4
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if(users == null) return;

            var roles = new List<AppRole> // creating a list of roles
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

            foreach (var role in roles) // creating the role  
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                // using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                // user.PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$word"));
                // user.PassWordSalt = hmac.Key;

               await userManager.CreateAsync(user, "Pa$$w0rd");
               await userManager.AddToRoleAsync(user, "Member");// creating the role of the member
            }
            // await context.SaveChangesAsync(); // user manger save changes we dont to do this here

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});// used Roles cuse we are passing more than one role
        }
    }
}