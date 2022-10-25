using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;
using System.Security.Claims;

namespace Simple_Stocks.Services
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly StocksDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefreshTokenRepo(StocksDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddToken(RefreshToken token)
        {
            await _dbContext.Set<RefreshToken>().AddAsync(token);
            await SaveChanges();
        }

        public async Task DeleteToken(RefreshToken token)
        {
            _dbContext.Set<RefreshToken>().Remove(token);
            await SaveChanges();
        }

        public async Task<RefreshToken> GetRefreshTokenByOwner(int ownerId)
        {
            return await _dbContext.Set<RefreshToken>().Where(t => t.UserID == ownerId).AsNoTracking().FirstOrDefaultAsync();
        }

        public string ReadToken()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateToken(RefreshToken token)
        {
            _dbContext.Set<RefreshToken>().Update(token);
            await SaveChanges();
        }
    }
}
