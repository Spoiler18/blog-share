using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IBlogReactionService
    {
        Task<List<DetailedReaction>> GetBlogReaction(int? blogId);
        Task<ResponseModel> AddBlogReaction(BlogReaction reaction);
        Task<ResponseModel> EditBlogReaction(BlogReaction reaction);
        Task<ResponseModel> DeleteBlogReaction(int? reactionId);
    }
}
