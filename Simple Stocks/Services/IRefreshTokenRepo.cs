using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public interface IRefreshTokenRepo
    {
        Task<RefreshToken> GetRefreshTokenByOwner(int ownerId);
        string ReadToken();
        Task AddToken(RefreshToken token);
        Task DeleteToken(RefreshToken token);
        Task UpdateToken(RefreshToken token);
        Task SaveChanges();
    }
}
