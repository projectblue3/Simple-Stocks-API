using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface ICommentRepo
    {
        Task<ICollection<Comment>> GetAllComments();
        Task<ICollection<User>> GetLikesOfComment(int id);
        Task AddComment(Comment comment);
        Task UpdateComment(Comment comment);
        Task DeleteComment(Comment comment);
        Task<Comment> GetCommentById(int id);
        Task SaveChanges();
    }
}
