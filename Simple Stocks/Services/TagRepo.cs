using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class TagRepo : ITagRepo
    {
        private readonly StocksDbContext _dbContext;

        public TagRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddTag(Tag tag)
        {
            await _dbContext.Set<Tag>().AddAsync(tag);
            await SaveChanges();
        }

        public async Task UpdateTag(Tag tag)
        {
            _dbContext.Set<Tag>().Update(tag);
            await SaveChanges();
        }

        public async Task DeleteTag(Tag tag)
        {
            _dbContext.Set<Tag>().Remove(tag);
            await SaveChanges();
        }

        public Task<ICollection<Post>> GetAllPostsWithTag()
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Tag>> GetAllTags()
        {
            return await _dbContext.Set<Tag>().AsNoTracking().ToListAsync();
        }

        public async Task<Tag> GetTagById(int id)
        {
            return await _dbContext.Set<Tag>().Where(t => t.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Tag> GetTagByTitle(string title)
        {
            return await _dbContext.Set<Tag>().Where(t => t.Text == title).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<bool> IsDuplicate(string title)
        {
            var tagToCheck = await _dbContext.Set<Tag>().Where(t => t.Text == title).AsNoTracking().FirstOrDefaultAsync();

            if (tagToCheck == null)
            {
                return false;
            }

            return true;
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<Tag>> SearchTags(string searchTerm)
        {
            return await _dbContext.Set<Tag>().Where(t => t.Text.Contains(searchTerm)).AsNoTracking().ToListAsync();
        }

    }
}
