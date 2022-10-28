using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class LikedCommentRepo : ILikedCommentRepo
    {
        private readonly StocksDbContext _dbContext;

        public LikedCommentRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LikeComment(LikedComment likedComment)
        {
            await _dbContext.Set<LikedComment>().AddAsync(likedComment);
            await SaveChanges();
        }

        public async Task UpdateLikedComment(LikedComment likedComment)
        {
            _dbContext.Set<LikedComment>().Update(likedComment);
            await SaveChanges();
        }

        public async Task DeleteLikedComment(LikedComment likedComment)
        {
            _dbContext.Set<LikedComment>().Remove(likedComment);
            await SaveChanges();
        }

        public async Task<ICollection<LikedComment>> GetAllLikedComments()
        {
            return await _dbContext.Set<LikedComment>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<LikedComment>> SearchByCommentId(int commentId)
        {
            return await _dbContext.Set<LikedComment>().Where(lc => lc.CommentId == commentId).AsNoTracking().ToListAsync();
        }

        public async Task<LikedComment> SearchByUserAndCommentIds(int userId, int commentId)
        {
            return await _dbContext.Set<LikedComment>().Where(lc => lc.UserId == userId && lc.CommentId == commentId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<LikedComment>> SearchByUserId(int userId)
        {
            return await _dbContext.Set<LikedComment>().Where(lc => lc.UserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }        
    }
}
