using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IUserBlockRepo
    {
        Task<ICollection<UserBlock>> GetAllBlocks();
        Task<UserBlock> SearchForBlockedUser(int userId, int blockedUserId);
        Task<ICollection<UserBlock>> SearchBlockedUsers(int userId);
        Task BlockUser(UserBlock userBlock);
        Task UpdateUserBlock(UserBlock userBlock);
        Task DeleteUserBlock(UserBlock userBlock);
        Task SaveChanges();
    }
}
