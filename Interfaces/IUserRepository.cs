using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string username);
        // Task<IEnumerable<MemberDto>> GetMembersAsync();//task that returen a list of member dto (Old)
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);//task that returen a list of member dto using pagination (New)
        Task<MemberDto> GetMemberAsync(string username, bool? isCurrentUser);
        Task<string> GetUserGender(string username);
        Task<AppUser> GetUserByPhotoId(int photoId);
    }
}