using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IBlogService
    {
        Task<List<BlogApplications>> GetBlogsList();
    }
}
