using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class CommentRepo : ICommentRepo
    {
        private readonly StocksDbContext _dbContext;
        public CommentRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddComment(Comment comment)
        {
            await _dbContext.Set<Comment>().AddAsync(comment);
            await SaveChanges();
        }

        public async Task UpdateComment(Comment comment)
        {
            _dbContext.Set<Comment>().Update(comment);
            await SaveChanges();
        }

        public async Task DeleteComment(Comment comment)
        {
            _dbContext.Set<Comment>().Remove(comment);
            await SaveChanges();
        }

        public async Task<ICollection<Comment>> GetAllComments()
        {
            return await _dbContext.Set<Comment>().AsNoTracking().ToListAsync();
        }

        public async Task<Comment> GetCommentById(int id)
        {
            return await _dbContext.Set<Comment>().Where(c => c.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<User>> GetLikesOfComment(int id)
        {
            return await _dbContext.Set<LikedComment>().Where(lc => lc.Comment.Id == id).Select(lc => lc.User).AsNoTracking().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
