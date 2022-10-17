using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IPostTagRepo
    {
        Task<ICollection<PostTag>> GetAllPostTags();
        Task<PostTag> SearchByPostAndTagIds(int postId, int tagId);
        Task<ICollection<PostTag>> SearchByPostId(int postId);
        Task<ICollection<PostTag>> SearchByTagId(int tagId);
        Task AddPostTag(PostTag postTag);
        Task UpdatePostTag(PostTag postTag);
        Task DeletePostTag(PostTag postTag);
        Task SaveChanges();
    }
}
