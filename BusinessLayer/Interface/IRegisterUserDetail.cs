using DatingApp.Entities;

namespace DatingApp.BusinessLayer.Interface
{
    public interface IRegisterUserDetail
    {
        Task RegisterUser(AppUser appUser);
        Task<bool> UserExists(string username);
        Task<AppUser> GetUserDetail(string username);
    }
}
