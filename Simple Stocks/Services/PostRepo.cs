using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class PostRepo : IPostRepo
    {
        private readonly StocksDbContext _dbContext;
        public PostRepo(StocksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddPost(Post post, List<int> tagIds)
        {
            await _dbContext.Set<Post>().AddAsync(post);
            await SaveChanges();

            if (tagIds.Count > 0) {
                foreach (int id in tagIds)
                {
                    var postTag = new PostTag()
                    {
                        PostId = post.Id,
                        TagId = id
                    };

                    await _dbContext.Set<PostTag>().AddAsync(postTag);
                    await SaveChanges();
                }
            }
        }

        public async Task UpdatePost(Post post)
        {
            _dbContext.Set<Post>().Update(post);
            await SaveChanges();
        }

        public async Task DeletePost(Post post)
        {
            _dbContext.Set<Post>().Remove(post);
            await SaveChanges();
        }

        public async Task<ICollection<Post>> FindPostsByContent(string searchTerm)
        {
            return await _dbContext.Set<Post>().Where(p => p.Text.Contains(searchTerm) && p.PostIsHidden == false && p.PostIsPrivate == false).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Post>> FindPostsByTitle(string searchTerm)
        {
            return await _dbContext.Set<Post>().Where(p => p.Title.Contains(searchTerm) && p.PostIsHidden == false && p.PostIsPrivate == false).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Post>> GetAllPostsModView()
        {
            return await _dbContext.Set<Post>().AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Post>> GetAllPostsUserView()
        {
            return await _dbContext.Set<Post>().Where(p => p.PostIsHidden == false && p.PostIsPrivate == false).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Tag>> GetAllTagsOfPost(int id)
        {
            return await _dbContext.Set<PostTag>().Where(pt => pt.Post.Id == id).Select(pt => pt.Tag).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<Comment>> GetAllCommentsOfPost(int id)
        {
            return await _dbContext.Set<Comment>().Where(c => c.PostId == id && c.CommentIsHidden == false && c.User.AccountIsHidden == false && c.User.AccountIsPrivate == false).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<User>> GetLikesOfPost(int id)
        {
            return await _dbContext.Set<LikedPost>().Where(lp => lp.Post.Id == id).Select(lp => lp.User).AsNoTracking().ToListAsync();
        }

        public async Task<Post> GetPostById(int id)
        {
            return await _dbContext.Set<Post>().Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        
    }
}
