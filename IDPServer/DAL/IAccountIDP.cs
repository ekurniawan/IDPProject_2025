using Microsoft.AspNetCore.Identity;

namespace IDPServer.DAL
{
    public interface IAccountIDP
    {
        Task<IdentityUser> Register(IdentityUser user, string password);
        Task<bool> Login(string username, string password);
        Task<IdentityUser> GetUser(string username);
        Task<IdentityUser> GetUserById(string id);
        Task<IdentityRole> GetRole(string roleName);
        Task<IEnumerable<string>> GetRolesFromUser(string username);
        Task AddRole(string roleName);
        Task AddUserToRole(string username, string roleName);
        Task AddRolesToUser(string username, List<string> roleNames);
        Task DeleteRole(string roleName);
    }
}
