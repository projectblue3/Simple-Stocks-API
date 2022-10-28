using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface ILikedCommentRepo
    {
        Task<ICollection<LikedComment>> GetAllLikedComments();
        Task<LikedComment> SearchByUserAndCommentIds(int userId, int commentId);
        Task<ICollection<LikedComment>> SearchByUserId(int userId);
        Task<ICollection<LikedComment>> SearchByCommentId(int commentId);
        Task LikeComment(LikedComment likedComment);
        Task UpdateLikedComment(LikedComment likedComment);
        Task DeleteLikedComment(LikedComment likedComment);
        Task SaveChanges();
    }
}
