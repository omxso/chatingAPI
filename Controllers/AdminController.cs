using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequierAdminRole")] // we dont have this policy yet
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWIthRoles()
        {
            var users = await _userManager.Users
                        .Include(r => r.UserRoles)
                        .ThenInclude(r => r.Role)
                        .OrderBy(u => u.UserName)
                        .Select(u => new
                        {
                            u.Id,
                            Username = u.UserName,
                            Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                        })
                        .ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound(" user not found");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Faild to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectRoles));

            if (!result.Succeeded) return BadRequest("faild to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            var photo = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();

            return Ok(photo);
        }

        [Authorize(Policy = "ModeratePhotoRole")] // we dont have this policy yet
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovedPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if(photo == null) return NotFound("Could not find photo");

            photo.isApproved = true;

            var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);

            if(!user.Photos.Any(x => x.isMine)) photo.isMine = true;

            await _unitOfWork.Complete();

            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhhotoAsync(photo.PublicId); 
            } else
            {
                _unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
            

            await _unitOfWork.Complete();

            return Ok();
        }
    }
}