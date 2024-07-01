using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DatingApp.Models
{
    public class RegisterUserDetail : IRegisterUserDetail
    {
        DataContext _dataContext;
        public RegisterUserDetail(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _dataContext.Users.AnyAsync(x=>x.UserName.Equals(username.ToLower()));
        }

        public async Task RegisterUser(AppUser appUser)
        {
            _dataContext.Users.Add(appUser);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<AppUser> GetUserDetail(string username)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName.Equals(username.ToLower()));
        }
    }
}
