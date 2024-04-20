using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class CommentReactionsService : ICommentReactionService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommentReactionsService(BlogContext blogContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _blogContext = blogContext;
        }

        public async Task<List<DetailedCommentReaction>> GetBlogCommentReactions(int? CommentId)
        {
            var data = (from commentReactions in _blogContext.CommentReactions
                        join user in _blogContext.UserDetail
                        on commentReactions.UserId equals user.UserId
                        where commentReactions.CommentId == CommentId
                        select new DetailedCommentReaction
                        {
                            CommentReactionId = commentReactions.CommentReactionId,
                            CommentId = commentReactions.CommentId,
                            UserId = commentReactions.UserId,
                            UserReaction = commentReactions.UserReaction,
                            UserCommentReactionFullName = user.FirstName + " " + user.LastName,
                        }).ToList();

            return data;
        }

        public async Task<ResponseModel> AddCommentReaction(CommentReaction commentReaction)
        {
            ResponseModel response = new ResponseModel();

            if (commentReaction.UserReaction != null)
            {
                CommentReaction newCommentReaction = new CommentReaction
                {
                    UserId = commentReaction.UserId,
                    CommentId = commentReaction.CommentId,
                    UserReaction = commentReaction.UserReaction,
                    CreatedOn = DateTime.Now,
                    CreatedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext),
                };

                try
                {
                    _blogContext.CommentReactions.Add(newCommentReaction);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Reaction Added Successfully!!!";

                }
                catch (Exception ex)
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Something Went Wrong!!!";
                }
            }
            else
            {
                response.isError = false;
                response.isSuccess = true;
                response.message = "Empty Comment Reaction!!!";
            }

            return response;
        }

        public async Task<ResponseModel> EditBlogCommentReaction(CommentReaction commentReaction)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                CommentReaction? editCommentReaction = await _blogContext.CommentReactions.Where(item => item.CommentReactionId == commentReaction.CommentReactionId).FirstOrDefaultAsync();

                if (editCommentReaction != null)
                {
                    editCommentReaction.UserReaction = commentReaction.UserReaction;
                    editCommentReaction.ModifiedOn = DateTime.Now;
                    editCommentReaction.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);

                    _blogContext.CommentReactions.Update(editCommentReaction);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Reaction Edited Successfully!!!";
                }
                else
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Comment Reaction Not Found!!!";
                }
            }
            catch (Exception ex)
            {
                response.isError = true;
                response.isSuccess = false;
                response.message = "Something Went Wrong!!!";
            }

            return response;
        }

        public async Task<ResponseModel> DeleteBlogComment(int? commentReactionId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var commentReaction = await _blogContext.CommentReactions.Where(reaction => reaction.CommentReactionId == commentReactionId).FirstOrDefaultAsync();
                _blogContext.CommentReactions.Remove(commentReaction);
                await _blogContext.SaveChangesAsync();

                response.isError = false;
                response.isSuccess = true;
                response.message = "Comment Reaction Deleted Successfully!!!";
            }
            catch
            {
                response.isError = false;
                response.isSuccess = true;
                response.message = "Something Went Wrong!!!";
            }

            return response;
        }
    }
}
