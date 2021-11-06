using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);//this is enoighto find the indivual like becuse both are primary keys
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)//return eitther who liked user or who the user liked
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if( likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if(likesParams.Predicate == "likedBy") //list of users that have liked the loged in user
            {
              likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
               users = likes.Select(like => like.SourceUser);   
            }

            var likedUsers = users.Select(user => new LikeDto 
            {
                Username = user.UserName,
                KownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.isMine).url,
                City = user.City,
                Id = user.Id 
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId) //list of user that this user has liked
        {
            return await _context.Users 
                 .Include(x => x.LikesUsers)
                 .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}