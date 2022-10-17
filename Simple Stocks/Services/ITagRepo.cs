using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface ITagRepo
    {
        Task<ICollection<Tag>> GetAllTags();
        Task<ICollection<Post>> GetAllPostsWithTag();
        Task<ICollection<Tag>> SearchTags(string searchTerm);
        Task<Tag> GetTagById(int id);
        Task<Tag> GetTagByTitle(string title);
        Task AddTag(Tag tag);
        Task UpdateTag(Tag tag);
        Task DeleteTag(Tag tag);
        Task<bool> IsDuplicate(string title);
        Task SaveChanges();
    }
}
