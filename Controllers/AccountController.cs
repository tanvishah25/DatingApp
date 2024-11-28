using AutoMapper;
using DatingApp.BusinessLayer.Interface;
using DatingApp.DTOs;
using DatingApp.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IRegisterUserDetail _registerUserDetail;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(IRegisterUserDetail registerUserDetail, ITokenService tokenService, IMapper mapper) {
            _registerUserDetail = registerUserDetail;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await _registerUserDetail.UserExists(registerDto.UserName)) return BadRequest("Username already Taken");
            var hmac = new HMACSHA512();
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            await _registerUserDetail.RegisterUser(user);

            return new UserDto
            {
                Token = _tokenService.CreateToken(user),
                Username = registerDto.UserName.ToLower(),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _registerUserDetail.GetUserDetail(loginDto.UserName);
            if (user == null) return BadRequest("Invalid Username");

            var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (user.PasswordHash[i] != computedHash[i])
                    return BadRequest("Invalid Password");
            }

            return new UserDto
            {
                Token = _tokenService.CreateToken(user),
                Username = user.UserName.ToLower(),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
    }
}


