using BlogApi.Models;

namespace BlogApi.Services
{
    public interface ICommentService
    {
        Task<List<DetailedComment>> GetBlogComments(int? blogId);
        Task<ResponseModel> AddBlogComment(BlogComment comment);
        Task<ResponseModel> EditBlogComment(BlogComment comment);
        Task<ResponseModel> DeleteBlogComment(int? commentId);
    }
}
