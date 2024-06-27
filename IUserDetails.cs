using DatingApp.Entities;

namespace DatingApp
{
    public interface IUserDetails
    {
        Task<List<AppUser>> GetUsersDetails();
        Task<AppUser> GetUser(int id);
    }
}
