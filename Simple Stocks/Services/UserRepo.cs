using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class UserRepo : IUserRepo
    {
        private readonly StocksDbContext _dbContext;

        public UserRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUser(User user)
        {
            await _dbContext.Set<User>().AddAsync(user);
            await SaveChanges();
        }

        public async Task UpdateUserAsync(User user)
        {
            _dbContext.Set<User>().Update(user);
            await SaveChanges();
        }

        public async Task DeleteUserAsync(User user)
        {
            _dbContext.Set<User>().Remove(user);
            await SaveChanges();
        }

        public async Task<ICollection<User>> GetAllBlockedUsers(int id)
        {
            return await _dbContext.Set<UserBlock>().Where(ub => ub.SourceUser.Id == id).Select(ub => ub.BlockedUser).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Comment>> GetAllComments(int id)
        {
            return await _dbContext.Set<Comment>().Where(c => c.UserID == id).AsNoTracking().ToListAsync();
        }

        public Task<ICollection<Stock>> GetAllFollowedStocks(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<User>> GetAllFollowedUsers(int id)
        {
            return await _dbContext.Set<UserFollow>().Where(uf => uf.SourceUser.Id == id).Select(uf => uf.FollowedUser).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<User>> GetAllFollowers(int id)
        {
            return await _dbContext.Set<UserFollow>().Where(uf => uf.FollowedUser.Id == id).Select(uf => uf.SourceUser).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Post>> GetUserFeed(int id)
        {
            ICollection<User> users = await _dbContext.Set<UserFollow>().Where(uf => uf.SourceUser.Id == id).Select(uf => uf.FollowedUser).AsNoTracking().ToListAsync();

            ICollection<Post> feed = new List<Post>();

            foreach(User user in users)
            {
                ICollection<Post> userPosts = await _dbContext.Set<Post>().Where(p => p.UserID == user.Id).AsNoTracking().ToListAsync();

                foreach(Post post in userPosts)
                {
                    feed.Add(post);
                }

                userPosts.Clear();
            }

            return feed;
        }

        public async Task<ICollection<Post>> GetAllLikedPosts(int id)
        {
            return await _dbContext.Set<LikedPost>().Where(lp => lp.User.Id == id).Select(lp => lp.Post).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Post>> GetAllPosts(int id)
        {
            return await _dbContext.Set<Post>().Where(p => p.UserID == id && p.PostIsHidden == false && p.PostIsPrivate == false).AsNoTracking().ToListAsync();
        }
        public async Task<ICollection<Post>> GetPersonalPosts(int id)
        {
            return await _dbContext.Set<Post>().Where(p => p.UserID == id).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Comment>> GetLikedComments(int id)
        {
            return await _dbContext.Set<LikedComment>().Where(lc => lc.User.Id == id).Select(lc => lc.Comment).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<User>> GetAllUsers()
        {
            return await _dbContext.Set<User>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<User>> GetSearchedUsers(string searchTerm)
        {
            return await _dbContext.Set<User>().Where(u => u.Username.Contains(searchTerm) && u.AccountIsHidden == false).AsNoTracking().ToListAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext.Set<User>().Where(u => u.Email == email && u.AccountIsHidden == false).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.Set<User>().Where(u => u.Id == userId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByToken(string refeshToken)
        {
            return await _dbContext.Set<User>().Where(u => u.RefreshToken.Equals(refeshToken)).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByPhone(string phone)
        {
            return await _dbContext.Set<User>().Where(u => u.PhoneNumber == phone && u.AccountIsHidden == false).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByRealName(string firstName, string lastName)
        {
            return await _dbContext.Set<User>().Where(u => u.FirstName == firstName && u.LastName == lastName && u.AccountIsHidden == false).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _dbContext.Set<User>().Where(u => u.Username == username && u.AccountIsHidden == false).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<string> IsDuplicate(string userName, string email, string phoneNumber)
        {
            var checkUserName = await _dbContext.Set<User>().Where(u => u.Username == userName).AsNoTracking().FirstOrDefaultAsync();
            var checkEmail = await _dbContext.Set<User>().Where(u => u.Email == email).AsNoTracking().FirstOrDefaultAsync();
            var checkPhone = await _dbContext.Set<User>().Where(u => u.PhoneNumber == phoneNumber).AsNoTracking().FirstOrDefaultAsync();

            if (checkUserName != null)
            {
                return "Username";
            }

            if (checkEmail != null)
            {
                return "Email";
            }

            if (checkPhone != null)
            {
                return "Phone Number";
            }

            return String.Empty;
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<Right>> GetUserRights(int roleId)
        {
            return await _dbContext.Set<RoleRight>().Where(ro => ro.Role.Id == roleId).Select(ro => ro.Right).AsNoTracking().ToListAsync();
        }
    }
}
