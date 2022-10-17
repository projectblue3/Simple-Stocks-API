using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class RoleRepo : IRoleRepo
    {
        private readonly StocksDbContext _dbContext;

        public RoleRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRole(Role role, List<int> rightIds)
        {
            await _dbContext.Set<Role>().AddAsync(role);
            await SaveChanges();

            foreach (int id in rightIds)
            {
                var roleRight = new RoleRight()
                {
                    RoleId = role.Id,
                    RightId = id
                };

                await _dbContext.Set<RoleRight>().AddAsync(roleRight);
                await SaveChanges();
            }
        }

        public async Task UpdateRole(Role role)
        {
            _dbContext.Set<Role>().Update(role);
            await SaveChanges();
        }

        public async Task DeleteRole(Role role)
        {
            _dbContext.Set<Role>().Remove(role);
            await SaveChanges();
        }

        public async Task<ICollection<Role>> GetAllRoles()
        {
            return await _dbContext.Set<Role>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<User>> GetAllUsersWithRole(int roleId)
        {
            return await _dbContext.Set<User>().Where(r => r.RoleId == roleId).AsNoTracking().ToListAsync();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _dbContext.Set<Role>().Where(r => r.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Role> GetRoleByTitle(string title)
        {
            return await _dbContext.Set<Role>().Where(r => r.Title == title).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<Right>> GetRightsOfRole(int id)
        {
            return await _dbContext.Set<RoleRight>().Where(ro => ro.Role.Id == id).Select(ro => ro.Right).AsNoTracking().ToListAsync();
        }

        public async Task<bool> IsDuplicate(string title)
        {
            var roleToCheck = await _dbContext.Set<Role>().Where(r => r.Title == title).AsNoTracking().FirstOrDefaultAsync();

            if(roleToCheck == null) {
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
