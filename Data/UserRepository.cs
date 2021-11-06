using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username, bool? isCurrentUser)
        {
            var query = _context.Users
            .Where(x => x.UserName == username)
           .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
           .AsQueryable();
            if ((bool)isCurrentUser) query = query.IgnoreQueryFilters();
            return await query.FirstOrDefaultAsync();
        }




        // public async Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser)
        // {
        //     var querey =   _context.Users
        //            .Where(x => x.UserName == username)
        //            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        //            .AsQueryable();
        //     //    .Select(user => new MemberDto{
        //     //        Id = user.Id,
        //     //        Username = user.UserName//etc

        //     //    })

        //     if(isCurrentUser) querey = querey.IgnoreQueryFilters();

        //     return await querey.FirstOrDefaultAsync();
        // }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var querey =  _context.Users.AsQueryable();
                //    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                //    .AsNoTracking()//remove the tracking form EF (only read)
                //    .AsQueryable();

            querey = querey.Where(u => u.UserName != userParams.CurrentUsername);
            querey = querey.Where(u => u.Gender == userParams.Gender);
            
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            querey = querey.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            querey = userParams.OrderBy switch 
            {
                "created" => querey.OrderByDescending(u => u.Created), //first case
                _ => querey.OrderByDescending(u => u.LastActive) // _ = defult case , second case

            };


            return await PagedList<MemberDto>.CreateAsync(querey.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking()
            , userParams.PageNumber, userParams.PageSize);
                //    .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await _context.Users
                     .Include(p => p.Photos)
                     .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await _context.Users
                            .Include(p => p.Photos)
                            .IgnoreQueryFilters()
                            .Where(x => x.Photos.Any(i => i.Id == photoId))
                            .FirstOrDefaultAsync();

        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
                       .Include(p => p.Photos)
                       .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetUserGender(string username) // this give us the gender prop from the DB
        {
            return await _context.Users
            .Where(x => x.UserName == username)
            .Select(g => g.Gender)
            .FirstOrDefaultAsync();
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}