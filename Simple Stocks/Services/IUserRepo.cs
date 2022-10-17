using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IUserRepo
    {
        Task<ICollection<User>> GetAllUsers();
        Task<ICollection<User>> GetSearchedUsers(string searchTerm);
        Task<User> GetUserById(int userId);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByPhone(string phone);
        Task<User> GetUserByRealName(string firstName, string lastName);
        Task<ICollection<Post>> GetAllPosts(int id);
        Task<ICollection<Post>> GetPersonalPosts(int id);
        Task<ICollection<Post>> GetAllLikedPosts(int id);
        Task<ICollection<Stock>> GetAllFollowedStocks(int id);
        Task<ICollection<Comment>> GetAllComments(int id);
        Task<ICollection<Comment>> GetLikedComments(int id);
        Task<ICollection<User>> GetAllFollowedUsers(int id);
        Task<ICollection<User>> GetAllFollowers(int id);
        Task<ICollection<User>> GetAllBlockedUsers(int id);
        Task<ICollection<Right>> GetUserRights(int roleId);
        Task<string> IsDuplicate(string userName, string email, string phoneNumber);
        Task AddUser(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task SaveChanges();
    }
}
