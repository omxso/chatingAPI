using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserRole : IdentityUserRole<int> // thid actting as join table
    {
        public AppUser User { get; set; }  // prop for the user
        public AppRole Role { get; set; } // prop for the role

        // this will be a many to many relationship
    }
}