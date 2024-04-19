using BlogApi.Models;

namespace BlogApi.Services
{
    public interface ICommentReactionService
    {
        Task<ResponseModel> AddCommentReaction(CommentReaction commentReaction);
        Task<ResponseModel> EditBlogCommentReaction(CommentReaction commentReaction);
        Task<ResponseModel> DeleteBlogComment(int? commentReactionId);
    }
}
