using IDPServer.DAL;
using IDPServer.DTO;
using IDPServer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IDPServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountIDP _accountIDP;
        private readonly AppSettings _appSettings;

        public AccountsController(IAccountIDP accountIDP, IOptions<AppSettings> appSettings)
        {
            _accountIDP = accountIDP;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(AccountRegisterDTO accountRegisterDTO)
        {
            var user = new IdentityUser
            {
                UserName = accountRegisterDTO.Username,
                Email = accountRegisterDTO.Email
            };

            try
            {
                var result = await _accountIDP.Register(user, accountRegisterDTO.Password);
                var accountRegisterDto = new AccountRegisterDTO
                {
                    Username = result.UserName,
                    Email = result.Email
                };
                return Ok(accountRegisterDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var result = await _accountIDP.Login(loginDTO.Username, loginDTO.Password);
                if (!result)
                {
                    return Unauthorized("Invalid credentials");
                }
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, loginDTO.Username));

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accountDto = new AccountDTO
                {
                    Username = loginDTO.Username,
                    Token = tokenHandler.WriteToken(token)
                };
                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid credentials: {ex.Message}");
            }
        }
    }
}
