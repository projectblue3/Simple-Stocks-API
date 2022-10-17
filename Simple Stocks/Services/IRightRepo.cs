using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IRightRepo
    {
        Task<ICollection<Right>> GetAllRights();
        Task<ICollection<Role>> GetAllRolesWithRight(int id);
        Task<Right> GetRightById(int id);
        Task<Right> GetRightByTitle(string title);
        Task AddRight(Right right);
        Task UpdateRight(Right right);
        Task DeleteRight(Right right);
        Task<bool> IsDuplicate(string title);
        Task SaveChanges();
    }
}
