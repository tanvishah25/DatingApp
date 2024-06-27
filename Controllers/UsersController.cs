using Microsoft.AspNetCore.Mvc;
using DatingApp.Entities;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserDetails _userDetails; 
        public UsersController(IUserDetails userDetails)
        {
            _userDetails = userDetails;
        }

        [HttpGet]
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
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            try
            {
                var user = await _userDetails.GetUser(id);

                if (user == null) { 
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
