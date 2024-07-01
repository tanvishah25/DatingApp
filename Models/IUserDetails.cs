using DatingApp.Entities;
using System.Threading.Tasks;

namespace DatingApp.Models
{
    public interface IUserDetails
    {
        Task<List<AppUser>> GetUsersDetails();
        Task<AppUser> GetUserDetailsById(int id);
    }
}
