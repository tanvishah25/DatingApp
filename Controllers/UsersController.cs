using Microsoft.AspNetCore.Mvc;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using DatingApp.BusinessLayer.Interface;
using DatingApp.DTOs;
using AutoMapper;

namespace DatingApp.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        IMapper _mapper;
        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var usertoreturn= _mapper.Map<IEnumerable<MemberDto>>(users);
                return Ok(usertoreturn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        [HttpGet("GetUserDetailsByUserName/{username}")]
        public async Task<ActionResult<MemberDto>> GetUserDetailsByUserName(string username)
        {
            try
            {
                var user = await _userRepository.GetUserByNameAsync(username);

                if (user == null)
                {
                    return NotFound();
                }
                var usertoreturn = _mapper.Map<MemberDto>(user);
                return Ok(usertoreturn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
  
    }
}
