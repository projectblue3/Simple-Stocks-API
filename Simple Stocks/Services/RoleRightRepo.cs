using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class RoleRightRepo : IRoleRightRepo
    {
        private readonly StocksDbContext _dbContext;

        public RoleRightRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRoleRight(RoleRight roleRight)
        {
            await _dbContext.Set<RoleRight>().AddAsync(roleRight);
            await SaveChanges();
        }

        public async Task UpdateRoleRight(RoleRight roleRight)
        {
            _dbContext.Set<RoleRight>().Update(roleRight);
            await SaveChanges();
        }

        public async Task DeleteRoleRight(RoleRight roleRight)
        {
            _dbContext.Set<RoleRight>().Remove(roleRight);
            await SaveChanges();
        }

        public async Task<ICollection<RoleRight>> GetAllRoleRights()
        {
            return await _dbContext.Set<RoleRight>().AsNoTracking().ToListAsync();
        }

        public async Task<RoleRight> SearchByRoleAndRightIds(int roleId, int rightId)
        {
            return await _dbContext.Set<RoleRight>().Where(rr => rr.RoleId == roleId && rr.RightId == rightId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<RoleRight>> SearchByRightId(int rightId)
        {
            return await _dbContext.Set<RoleRight>().Where(rr => rr.RightId == rightId).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<RoleRight>> SearchByRoleId(int roleId)
        {
            return await _dbContext.Set<RoleRight>().Where(rr => rr.RoleId == roleId).AsNoTracking().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
