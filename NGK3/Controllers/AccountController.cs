using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using WebApi.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NGK3.Models;
using WebApi;
using static BCrypt.Net.BCrypt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NGK3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private const int BcryptWorkfactor = 10;
        private WeatherContext _context;
        private readonly AppSettings _appSettings;
        public AccountController(WeatherContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserDto regUser)
        {
            regUser.Email = regUser.Email.ToLower();
            var emailExist = await _context.User.Where(u => u.Email == regUser.Email).FirstOrDefaultAsync();

            if (emailExist != null)
            {
                return BadRequest(new {errorMessage = "Email already in use"});
            }

            User user = new User
            {
                Email = regUser.Email,
                FirstName = regUser.FirstName,
                LastName = regUser.LastName
            };

            user.PwHash = HashPassword(regUser.Password, BcryptWorkfactor);
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Get", new {id = user.UserId}, regUser);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            } 
            var userDto = new UserDto();
            userDto.Email = user.Email;
            userDto.FirstName = user.FirstName;
            userDto.LastName = user.LastName;
            return userDto;
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<TokenDto>> Login(UserDto login)
        {
            login.Email = login.Email.ToLower();
            var user = await _context.User.Where(u => u.Email == login.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                var validPwd = Verify(login.Password, user.PwHash);
                if (validPwd)
                {
                    var token = new TokenDto();
                    token.JWT = GenerateToken(user);
                    return token;
                }
            } 
            ModelState.AddModelError(string.Empty, "Forkert brugernavn eller password");
            return BadRequest(ModelState);
        }

        private string GenerateToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim("Email", user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,
                    new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            }; 
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey); 
            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)),
                    new JwtPayload(claims));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
