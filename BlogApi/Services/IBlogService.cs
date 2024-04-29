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
        Task<List<DetailedBlogApplications>> GetTopBlogs(bool Alltime, DateTime? fromDate, DateTime? toDate);
        Task<List<PopularUsers>> GetTopBloggers(bool Alltime, DateTime? fromDate, DateTime? toDate);
        Task<List<DetailedBlogApplications>> GetBlogSummary(bool Alltime, DateTime? fromDate, DateTime? toDate);
        Task<DetailedBlogApplications> GetBlogDetailPerodically(int? blogId, bool Alltime, DateTime? fromDate, DateTime? toDate);
    }
}
