using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IBlogService
    {
        Task<List<DetailedBlogApplications>> GetBlogsListForDashboard();
        Task<List<DetailedBlogApplications>> GetBlogsListForBlogs();
        Task<DetailedBlogApplications> GetBlogDetail(int? id);
        Task<ResponseModel> AddBlogAsync(BlogApplications blogApplications);
        Task<ResponseModel> EditBlogAsync(BlogApplications blogApplications);
        Task<ResponseModel> DeleteBlogAsync(int? blogApplicationId);
    }
}
