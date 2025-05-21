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
                var roles = await _accountIDP.GetRolesFromUser(loginDTO.Username);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                claims.Add(new Claim("menu", "employee_read"));
                claims.Add(new Claim("menu", "employee_write"));
                claims.Add(new Claim("menu", "employee_delete"));
                claims.Add(new Claim("menu", "employee_read_all"));

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(30),
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

        [Authorize(Roles = "admin")]
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            try
            {
                await _accountIDP.AddRole(roleName);
                return Ok($"Role {roleName} created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("addusertorole")]
        public async Task<IActionResult> AddUserToRole(string username, string roleName)
        {
            try
            {
                await _accountIDP.AddUserToRole(username, roleName);
                return Ok($"User {username} added to role {roleName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getrolesfromuser")]
        public async Task<IActionResult> GetRolesFromUser(string username)
        {
            try
            {
                var roles = await _accountIDP.GetRolesFromUser(username);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("addroletouser")]
        public async Task<IActionResult> AddRolesToUser(string username, List<string> roleNames)
        {
            try
            {
                await _accountIDP.AddRolesToUser(username, roleNames);
                return Ok($"Roles added to user {username}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser(string username)
        {
            try
            {
                var user = await _accountIDP.GetUser(username);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                var userDto = new AccountRegisterDTO
                {
                    Username = user.UserName,
                    Email = user.Email
                };
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteuser")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            try
            {
                await _accountIDP.DeleteRole(roleName);
                return Ok($"Role {roleName} deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
