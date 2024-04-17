using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IAccountService
    {
        Task<List<UserDetails>> GetAllUsers();
        Task<UserDetails> GetSingleUser(int? id);
    }
}
