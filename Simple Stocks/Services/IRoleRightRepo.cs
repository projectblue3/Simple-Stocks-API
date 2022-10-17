using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IRoleRightRepo
    {
        Task<ICollection<RoleRight>> GetAllRoleRights();
        Task<RoleRight> SearchByRoleAndRightIds(int roleId, int rightId);
        Task<ICollection<RoleRight>> SearchByRoleId(int roleId);
        Task<ICollection<RoleRight>> SearchByRightId(int rightId);
        Task AddRoleRight(RoleRight roleRight);
        Task UpdateRoleRight(RoleRight roleRight);
        Task DeleteRoleRight(RoleRight roleRight);
        Task SaveChanges();
    }
}
