using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class RightRepo : IRightRepo
    {
        private readonly StocksDbContext _dbContext;

        public RightRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRight(Right right)
        {
            await _dbContext.Set<Right>().AddAsync(right);
            await SaveChanges();
        }

        public async Task UpdateRight(Right right)
        {
            _dbContext.Set<Right>().Update(right);
            await SaveChanges();
        }

        public async Task DeleteRight(Right right)
        {
            _dbContext.Set<Right>().Remove(right);
            await SaveChanges();
        }

        public async Task<ICollection<Right>> GetAllRights()
        {
            return await _dbContext.Set<Right>().AsNoTracking().ToListAsync();
        }
        public async Task<ICollection<Role>> GetAllRolesWithRight(int id)
        {
            return await _dbContext.Set<RoleRight>().Where(rr => rr.Right.Id == id).Select(rr => rr.Role).AsNoTracking().ToListAsync();
        }

        public async Task<Right> GetRightById(int id)
        {
            return await _dbContext.Set<Right>().Where(r => r.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Right> GetRightByTitle(string title)
        {
            return await _dbContext.Set<Right>().Where(r => r.Title == title).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<bool> IsDuplicate(string title)
        {
            var rightToCheck = await _dbContext.Set<Right>().Where(r => r.Title == title).AsNoTracking().FirstOrDefaultAsync();

            if (rightToCheck == null)
            {
                return false;
            }

            return true;
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
