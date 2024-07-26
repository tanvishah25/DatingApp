using DatingApp.BusinessLayer.Interface;
using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.BusinessLayer
{
    public class UserRepository : IUserRepository
    {
        private DataContext _context;
        public UserRepository(DataContext dataContext) {
            _context = dataContext;
        }
        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _context.Users.Include(x=>x.Photos).ToListAsync();
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByNameAsync(string name)
        {
            return await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == name);
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
