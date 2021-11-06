using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController //the controler need to derive from "ControllerBase"
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;

        }


        [HttpGet]

        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        //[FromQuery] = cuse its a querey string we need to specify from querey, give API controller this attre [FromQuery] so it knoew to get it from querey 
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddpaginationHeader(users.CurrentPage, users.PageSize,
             users.ToltalCount, users.TotalPages);

            return Ok(users);
        }


        [HttpGet("{username}", Name = "GetUser")] //Name = root name to be used as parametar in add-photo reqest
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var currentUsername = User.GetUsername();
            return await _unitOfWork.UserRepository
                         .GetMemberAsync(username, isCurrentUser: currentUsername  == username);
        }

        [HttpPut]//this request is used to update , no para is passed beco this request is the only on in the file
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());

            _mapper.Map(memberUpdateDto, user);//this auto maper will map prop automaticly 
            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // if (user.Photos.Count == 0)
            // {
            //     photo.isMine = true;
            // }

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
            {
                // return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));//thsi is going to return a 201 response 
            }


            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());//get the user by name

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.isMine) return BadRequest("this is aready your main photo");//if the user set main to main
            //if they wher able to set main to main set main fals then new to true
            var currentMain = user.Photos.FirstOrDefault(x => x.isMine);
            if (currentMain != null) currentMain.isMine = false;
            photo.isMine = true;

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to set main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());//get user object

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);//get photo that user wants to delelt

            if (photo == null) return NotFound();//make sure photo not null

            if (photo.isMine) return BadRequest("You Cant Delete Your Main Photo");//cont del main

            if (photo.PublicId != null)//only photo we want to del must have public id
            {
                var result = await _photoService.DeletePhhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Filled to del photo");
        }
    }
}