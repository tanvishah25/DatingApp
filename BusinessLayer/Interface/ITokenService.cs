using DatingApp.Entities;

namespace DatingApp.BusinessLayer.Interface
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
