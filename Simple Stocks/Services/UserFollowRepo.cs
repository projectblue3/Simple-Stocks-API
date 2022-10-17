using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class UserFollowRepo : IUserFollowRepo
    {
        private readonly StocksDbContext _dbContext;

        public UserFollowRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task FollowUser(UserFollow userFollow)
        {
            _dbContext.Set<UserFollow>().Add(userFollow);
            await SaveChanges();
        }

        public async Task DeleteUserFollow(UserFollow userFollow)
        {
            _dbContext.Set<UserFollow>().Remove(userFollow);
            await SaveChanges();
        }

        public async Task UpdateUserFollow(UserFollow userFollow)
        {
            _dbContext.Set<UserFollow>().Update(userFollow);
            await SaveChanges();
        }

        public async Task<ICollection<UserFollow>> GetAllFollows()
        {
            return await _dbContext.Set<UserFollow>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<UserFollow>> SearchFollowedUsers(int userId)
        {
            return await _dbContext.Set<UserFollow>().Where(uf => uf.SourceUserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task<UserFollow> SearchForFollowedUser(int userId, int followedUserId)
        {
            return await _dbContext.Set<UserFollow>().Where(uf => uf.SourceUserId == userId && uf.FollowedUserId == followedUserId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<UserFollow>> SearchForFollowers(int userId)
        {
            return await _dbContext.Set<UserFollow>().Where(uf => uf.FollowedUserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
