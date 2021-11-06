using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        // private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            // _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);//from Dto to appuser
                                                         // using var hmac = new HMACSHA512(); // this is the hashing method// using stait mean when we done with this class its going to dispose of corctly 


            user.UserName = registerDto.Username.ToLower();
            // user.PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PassWordSalt = hmac.Key;




            // _context.Users.Add(user);
            // await _context.SaveChangesAsync();
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);// this create the user and save the changes to the Db

            if(!result.Succeeded) return BadRequest(result.Errors); // check the action has succeeded

            var roleResult = await _userManager.AddToRoleAsync(user, "Member"); // this user will auto go member role

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto // what is gooing to be rutend after logedin
            {
                Username = user.UserName,
                Token =  await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager
                        .CheckPasswordSignInAsync(user, loginDto.Password, false); // the last param is to check to lookdown the user upown faller

            if(!result.Succeeded) return Unauthorized();
            // using var hmac = new HMACSHA512(user.PassWordSalt);

            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // for (int i = 0; i < computedHash.Length; i++)
            // {
            //     if (computedHash[i] != user.PassWordHash[i]) return Unauthorized("Invalid password");
            // }

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMine)?.url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}