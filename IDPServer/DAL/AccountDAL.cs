using Microsoft.AspNetCore.Identity;

namespace IDPServer.DAL
{
    public class AccountDAL : IAccountIDP
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountDAL(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddRole(string roleName)
        {
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole
                    {
                        Name = roleName
                    };
                    var result = await _roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new ArgumentException("Role creation failed");
                    }
                }
                else
                {
                    throw new ArgumentException("Role already exists");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task AddRolesToUser(string username, List<string> roleNames)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    var result = await _userManager.AddToRolesAsync(user, roleNames);
                    if (!result.Succeeded)
                    {
                        throw new ArgumentException("User assignment to roles failed");
                    }
                }
                else
                {
                    throw new ArgumentException("User not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddUserToRole(string username, string roleName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    throw new ArgumentException("Role does not exist");
                }
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    throw new ArgumentException("User assignment to role failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteRole(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new ArgumentException("Role deletion failed");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IdentityRole> GetRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new ArgumentException("Role not found");
            }
            return role;
        }

        public async Task<IEnumerable<string>> GetRolesFromUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<IdentityUser> GetUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            return user;
        }

        public Task<IdentityUser> GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                throw new ArgumentException("Invalid password");
            }
            return result;
        }

        public async Task<IdentityUser> Register(IdentityUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    return user;
                }
                else
                {
                    throw new ArgumentException("User registration failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
