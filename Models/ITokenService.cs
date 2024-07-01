using DatingApp.Entities;

namespace DatingApp.Models
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
