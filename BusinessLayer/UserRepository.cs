using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.BusinessLayer.Interface;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.BusinessLayer
{
    public class UserRepository : IUserRepository
    {
        private DataContext _context;
        IMapper _mapper;
        public UserRepository(DataContext dataContext, IMapper mapper) {
            _context = dataContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _context.Users.Include(x=>x.Photos).ToListAsync();
        }
        //alternative of above method
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query =  _context.Users.AsQueryable();
            query = query.Where(x => x.UserName != userParams.CurrentUserName)
                         .Where(x => userParams.Gender == null || x.Gender.Equals(userParams.Gender));
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge - 1));

            query = query.Where(x=>x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }
        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByNameAsync(string name)
        {
            return await _context.Users.Include(x => x.Photos).
                SingleOrDefaultAsync(x => x.UserName == name);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
