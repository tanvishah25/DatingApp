using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace DatingApp
{
    public class UserDetails : IUserDetails
    {
        public DataContext _dataContext;
        public UserDetails(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }
        public async Task<List<AppUser>> GetUsersDetails()
        {
            try
            {
                return await _dataContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AppUser> GetUser(int id)
        {
            try
            {
                return await _dataContext.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
