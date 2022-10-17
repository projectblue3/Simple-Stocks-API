using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IUserFollowRepo
    {
        Task<ICollection<UserFollow>> GetAllFollows();
        Task<UserFollow> SearchForFollowedUser(int userId, int followedUserId);
        Task<ICollection<UserFollow>> SearchFollowedUsers(int userId);
        Task<ICollection<UserFollow>> SearchForFollowers(int userId);
        Task FollowUser(UserFollow userFollow);
        Task UpdateUserFollow(UserFollow userFollow);
        Task DeleteUserFollow(UserFollow userFollow);
        Task SaveChanges();
    }
}