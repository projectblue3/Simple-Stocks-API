using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface ILikedPostRepo
    {
        Task<ICollection<LikedPost>> GetAllLikedPosts();
        Task<LikedPost> SearchByUserAndPostIds(int userId, int postId);
        Task<ICollection<LikedPost>> SearchByUserId(int userId);
        Task<ICollection<LikedPost>> SearchByPostId(int postId);
        Task UpdateLikedPost(LikedPost likedPost);
        Task DeleteLikedPost(LikedPost likedPost);
        Task SaveChanges();
    }
}
