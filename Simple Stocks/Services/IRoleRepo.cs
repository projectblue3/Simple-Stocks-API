using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IRoleRepo
    {
        Task<ICollection<Role>> GetAllRoles();
        Task<ICollection<User>> GetAllUsersWithRole(int roleId);
        Task<Role> GetRoleById(int id);
        Task<Role> GetRoleByTitle(string title);
        Task<ICollection<Right>> GetRightsOfRole(int id);
        Task AddRole(Role role, List<int> rightIds);
        Task UpdateRole(Role role);
        Task DeleteRole(Role role);
        Task<bool> IsDuplicate(string title);
        Task SaveChanges();
    }
}
