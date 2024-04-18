using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IBlogService
    {
        Task<List<BlogApplications>> GetBlogsList();
        Task<ResponseModel> AddBlogAsync(BlogApplications blogApplications);
        Task<ResponseModel> EditBlogAsync(BlogApplications blogApplications);
        Task<ResponseModel> DeleteBlogAsync(int? blogApplicationId);
    }
}
