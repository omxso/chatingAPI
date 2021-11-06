using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            var SourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound(); //make sure the user excist

            if (SourceUser.UserName == username) return BadRequest("You cannot like yourself");//make sure the user cannot like themslefs

            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id); //check if the user already liked

            if (userLike != null) return BadRequest("You already Like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            SourceUser.LikesUsers.Add(userLike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Falied to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

            Response.AddpaginationHeader(users.CurrentPage, users.PageSize, users.ToltalCount, users.TotalPages);
            return Ok(users);
        }
    }
}