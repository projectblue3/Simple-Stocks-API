using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class PostTagRepo : IPostTagRepo
    {
        private readonly StocksDbContext _dbContext;
        public PostTagRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddPostTag(PostTag postTag)
        {
            await _dbContext.Set<PostTag>().AddAsync(postTag);
            await SaveChanges();
        }

        public async Task UpdatePostTag(PostTag postTag)
        {
            _dbContext.Set<PostTag>().Update(postTag);
            await SaveChanges();
        }

        public async Task DeletePostTag(PostTag postTag)
        {
            _dbContext.Set<PostTag>().Remove(postTag);
            await SaveChanges();
        }

        public async Task<ICollection<PostTag>> GetAllPostTags()
        {
            return await _dbContext.Set<PostTag>().AsNoTracking().ToListAsync();
        }

        public async Task<PostTag> SearchByPostAndTagIds(int postId, int tagId)
        {
            return await _dbContext.Set<PostTag>().Where(pt => pt.PostId == postId && pt.TagId == tagId).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<PostTag>> SearchByPostId(int postId)
        {
            return await _dbContext.Set<PostTag>().Where(pt => pt.PostId == postId).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<PostTag>> SearchByTagId(int tagId)
        {
            return await _dbContext.Set<PostTag>().Where(pt => pt.TagId == tagId).AsNoTracking().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
