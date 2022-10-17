using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IPostRepo
    {
        Task<ICollection<Post>> GetAllPostsModView();
        Task<ICollection<Post>> GetAllPostsUserView();
        Task<ICollection<Tag>> GetAllTagsOfPost(int id);
        Task<ICollection<Comment>> GetAllCommentsOfPost(int id);
        Task<ICollection<User>> GetLikesOfPost(int id);
        Task<ICollection<Post>> FindPostsByTitle(string searchTerm);
        Task<ICollection<Post>> FindPostsByContent(string searchTerm);
        Task<Post> GetPostById(int id);
        Task AddPost(Post post, List<int> tagIds);
        Task UpdatePost(Post post);
        Task DeletePost(Post post);
        Task SaveChanges();
    }
}
