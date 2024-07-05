using Microsoft.AspNetCore.Mvc;
using DatingApp.Entities;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserDetails _userDetails; 
        public UsersController(IUserDetails userDetails)
        {
            _userDetails = userDetails;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AppUser>>> GetUsers()
        {
            try
            {
                return Ok(await _userDetails.GetUsersDetails());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserDetailsById(int id)
        {
            try
            {
                var user = await _userDetails.GetUserDetailsById(id);

                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
