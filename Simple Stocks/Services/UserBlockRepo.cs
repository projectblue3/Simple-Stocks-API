using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class UserBlockRepo : IUserBlockRepo
    {
        private readonly StocksDbContext _dbContext;

        public UserBlockRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BlockUser(UserBlock userBlock)
        {
            _dbContext.Set<UserBlock>().Add(userBlock);
            await SaveChanges();
        }

        public async Task UpdateUserBlock(UserBlock userBlock)
        {
            _dbContext.Set<UserBlock>().Update(userBlock);
            await SaveChanges();
        }

        public async Task DeleteUserBlock(UserBlock userBlock)
        {
            _dbContext.Set<UserBlock>().Remove(userBlock);
            await SaveChanges();
        }

        public async Task<ICollection<UserBlock>> GetAllBlocks()
        {
            return await _dbContext.Set<UserBlock>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<UserBlock>> SearchBlockedUsers(int userId)
        {
            return await _dbContext.Set<UserBlock>().Where(ub => ub.SourceUserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task<UserBlock> SearchForBlockedUser(int userId, int blockedUserId)
        {
            return await _dbContext.Set<UserBlock>().Where(ub => ub.SourceUserId == userId && ub.BlockedUserId == blockedUserId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
