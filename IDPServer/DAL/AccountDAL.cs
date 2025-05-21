using Microsoft.AspNetCore.Identity;

namespace IDPServer.DAL
{
    public class AccountDAL : IAccountIDP
    {
        private readonly UserManager<IdentityUser> _userManager;
        public AccountDAL(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public Task AddRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task AddRolesToUser(string username, List<string> roleNames)
        {
            throw new NotImplementedException();
        }

        public Task AddUserToRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityRole> GetRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetRolesFromUser(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser> GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser> GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser> Register(IdentityUser user, string password)
        {
            throw new NotImplementedException();
        }
    }
}
