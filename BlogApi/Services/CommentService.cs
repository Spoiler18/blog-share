using BlogApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommentService(BlogContext blogContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _blogContext = blogContext;
        }

        public async Task<List<DetailedComment>> GetBlogComments(int? blogId)
        {
            var userData = await _blogContext.UserDetail.ToListAsync();
            var commentData = await _blogContext.BlogComments.Where(item => item.BlogId == blogId).ToListAsync();
            var commentReactionData = await _blogContext.CommentReactions.Where(item => commentData.Select(item => item.CommentId).ToList().Contains(((int)item.CommentId))).ToListAsync();

            var data = (from comment in commentData
                       join user in userData
                       on comment.UserId equals user.UserId
                       where comment.BlogId == blogId && comment.IsDeleted != true
                       select new DetailedComment
                       {
                           CommentId = comment.CommentId,
                           UserId = comment.UserId,
                           BlogId = comment.BlogId,
                           UserComment = comment.UserComment,
                           ReplyToCommentId = comment.ReplyToCommentId,
                           UserCommentFullName = user.FirstName + " " + user.LastName,
                       }).ToList();

            Parallel.ForEach(data, item =>
            {
                item.CommentReactions = (from CommentReactions in commentReactionData
                                         join user in userData
                                         on CommentReactions.UserId equals user.UserId
                                         where CommentReactions.CommentId == item.CommentId
                                         select new DetailedCommentReaction
                                         {
                                             CommentReactionId = CommentReactions.CommentReactionId,
                                             CommentId = CommentReactions.CommentId,
                                             UserId= user.UserId,
                                             UserReaction = CommentReactions.UserReaction,
                                             UserCommentReactionFullName = user.FirstName + " " + user.LastName
                                         }).ToList();
            });

            return data;
        }

        public async Task<ResponseModel> AddBlogComment(BlogComment comment)
        {
            ResponseModel response = new ResponseModel();

            if (comment.UserComment != null)
            {
                BlogComment newComment = new BlogComment
                {
                    UserId = comment.UserId,
                    BlogId = comment.BlogId,
                    UserComment = comment.UserComment,
                    ReplyToCommentId = comment.ReplyToCommentId,
                    CreatedOn = DateTime.Now,
                    CreatedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext),
                };

                try
                {
                    _blogContext.BlogComments.Add(newComment);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Added Successfully!!!";

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
                response.message = "Empty Comment!!!";
            }

            return response;
        }

        public async Task<ResponseModel> EditBlogComment(BlogComment comment)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogComment? editComment = await _blogContext.BlogComments.Where(item => item.CommentId == comment.CommentId && item.IsDeleted != true).FirstOrDefaultAsync();

                if (editComment != null)
                {
                    editComment.UserComment = comment.UserComment;
                    editComment.ModifiedOn = DateTime.Now;
                    editComment.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);

                    _blogContext.BlogComments.Update(editComment);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Edited Successfully!!!";
                }
                else
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Comment Not Found!!!";
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

        public async Task<ResponseModel> DeleteBlogComment(int? commentId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogComment? comment = await _blogContext.BlogComments.Where(x => x.CommentId == commentId && x.IsDeleted != true).FirstOrDefaultAsync();
                if (comment != null)
                {
                    comment.IsDeleted = true;
                    comment.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);
                    comment.ModifiedOn = DateTime.UtcNow;

                    _blogContext.BlogComments.Update(comment);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Deleted Successfully!!!";
                }
                else
                {
                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Has Already Been Deleted!!!";
                }
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
