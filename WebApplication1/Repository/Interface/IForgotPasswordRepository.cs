using WebApplication1.Models.Auth;

namespace WebApplication1.Repository.Interface
{
    public interface IForgotPasswordRepository
    {
        Task AddAsync(ForgotPassword resetTokenEntry);
        Task<ForgotPassword?> GetByTokenHashAsync(string incomingHash);
        Task<int> GetRecentRequestsCountAsync(Guid id, DateTime dateTime);
        Task InvalidatePreviousTokensAsync(Guid id);
        Task UpdateAsync(ForgotPassword resetTokenEntry);
    }
}
