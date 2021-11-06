
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>//this comes with a string as prKey so to change that we need to spicfiy
    {
        // public int Id { get; set; } //those going to be created for as by idintity

        // public string UserName { get; set; }

        // public byte[] PassWordHash {get; set;}

        // public byte[] PassWordSalt {get; set;}

        public DateTime DateOfBirth {get; set;}
        
        public string KnownAs { get; set; }
        
        public DateTime Created { get; set; }  = DateTime.Now;

        public DateTime LastActive { get; set; } = DateTime.Now;

        public string Gender { get; set; }

        public string Introduction { get; set; }
        
        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }

        public ICollection<UserLike> LikedByUsers { get; set; }//user that like logged in users
        public ICollection<UserLike> LikesUsers { get; set; }//user that has been liked
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
         public ICollection<AppUserRole> UserRoles { get; set; }

        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }
    }
}