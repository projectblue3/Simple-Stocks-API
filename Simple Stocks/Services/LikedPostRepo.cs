using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class LikedPostRepo : ILikedPostRepo
    {
        private readonly StocksDbContext _dbContext;

        public LikedPostRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateLikedPost(LikedPost likedPost)
        {
            _dbContext.Set<LikedPost>().Update(likedPost);
            await SaveChanges();
        }

        public async Task DeleteLikedPost(LikedPost likedPost)
        {
            _dbContext.Set<LikedPost>().Remove(likedPost);
            await SaveChanges();
        }

        public async Task<ICollection<LikedPost>> GetAllLikedPosts()
        {
            return await _dbContext.Set<LikedPost>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<LikedPost>> SearchByPostId(int postId)
        {
            return await _dbContext.Set<LikedPost>().Where(lp => lp.PostId == postId).AsNoTracking().ToListAsync();
        }

        public async Task<LikedPost> SearchByUserAndPostIds(int userId, int postId)
        {
            return await _dbContext.Set<LikedPost>().Where(lp => lp.UserId == userId && lp.PostId == postId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<LikedPost>> SearchByUserId(int userId)
        {
            return await _dbContext.Set<LikedPost>().Where(lp => lp.UserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
