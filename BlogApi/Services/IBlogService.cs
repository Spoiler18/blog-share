using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IBlogService
    {
        Task<List<DetailedBlogApplications>> GetBlogsListForDashboard();
        Task<List<DetailedBlogApplications>> GetBlogsListForBlogs();
        Task<DetailedBlogApplications> GetBlogDetail(int? id);
        Task<ResponseModel> AddBlogAsync(AddEditBlogApplications blogApplications);
        Task<ResponseModel> EditBlogAsync(AddEditBlogApplications blogApplications);
        Task<ResponseModel> DeleteBlogAsync(int? blogApplicationId);
        Task<List<Notifications>> GetUserNotifications(int? userId);
    }
}
